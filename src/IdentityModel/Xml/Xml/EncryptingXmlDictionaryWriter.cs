using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using RSAKeyValue = System.Security.Cryptography.Xml.RSAKeyValue;
using KeyInfo = System.Security.Cryptography.Xml.KeyInfo;

namespace Solid.IdentityModel.Xml
{
    public delegate bool StartEncryptionDelegate(string prefix, string localName, string ns);
    public class EncryptingXmlDictionaryWriter : DelegatingXmlDictionaryWriter
    {
        private static readonly StartEncryptionDelegate _yes = (_, __, ___) => true;

        private XmlDictionaryWriter _writer;
        private EncryptingCredentials _credentials;
        private StartEncryptionDelegate _startEncryption;
        private int _encryptedElements;
        private int _elements;
        private MemoryStream _stream;
        private XmlDictionaryWriter _innerWriter;
        private MemoryStream _encryptedStream;
        private XmlDictionaryWriter _encryptingWriter;

        protected CryptoProviderFactory Crypto => _credentials.CryptoProviderFactory ?? _credentials.Key.CryptoProviderFactory ?? CryptoProviderFactory.Default;
        
        /// <summary>
        /// Encrypts an XML element while writing.
        /// </summary>
        /// <param name="writer">The original <see cref="XmlWriter"/>.</param>
        /// <param name="credentials">The <see cref="EncryptingCredentials"/> used to encrypt the XML element.</param>
        public EncryptingXmlDictionaryWriter(XmlWriter writer, EncryptingCredentials credentials) : this(writer, credentials, _yes) { }

        /// <summary>
        /// Encrypts an XML element while writing.
        /// </summary>
        /// <param name="writer">The original <see cref="XmlDictionaryWriter"/>.</param>
        /// <param name="credentials">The <see cref="EncryptingCredentials"/> used to encrypt the XML element.</param>
        public EncryptingXmlDictionaryWriter(XmlDictionaryWriter writer, EncryptingCredentials credentials) : this(writer, credentials, _yes) { }
            

        /// <summary>
        /// Encrypts an XML element while writing.
        /// </summary>
        /// <param name="writer">The original <see cref="XmlWriter"/>.</param>
        /// <param name="credentials">The <see cref="EncryptingCredentials"/> used to encrypt the XML element.</param>
        /// <param name="startEncryption">A delegate to decide on what element encryption should start.</param>
        public EncryptingXmlDictionaryWriter(XmlWriter writer, EncryptingCredentials credentials, StartEncryptionDelegate startEncryption)
            : this(CreateDictionaryWriter(writer), credentials, startEncryption)
        {
        }

        /// <summary>
        /// Encrypts an XML element while writing.
        /// </summary>
        /// <param name="writer">The original <see cref="XmlDictionaryWriter"/>.</param>
        /// <param name="credentials">The <see cref="EncryptingCredentials"/> used to encrypt the XML element.</param>
        /// <param name="startEncryption">A delegate to decide on what element encryption should start.</param>
        public EncryptingXmlDictionaryWriter(XmlDictionaryWriter writer, EncryptingCredentials credentials, StartEncryptionDelegate startEncryption)
        {
            _writer = writer;
            _credentials = credentials;
            _startEncryption = startEncryption;

            var settings = _writer.Settings ?? new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = new UTF8Encoding(false) };

            _elements = 0;
            _stream = new MemoryStream();
            _innerWriter = CreateDictionaryWriter(Create(_stream, settings)); 
            _encryptedStream = new MemoryStream();
            _encryptingWriter = CreateDictionaryWriter(Create(_encryptedStream, settings));
            InnerWriter = _innerWriter;
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            _elements++;
            if (_encryptedElements > 0 || _startEncryption(prefix, localName, ns))
            {
                base.Flush();
                InnerWriter = _encryptingWriter;
                _encryptedElements++;
            }
            base.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteFullEndElement()
        {
            base.WriteFullEndElement();
            HandleEndElement();
        }

        public override void WriteEndElement()
        {
            base.WriteEndElement();
            HandleEndElement();
        }

        private void HandleEndElement()
        {
            if (_encryptedElements > 0)
            {
                if (--_encryptedElements == 0)
                {
                    base.Flush();
                    InnerWriter = _innerWriter;
                    Encrypt();
                    _encryptingWriter.Dispose();
                }
            }
            if (--_elements == 0)
            {
                base.Flush();
                Complete();
            }
        }

        private void Complete()
        {
            _stream.Position = 0;
            using (var reader = XmlReader.Create(_stream, new XmlReaderSettings { CloseInput = false }))
                _writer.WriteNode(reader, true);
        }

