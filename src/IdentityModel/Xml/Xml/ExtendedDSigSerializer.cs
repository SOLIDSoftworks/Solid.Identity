using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using Legacy = System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Xml.XPath;
using Solid.IdentityModel.Protocols;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.IdentityModel.Xml
{
    public class ExtendedDSigSerializer
    #if NET6_0
        : ExtendableDSigSerializer
    #else
        : DSigSerializer
    #endif
    {
        static ExtendedDSigSerializer()
        {
            Default = new ExtendedDSigSerializer();
        }

        public static void SetDefault(ExtendedDSigSerializer serializer)
        {
            Default = serializer;
        }

        protected override bool TryReadKeyInfoType(XmlReader reader, ref KeyInfo keyInfo)
        {
            if (TryReadEncryptedKey(reader, out var encryptedKey))
                keyInfo = encryptedKey;
            else if (TryReadBinarySecret(reader, out var binarySecret))
                keyInfo = binarySecret;
            else if (TryReadX509SecurityTokenReference(reader, out var reference))
                keyInfo = reference;
            else
                return base.TryReadKeyInfoType(reader, ref keyInfo);

            return true;
        }

        protected virtual bool TryReadEncryptedKey(XmlReader reader, out KeyInfo keyInfo)
        {
            if (!reader.IsStartElement(XmlEncConstants.Elements.EncryptedKey, XmlEncConstants.Namespace))
                return Out.False(out keyInfo);

            var encryptedKeyInfo = new EncryptedKeyInfo();

            while (reader.Read())
            {
                if (reader.IsStartElement(XmlEncConstants.Elements.EncryptionMethod, XmlEncConstants.Namespace))
                {
                    encryptedKeyInfo.EncryptionMethod = reader.GetAttribute("Algorithm");
                    continue;
                }

                if (reader.IsStartElement(XmlSignatureConstants.Elements.DigestMethod, XmlSignatureConstants.Namespace))
                {
                    encryptedKeyInfo.DigestMethod = reader.GetAttribute("Algorithm");
                    continue;
                }

                if (reader.IsStartElement(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace))
                {
                    encryptedKeyInfo.KeyInfo = ReadKeyInfo(reader);
                    continue;
                }

                if (reader.IsStartElement(XmlEncConstants.Elements.CipherData, XmlEncConstants.Namespace)) continue;
                if (reader.IsStartElement(XmlEncConstants.Elements.CipherValue, XmlEncConstants.Namespace))
                {
                    var base64 = reader.ReadElementContentAsString();
                    encryptedKeyInfo.CipherValue = Convert.FromBase64String(base64);
                    continue;
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlEncConstants.Elements.EncryptedKey)
                {
                    reader.Read();
                    break;
                }
            } 
            keyInfo = encryptedKeyInfo;
            return true;
        }

        protected virtual bool TryReadBinarySecret(XmlReader reader, out KeyInfo keyInfo)
        {
            const string BinarySecret = nameof(BinarySecret);
            if (!reader.IsStartElement(BinarySecret, WsTrustConstants.TrustFeb2005.Namespace) &&
                !reader.IsStartElement(BinarySecret, WsTrustConstants.Trust13.Namespace) &&
                !reader.IsStartElement(BinarySecret, WsTrustConstants.Trust14.Namespace))
            {
                return Out.False(out keyInfo);
            }

            var base64 = reader.ReadElementContentAsString();
            var key = Convert.FromBase64String(base64);

            keyInfo = new BinarySecretKeyInfo
            {
                Key = key
            };
            return true;
        }

        protected virtual bool TryReadX509SecurityTokenReference(XmlReader reader, out KeyInfo keyInfo)
        {
            if(!reader.IsStartElement("SecurityTokenReference", WsSecurityConstants.WsSecurity10.Namespace))
                return Out.False(out keyInfo);

            reader.Read();
            if(reader.IsStartElement("KeyIdentifier", WsSecurityConstants.WsSecurity10.Namespace))
            {
                var valueType = reader.GetAttribute("ValueType");
                var id = reader.ReadElementContentAsString();
                keyInfo = new SecurityTokenReferenceKeyInfo
                {
                    KeyIdValueType = valueType,
                    KeyId = id
                };
            }
            else
            {
                // throw exception?
                return Out.False(out keyInfo);
            }
            return true;
        }

        //public override KeyInfo ReadKeyInfo(XmlReader reader)
        //{
        //    const string xmlenc = "http://www.w3.org/2001/04/xmlenc#";

        //    // buffer element
        //    var document = XDocument.Load(reader.ReadSubtree());
        //    var keyInfo = document.Root;
        //    var child = keyInfo.Nodes().OfType<XElement>().FirstOrDefault();
        //    reader.Read();

        //    if (child != null)
        //    {
        //        if (child.Name == XName.Get("EncryptedKey", xmlenc))
        //        {

        //        }

        //        if (child.Name == XName.Get("BinarySecret", WsTrustConstants.TrustFeb2005.Namespace) ||
        //            child.Name == XName.Get("BinarySecret", WsTrustConstants.Trust13.Namespace) ||
        //            child.Name == XName.Get("BinarySecret", WsTrustConstants.Trust14.Namespace))
        //        {
        //            var base64 = child.Value;
        //            var key = Convert.FromBase64String(base64);

        //            return new BinarySecretKeyInfo
        //            {
        //                Key = key
        //            };
        //        }

        //        if (child.Name == XName.Get("SecurityTokenReference", WsSecurityConstants.WsSecurity10.Namespace))
        //        {
        //            var keyIdentifier = child.Nodes().OfType<XElement>().FirstOrDefault(e => e.Name == XName.Get("KeyIdentifier", WsSecurityConstants.WsSecurity10.Namespace));
        //            var valueType = keyIdentifier?.Attribute(XName.Get("ValueType", string.Empty));
        //            var id = keyIdentifier?.Value;

        //            return new SecurityTokenReferenceKeyInfo
        //            {
        //                KeyIdValueType = valueType?.Value,
        //                KeyId = id
        //            };
        //        }
        //    }

        //    using (var temp = document.CreateReader())
        //    {
        //        temp.Settings.IgnoreWhitespace = true;
        //        temp.Read();
        //        return base.ReadKeyInfo(temp);
        //    }
        //}

        public override void WriteKeyInfo(XmlWriter writer, KeyInfo keyInfo)
        {
            if (keyInfo is EncryptedKeyInfo encrypted)
            {
                const string xmlenc = "http://www.w3.org/2001/04/xmlenc#";

                // <KeyInfo>
                writer.WriteStartElement(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace);
                writer.WriteStartElement("EncryptedKey", xmlenc);
                
                writer.WriteStartElement("EncryptionMethod", xmlenc);
                writer.WriteAttributeString("Algorithm", encrypted.EncryptionMethod);
                if(!string.IsNullOrEmpty(encrypted.DigestMethod))
                {
                    writer.WriteStartElement("DigestMethod", XmlSignatureConstants.Namespace);
                    writer.WriteAttributeString("Algorithm", encrypted.DigestMethod);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                WriteKeyInfo(writer, encrypted.KeyInfo);

                writer.WriteStartElement("CipherData", xmlenc);
                writer.WriteStartElement("CipherValue", xmlenc);
                writer.WriteString(Convert.ToBase64String(encrypted.CipherValue));
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndElement();
                return;
            }

            if (keyInfo is BinarySecretKeyInfo binary)
            {
                // <KeyInfo>
                writer.WriteStartElement(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace);
                var binarySecret = new BinarySecret(binary.Key);
                var xmlDictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
                var version = null as WsTrustVersion; 
                if (xmlDictionaryWriter.LookupPrefix(WsTrustConstants.TrustFeb2005.Namespace) != null)
                    version = WsTrustVersion.TrustFeb2005;
                else if (xmlDictionaryWriter.LookupPrefix(WsTrustConstants.Trust14.Namespace) != null)
                    version = WsTrustVersion.Trust14;
                else
                    version = WsTrustVersion.Trust13; // default to Trust 1.3
                WsTrustSerializer.WriteBinarySecret(xmlDictionaryWriter, new WsSerializationContext(version), binarySecret);
                writer.WriteEndElement();
                return;
            }

            // this might be a bit naive
            if (keyInfo is SecurityTokenReferenceKeyInfo securityTokenReference)
            {
                // <KeyInfo>
                writer.WriteStartElement(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace);
                writer.WriteStartElement(WsSecurityConstants.WsSecurity10.Prefix, "SecurityTokenReference", WsSecurityConstants.WsSecurity10.Namespace);
                writer.WriteStartElement(WsSecurityConstants.WsSecurity10.Prefix, "KeyIdentifier", WsSecurityConstants.WsSecurity10.Namespace);
                writer.WriteAttributeString("ValueType", securityTokenReference.KeyIdValueType);

                writer.WriteString(securityTokenReference.KeyId);

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
                return;
            }

            base.WriteKeyInfo(writer, keyInfo);
        }

        private void WriteXmlElement(XmlWriter writer, XmlElement element)
        {
            if (!string.IsNullOrEmpty(element.Prefix))
                writer.WriteStartElement(element.Prefix, element.LocalName, element.NamespaceURI);
            else if (!string.IsNullOrEmpty(element.NamespaceURI))
                writer.WriteStartElement(element.LocalName, element.NamespaceURI);
            else
                writer.WriteStartElement(element.LocalName);

            foreach (var attribute in element.Attributes.Cast<XmlAttribute>())
                WriteXmlAttribute(writer, attribute);

            foreach (var node in element.ChildNodes)
            {
                if (node is XmlElement child)
                    WriteXmlElement(writer, child);
                if (node is XmlText text)
                    writer.WriteValue(text.Value);
            }

            writer.WriteEndElement();
        }

        private void WriteXmlAttribute(XmlWriter writer, XmlAttribute attribute)
        {
            if (attribute.Prefix != null)
                writer.WriteAttributeString(attribute.Prefix, attribute.LocalName, attribute.NamespaceURI, attribute.Value);
            else if (attribute.NamespaceURI != null)
                writer.WriteAttributeString(attribute.LocalName, attribute.NamespaceURI, attribute.Value);
            else
                writer.WriteAttributeString(attribute.LocalName, attribute.Value);
        }

        private XmlElement CreateEncryptedSymmetricKeyInfo(byte[] key, EncryptingCredentials credentials, XmlDocument document)
        {
            var digestMethod = null as string;
            var clause = null as Legacy.KeyInfoClause;
            if (credentials.Key is X509SecurityKey x509)
            {
                digestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
                var securityTokenReference = document.CreateElement(WsSecurityConstants.WsSecurity10.Prefix, "SecurityTokenReference", WsSecurityConstants.WsSecurity10.Namespace);
                var keyIdentifier = document.CreateElement(WsSecurityConstants.WsSecurity10.Prefix, "KeyIdentifier", WsSecurityConstants.WsSecurity10.Namespace);
                keyIdentifier.SetAttribute("ValueType", "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#ThumbprintSHA1");
                keyIdentifier.InnerText = x509.X5t;
                securityTokenReference.AppendChild(keyIdentifier);

                using (var rsa = x509.Certificate.GetRSAPublicKey())
                    clause = EncryptKey(key, rsa, credentials.Alg, new Legacy.KeyInfoNode(securityTokenReference));
            }
            else if(credentials.Key is RsaSecurityKey rsa)
            {
                clause = EncryptKey(key, rsa.Rsa, credentials.Alg, CreateKeyInfoClause(rsa));
            }
            else
            {
                throw new NotSupportedException($"Encryption key type '{credentials?.Key.GetType()?.Name}' not supported.");
            }

            var keyInfo = new Legacy.KeyInfo();
            keyInfo.AddClause(clause);
            var element = keyInfo.GetXml();

            if (digestMethod != null)
            {
                var encryptionMethods = element
                    .SelectNodes("//*")
                    .OfType<XmlElement>()
                    .Where(e => e.LocalName == "EncryptionMethod")
                    .Where(e =>
                    {
                        var algorithm = e.GetAttribute("Algorithm");
                        return algorithm == Legacy.EncryptedXml.XmlEncRSAOAEPUrl;
                    })
                ;
                foreach (var encryptionMethod in encryptionMethods)
                {
                    var digest = element.OwnerDocument.CreateElement("DigestMethod", "http://www.w3.org/2000/09/xmldsig#");
                    digest.SetAttribute("Algorithm", digestMethod);
                    encryptionMethod.AppendChild(digest);
                }
            }

            return element;
        }

        private Legacy.KeyInfoClause CreateKeyInfoClause(RsaSecurityKey rsa)
        {
            var id = rsa.KeyId;
            if (!string.IsNullOrEmpty(id)) return new Legacy.KeyInfoName(id);
            if (rsa.CanComputeJwkThumbprint())
                id = Base64UrlEncoder.Encode(rsa.ComputeJwkThumbprint());
            return new Legacy.KeyInfoName(id);
        }

        private Legacy.KeyInfoEncryptedKey EncryptKey(byte[] key, RSA encryptionKey, string method, Legacy.KeyInfoClause encryptionKeyInfoClause)
        {
            var encryptionKeyInfo = new Legacy.KeyInfo();
            encryptionKeyInfo.AddClause(encryptionKeyInfoClause);
            var cipherValue = Legacy.EncryptedXml.EncryptKey(key, encryptionKey, true);
            var encryptedKey = new Legacy.EncryptedKey
            {
                CipherData = new Legacy.CipherData { CipherValue = cipherValue },
                EncryptionMethod = new Legacy.EncryptionMethod(method),
                KeyInfo = encryptionKeyInfo
            };

            return new Legacy.KeyInfoEncryptedKey { EncryptedKey = encryptedKey };
        }
    }
}
