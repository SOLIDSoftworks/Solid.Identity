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

namespace Solid.IdentityModel.Xml
{
    public class DecryptingXmlDictionaryReader : DelegatingXmlDictionaryReader
    {
        private XmlDictionaryReader _inner;
        private TokenValidationParameters _tokenValidationParameters;

        public DecryptingXmlDictionaryReader(XmlReader reader, TokenValidationParameters tokenValidationParameters)
        {
            _inner = CreateDictionaryReader(reader);
            _tokenValidationParameters = tokenValidationParameters;
            InnerReader = _inner;
            ProcessCurrentElement();
        }

        public EncryptedData EncryptedData { get; private set; }
        public byte[] PlainText { get; private set; }

        public override bool Read()
        {
            base.Read();
            ProcessCurrentElement();
            return !InnerReader.EOF;
        }

        protected CryptoProviderFactory GetCrypto(SecurityKey key)
            => key.CryptoProviderFactory ?? CryptoProviderFactory.Default;

        private void ProcessCurrentElement()
        {
            if(!ReferenceEquals(InnerReader, _inner)) // already decrypting
            {
                if (InnerReader.EOF)
                {
                    InnerReader.Dispose();
                    InnerReader = _inner;
                }
            }
            else if(TryReadEncryptedData(out var data))
            {
                EncryptedData = data;
                if (!TryDecrypt(data, out var plainText))
                    throw new CryptographicException("Unable to decrypt encrypted XML.");
                PlainText = plainText;
                var stream = new MemoryStream(plainText);
                InnerReader = CreateDictionaryReader(Create(stream, InnerReader.Settings));
                InnerReader.MoveToContent();
            }
        }

        private bool TryReadEncryptedData(out EncryptedData encryptedData)
        {
            if(!InnerReader.IsStartElement("EncryptedData", EncryptedXml.XmlEncNamespaceUrl))
                return Out.False(out encryptedData);

            var xml = InnerReader.ReadOuterXml();
            var document = new XmlDocument();
            document.LoadXml(xml);
            var data = new EncryptedData();
            data.LoadXml(document.DocumentElement);

            encryptedData = data;
            return true;
        }

        private bool TryDecrypt(EncryptedType encryptedType, out byte[] plainText)
        {
            var xml = new EncryptedXml();
            var keys = GetSecurityKeys(encryptedType);
            var algorithm = encryptedType.EncryptionMethod.KeyAlgorithm;
            foreach(var key in keys)
            {
                var crypto = GetCrypto(key);
                if (!crypto.IsSupportedAlgorithm(algorithm, key)) continue;
                var symmetric = null as SymmetricAlgorithm;
                try
                {
                    if (encryptedType is EncryptedData encryptedData)
                    {
                        if (!(key is SymmetricSecurityKey symmetricKey)) continue;
                        symmetric = crypto.CreateSymmetricAlgorithm(symmetricKey, algorithm);
                        symmetric.IV = xml.GetDecryptionIV(encryptedData, algorithm);
                        var pt = xml.DecryptData(encryptedData, symmetric);
                        plainText = pt;
                        return true;
                    }

                    if (encryptedType is EncryptedKey encryptedKey)
                    {
                        var pt = null as byte[];
                        var keyWrapAlgorithm = encryptedKey.EncryptionMethod.KeyAlgorithm;
                        if(crypto.IsSupportedAlgorithm(algorithm, key))
                        {
                            var keyWrap = crypto.CreateKeyWrapProviderForUnwrap(key, encryptedKey.EncryptionMethod.KeyAlgorithm);
                            try
                            {
                                pt = keyWrap.UnwrapKey(encryptedKey.CipherData.CipherValue);
                                plainText = pt;
                                return true;
                            }
                            finally
                            {
                                crypto.ReleaseKeyWrapProvider(keyWrap);
                            }
                        }
                    }
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    if (symmetric != null)
                    {
                        crypto.ReleaseSymmetricAlgorithm(symmetric);
                        symmetric = null;
                    }
                }
            }
            return Out.False(out plainText);
        }

        private IEnumerable<SecurityKey> GetSecurityKeys(EncryptedType encryptedType)
        {
            var keys = new List<SecurityKey>();
            foreach (var clause in encryptedType.KeyInfo.Cast<KeyInfoClause>())
            {
                if (clause is KeyInfoEncryptedKey encryptedKey && TryDecrypt(encryptedKey.EncryptedKey, out var symmetricKey))
                    keys.Add(new SymmetricSecurityKey(symmetricKey));
                if (clause is KeyInfoName name)
                    keys.AddRange(_tokenValidationParameters.ResolveDecryptionKeys(kid: name.Value));
                if (clause is KeyInfoX509Data x509)
                    keys.AddRange(x509.Certificates.Cast<X509Certificate2>().Select(cert => new X509SecurityKey(cert)));
                if (clause is RSAKeyValue rsa)
                {
                    var key = new RsaSecurityKey(rsa.Key);
                    keys.AddRange(_tokenValidationParameters.ResolveDecryptionKeys(kid: key.KeyId));
                    if (key.PrivateKeyStatus != PrivateKeyStatus.DoesNotExist)
                        keys.Add(key);
                }
                if (clause is KeyInfoNode node)
                {
                    if(node.Value.LocalName == "SecurityTokenReference")
                        keys.AddRange(_tokenValidationParameters.ResolveDecryptionKeys(kid: node.Value.InnerText));
                }
            }
            return keys;
        }
    }
}