        private void Encrypt()
        {
            _encryptedStream.Position = 0;
            var algorithm = _credentials.Enc;

            var document = new XmlDocument();
            document.Load(_encryptedStream);

            var symmetric = Crypto.CreateSymmetricAlgorithm(algorithm);
            var xml = new EncryptedXml();
            xml.Mode = symmetric.Mode;
            var cipherText = xml.EncryptData(document.DocumentElement, symmetric, false);

            var keyInfo = new KeyInfo();
            var data = new EncryptedData
            {
                Type = EncryptedXml.XmlEncElementUrl,
                EncryptionMethod = new EncryptionMethod(algorithm),
                CipherData = new CipherData { CipherValue = cipherText }
            };

            if (_credentials.Key is RsaSecurityKey rsa)
                keyInfo.AddClause(CreateEncryptedKeyClause(symmetric.Key, rsa.Rsa, document));
            else if (_credentials.Key is X509SecurityKey x509)
                keyInfo.AddClause(CreateEncryptedKeyClause(symmetric.Key, x509.Certificate, document));
            else if (!string.IsNullOrEmpty(_credentials.Key.KeyId))
                keyInfo.AddClause(new KeyInfoName(_credentials.Key.KeyId));

            if (keyInfo.Cast<KeyInfoClause>().Any())
                data.KeyInfo = keyInfo;

            var element = data.GetXml();

            //element = AddDigestMethodToEncryptionMethod(element, "http://www.w3.org/2000/09/xmldsig#sha1");

            WriteNode(element.CreateNavigator(), true);

            Crypto.ReleaseSymmetricAlgorithm(symmetric);
        }

        private KeyInfoClause CreateEncryptedKeyClause(byte[] key, RSA rsa, XmlDocument document)
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(CreateKeyInfoClause(rsa));
            return CreateEncryptedKeyClause(key, keyInfo);
        }

        private KeyInfoClause CreateEncryptedKeyClause(byte[] key, X509Certificate2 certificate, XmlDocument document)
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(CreateKeyInfoClause(certificate, document, out _));
            return CreateEncryptedKeyClause(key, keyInfo);
        }

        private KeyInfoClause CreateEncryptedKeyClause(byte[] key, KeyInfo keyInfo)
        {
            if (!Crypto.IsSupportedAlgorithm(_credentials.Alg, _credentials.Key))
                throw new NotSupportedException(_credentials.Alg);

            var keyWrap = Crypto.CreateKeyWrapProvider(_credentials.Key, _credentials.Alg);
            var cipherValue = keyWrap.WrapKey(key);
            var encryptedKey = new EncryptedKey
            {
                CipherData = new CipherData { CipherValue = cipherValue },
                EncryptionMethod = new EncryptionMethod(_credentials.Alg),
                KeyInfo = keyInfo
            };

            return new KeyInfoEncryptedKey { EncryptedKey = encryptedKey };
        }

        private KeyInfoClause CreateKeyInfoClause(RSA rsa)
        {
            return new RSAKeyValue(rsa);
        }

        private KeyInfoClause CreateKeyInfoClause(X509Certificate2 certificate, XmlDocument document, out string digest)
        {
            const string algorithm = "http://www.w3.org/2000/09/xmldsig#sha1";
            if (Crypto.IsSupportedAlgorithm(algorithm))
            {
                var sha1 = Crypto.CreateHashAlgorithm(algorithm);
                var securityTokenReference = document.CreateElement(WsSecurityConstants.WsSecurity10.Prefix, "SecurityTokenReference", WsSecurityConstants.WsSecurity10.Namespace);
                var keyIdentifier = document.CreateElement(WsSecurityConstants.WsSecurity10.Prefix, "KeyIdentifier", WsSecurityConstants.WsSecurity10.Namespace);
                keyIdentifier.SetAttribute("ValueType", "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#ThumbprintSHA1");
                keyIdentifier.InnerText = GetCertificateThumbprintHash(algorithm, certificate);
                securityTokenReference.AppendChild(keyIdentifier);
                digest = algorithm;
                return new KeyInfoNode(securityTokenReference);
            }
            else
            {
                digest = null;
                return new KeyInfoX509Data(certificate, X509IncludeOption.None);
            }
        }

        private string GetCertificateThumbprintHash(string algorithm, X509Certificate2 certificate)
        {
            var publicKey = certificate.Export(X509ContentType.Cert);
            var hash = Crypto.CreateHashAlgorithm(algorithm);
            try
            {
                var hashed = hash.ComputeHash(publicKey);
                return Convert.ToBase64String(hashed);
            }
            finally
            {
                Crypto.ReleaseHashAlgorithm(hash);
            }
        }

        // shim
        abstract class WsSecurityConstants
        {
            public abstract string Prefix { get; }
            public abstract string Namespace { get; }

            public static readonly WsSecurityConstants WsSecurity10 = new WsSecurity10Constants();

            public class WsSecurity10Constants : WsSecurityConstants
            {
                public override string Prefix => "wsse";

                public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
            }
        }
    }
}
