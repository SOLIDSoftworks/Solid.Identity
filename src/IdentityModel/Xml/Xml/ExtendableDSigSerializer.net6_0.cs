#if NET6_0
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using static Microsoft.IdentityModel.Logging.LogHelper;

namespace Solid.IdentityModel.Xml
{
    // This class reflects changes that were included in a PR for Microsoft.IdentityModel.Xml, but were never merged in.
    // We decided to keep this seperate from the other DSigSerializer extensions if the PR would ever be merged with the main branch.
    public class ExtendableDSigSerializer : DSigSerializer
    {
        private static IDictionary<string, string> _logMessages;
        static ExtendableDSigSerializer()
        {
            var assembly = typeof(DSigSerializer).Assembly;
            var type = assembly.GetType("Microsoft.IdentityModel.Xml.LogMessages");
            var fields = type?.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField);
            _logMessages = fields?.Where(f => f.FieldType == typeof(string)).ToDictionary(f => f.Name, f => f.GetValue(null) as string);
        }

        /// <summary>
        /// Reads XML conforming to https://www.w3.org/TR/2001/PR-xmldsig-core-20010820/#sec-KeyInfo
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/> pointing positioned on a &lt;KeyInfo> element.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="reader"/> is null.</exception>
        /// <exception cref="XmlReadException">if there is a problem reading the XML.</exception>
        /// <remarks>Only handles IssuerSerial, Ski, SubjectName, Certificate. Unsupported types are skipped. Only a X509 data element is supported.</remarks>
        public override KeyInfo ReadKeyInfo(XmlReader reader)
        {
            XmlUtil.CheckReaderOnEntry(reader, XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace);

            var keyInfo = CreateKeyInfo(reader);

            try
            {
                bool isEmptyElement = reader.IsEmptyElement;

                // <KeyInfo>
                reader.ReadStartElement();
                while (reader.IsStartElement())
                {
                    if (!TryReadKeyInfoType(reader, ref keyInfo))
                    {
                        // Skip the element since it is not one of elements handled by TryReadKeyInfoType                        
                        LogHelper.LogWarning(GetLogMessage("IDX30300"), reader.ReadOuterXml());
                    }
                }

                // </KeyInfo>
                if (!isEmptyElement)
                    reader.ReadEndElement();

            }
            catch (Exception ex)
            {
                if (ex is XmlReadException)
                    throw;

                throw XmlUtil.LogReadException(GetLogMessage("IDX30017"), ex, XmlSignatureConstants.Elements.KeyInfo, ex);
            }

            return keyInfo;
        }


        /// <summary>
        /// Creates a new <see cref="KeyInfo"/> object.
        /// </summary>
        /// <param name="reader">The current <see cref="XmlReader"/>.</param>
        protected virtual KeyInfo CreateKeyInfo(XmlReader reader)
        {
            XmlUtil.CheckReaderOnEntry(reader, XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace);

            return new KeyInfo
            {
                Prefix = reader.Prefix
            };
        }

        /// <summary>
        /// Attempts to read the key info type which is the child of the &lt;KeyInfo> element.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> positioned on a child of a <see cref="XmlSignatureConstants.Elements.KeyInfo"/> element.</param>
        /// <param name="keyInfo">The <see cref="KeyInfo"/> object to populate.</param>
        protected virtual bool TryReadKeyInfoType(XmlReader reader, ref KeyInfo keyInfo)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            if (keyInfo == null)
                throw LogArgumentNullException(nameof(keyInfo));

            // <X509Data>
            if (TryReadX509Data(reader, out var x509Data))
                keyInfo.X509Data.Add(x509Data);
            // <RetrievalMethod>
            else if (TryReadRetrievalMethod(reader, out var retreivalMethodUri))
                keyInfo.RetrievalMethodUri = retreivalMethodUri;
            // <KeyName>
            else if (TryReadKeyName(reader, out var keyName))
                keyInfo.KeyName = keyName;
            // <KeyValue>
            else if (reader.IsStartElement(XmlSignatureConstants.Elements.KeyValue, XmlSignatureConstants.Namespace))
            {
                reader.ReadStartElement(XmlSignatureConstants.Elements.KeyValue, XmlSignatureConstants.Namespace);
                if (!TryReadKeyValueType(reader, ref keyInfo)) return false;

                // </KeyValue>
                reader.ReadEndElement();
            }
            else
                return false;

            return true;
        }


        /// <summary>
        /// Attempts to read the <see cref="XmlSignatureConstants.Elements.KeyValue"/> element conforming to https://www.w3.org/TR/2001/PR-xmldsig-core-20010820/#sec-KeyValue.
        /// <para>Only supports <see cref="RSAKeyValue"/>, but can be extended to support more key value types.</para>
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> positioned on a child of <see cref="XmlSignatureConstants.Elements.KeyValue"/> element.</param>
        /// <param name="keyInfo">The <see cref="KeyInfo"/> object to populate.</param>
        protected virtual bool TryReadKeyValueType(XmlReader reader, ref KeyInfo keyInfo)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            if (keyInfo == null)
                throw LogArgumentNullException(nameof(keyInfo));

            if (TryReadRSAKeyValue(reader, out var rsaKeyValue))
            {
                if (keyInfo.RSAKeyValue != null)
                    throw XmlUtil.LogReadException(GetLogMessage("IDX30015"), XmlSignatureConstants.Elements.RSAKeyValue);
                keyInfo.RSAKeyValue = rsaKeyValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to read the <see cref="XmlSignatureConstants.Elements.RSAKeyValue"/> element conforming to https://www.w3.org/TR/2001/PR-xmldsig-core-20010820/#sec-RSAKeyValue.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> positioned on a <see cref="XmlSignatureConstants.Elements.RSAKeyValue"/> element.</param>
        /// <param name="value">The parsed <see cref="XmlSignatureConstants.Elements.RSAKeyValue"/> element.</param>
        protected virtual bool TryReadRSAKeyValue(XmlReader reader, out RSAKeyValue value)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.RSAKeyValue, XmlSignatureConstants.Namespace))
            {
                value = null;
                return false;
            }

            reader.ReadStartElement(XmlSignatureConstants.Elements.RSAKeyValue, XmlSignatureConstants.Namespace);

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.Modulus, XmlSignatureConstants.Namespace))
                throw XmlUtil.LogReadException(GetLogMessage("IDX30011"), XmlSignatureConstants.Namespace, XmlSignatureConstants.Elements.Modulus, reader.NamespaceURI, reader.LocalName);

            var modulus = reader.ReadElementContentAsString(XmlSignatureConstants.Elements.Modulus, XmlSignatureConstants.Namespace);

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.Exponent, XmlSignatureConstants.Namespace))
                throw XmlUtil.LogReadException(GetLogMessage("IDX30011"), XmlSignatureConstants.Namespace, XmlSignatureConstants.Elements.Exponent, reader.NamespaceURI, reader.LocalName);

            var exponent = reader.ReadElementContentAsString(XmlSignatureConstants.Elements.Exponent, XmlSignatureConstants.Namespace);

            reader.ReadEndElement();

            value = new RSAKeyValue(modulus, exponent);

            return true;
        }


        /// <summary>
        /// Attempts to read the <see cref="XmlSignatureConstants.Elements.KeyName"/> element conforming to https://www.w3.org/TR/2001/PR-xmldsig-core-20010820/#sec-KeyName.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> positioned on a <see cref="XmlSignatureConstants.Elements.KeyName"/> element.</param>
        /// <param name="name">The parsed <see cref="XmlSignatureConstants.Elements.KeyName"/> element.</param>
        protected virtual bool TryReadKeyName(XmlReader reader, out string name)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.KeyName, XmlSignatureConstants.Namespace))
            {
                name = null;
                return false;
            }
            name = reader.ReadElementContentAsString(XmlSignatureConstants.Elements.KeyName, XmlSignatureConstants.Namespace);
            return true;
        }

        /// <summary>
        /// Attempts to read the <see cref="XmlSignatureConstants.Elements.RetrievalMethod"/> element conforming to https://www.w3.org/TR/2001/PR-xmldsig-core-20010820/#sec-RetrievalMethod.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> positioned on a <see cref="XmlSignatureConstants.Elements.RetrievalMethod"/> element.</param>
        /// <param name="method">The parsed <see cref="XmlSignatureConstants.Elements.RetrievalMethod"/> element.</param>
        protected virtual bool TryReadRetrievalMethod(XmlReader reader, out string method)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.RetrievalMethod, XmlSignatureConstants.Namespace))
            {
                method = null;
                return false;
            }
            method = reader.GetAttribute(XmlSignatureConstants.Attributes.URI);
            _ = reader.ReadOuterXml();
            return true;
        }

        /// <summary>
        /// Attempts to read the <see cref="XmlSignatureConstants.Elements.X509Data"/> element conforming to https://www.w3.org/TR/2001/PR-xmldsig-core-20010820/#sec-X509Data.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> positioned on a <see cref="XmlSignatureConstants.Elements.X509Data"/> element.</param>
        /// <param name="data">The parsed <see cref="XmlSignatureConstants.Elements.X509Data"/> element.</param>
        protected virtual bool TryReadX509Data(XmlReader reader, out X509Data data)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.X509Data, XmlSignatureConstants.Namespace))
            {
                data = null;
                return false;
            }

            data = new X509Data();

            if (reader.IsEmptyElement)
                throw XmlUtil.LogReadException(GetLogMessage("IDX30108"));

            reader.ReadStartElement(XmlSignatureConstants.Elements.X509Data, XmlSignatureConstants.Namespace);
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement(XmlSignatureConstants.Elements.X509Certificate, XmlSignatureConstants.Namespace))
                {
                    data.Certificates.Add(reader.ReadElementContentAsString());
                }
                else if (reader.IsStartElement(XmlSignatureConstants.Elements.X509IssuerSerial, XmlSignatureConstants.Namespace))
                {
                    if (data.IssuerSerial != null)
                        throw XmlUtil.LogReadException(GetLogMessage("IDX30015"), XmlSignatureConstants.Elements.X509IssuerSerial);
                    data.IssuerSerial = ReadIssuerSerial(reader);
                }
                else if (reader.IsStartElement(XmlSignatureConstants.Elements.X509SKI, XmlSignatureConstants.Namespace))
                {
                    if (data.SKI != null)
                        throw XmlUtil.LogReadException(GetLogMessage("IDX30015"), XmlSignatureConstants.Elements.X509SKI);
                    data.SKI = reader.ReadElementContentAsString();
                }
                else if (reader.IsStartElement(XmlSignatureConstants.Elements.X509SubjectName, XmlSignatureConstants.Namespace))
                {
                    if (data.SubjectName != null)
                        throw XmlUtil.LogReadException(GetLogMessage("IDX30015"), XmlSignatureConstants.Elements.X509SubjectName);
                    data.SubjectName = reader.ReadElementContentAsString();
                }
                else if (reader.IsStartElement(XmlSignatureConstants.Elements.X509CRL, XmlSignatureConstants.Namespace))
                {
                    if (data.CRL != null)
                        throw XmlUtil.LogReadException(GetLogMessage("IDX30015"), XmlSignatureConstants.Elements.X509CRL);
                    data.CRL = reader.ReadElementContentAsString();
                }
                else
                {
                    // Skip the element since it is not one of  <X509Certificate>, <X509IssuerSerial>, <X509SKI>, <X509SubjectName>, <X509CRL>
                    LogHelper.LogWarning(GetLogMessage("IDX30300"), reader.ReadOuterXml());
                }
            }

            // </X509Data>
            reader.ReadEndElement();

            return true;
        }

        /// <summary>
        /// Reads the "X509IssuerSerial" element conforming to https://www.w3.org/TR/2001/PR-xmldsig-core-20010820/#sec-X509Data.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> positioned on a <see cref="XmlSignatureConstants.Elements.X509IssuerSerial"/> element.</param>
        private static IssuerSerial ReadIssuerSerial(XmlReader reader)
        {
            reader.ReadStartElement(XmlSignatureConstants.Elements.X509IssuerSerial, XmlSignatureConstants.Namespace);

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.X509IssuerName, XmlSignatureConstants.Namespace))
                throw XmlUtil.LogReadException(GetLogMessage("IDX30011"), XmlSignatureConstants.Namespace, XmlSignatureConstants.Elements.X509IssuerName, reader.NamespaceURI, reader.LocalName);

            var issuerName = reader.ReadElementContentAsString(XmlSignatureConstants.Elements.X509IssuerName, XmlSignatureConstants.Namespace);

            if (!reader.IsStartElement(XmlSignatureConstants.Elements.X509SerialNumber, XmlSignatureConstants.Namespace))
                throw XmlUtil.LogReadException(GetLogMessage("IDX30011"), XmlSignatureConstants.Namespace, XmlSignatureConstants.Elements.X509SerialNumber, reader.NamespaceURI, reader.LocalName);

            var serialNumber = reader.ReadElementContentAsString(XmlSignatureConstants.Elements.X509SerialNumber, XmlSignatureConstants.Namespace);

            reader.ReadEndElement();

            return new IssuerSerial(issuerName, serialNumber);
        }

        private static string GetLogMessage(string id)
        {
            if (_logMessages.TryGetValue(id, out var message)) return message;
            return "Unexpected XML error";
        }
    }
}
#endif