using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Solid.IdentityModel.Tokens.Saml2
{
    public class ExtendedSaml2Serializer : Saml2Serializer
    {
        private static IDictionary<string, string> _logMessages;
        static ExtendedSaml2Serializer()
        {
            var assembly = typeof(DSigSerializer).Assembly;
            var type = assembly.GetType("Microsoft.IdentityModel.Saml.LogMessages");
            var fields = type?.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField);
            _logMessages = fields?.Where(f => f.FieldType == typeof(string)).ToDictionary(f => f.Name, f => f.GetValue(null) as string);
        }
        public ExtendedSaml2Serializer()
            : this(new ExtendedDSigSerializer())
        {

        }

        public ExtendedSaml2Serializer(ExtendedDSigSerializer dsigSerializer)
        {
            DSigSerializer = dsigSerializer;
        }

        protected override Saml2SubjectConfirmationData ReadSubjectConfirmationData(XmlDictionaryReader reader)
        {
            XmlUtil.CheckReaderOnEntry(reader, Saml2Constants.Elements.SubjectConfirmationData, Saml2Constants.Namespace);
            try
            {
                var confirmationData = new Saml2SubjectConfirmationData();
                bool isEmpty = reader.IsEmptyElement;

                // @xsi:Type
                var requireKeyInfo = false;
                var attribute = reader.GetAttribute(EncryptedSaml2Constants.Elements.SubjectConfigurationData.Attributes.Type, XmlSignatureConstants.XmlSchemaNamespace);
                if (attribute != null)
                {
                    var type = XmlUtil.ResolveQName(reader, attribute);
                    if (type != null)
                    {
                        if (XmlUtil.EqualsQName(type, Saml2Constants.Types.KeyInfoConfirmationDataType, Saml2Constants.Namespace) ||
                            type.Name == Saml2Constants.Types.KeyInfoConfirmationDataType)
                            requireKeyInfo = true;
                        else if (!XmlUtil.EqualsQName(type, Saml2Constants.Types.SubjectConfirmationDataType,
                                     Saml2Constants.Namespace) &&
                                 type.Name != Saml2Constants.Types.SubjectConfirmationDataType)
                            throw XmlUtil.LogReadException(GetLogMessage("IDX13126"), type.Name, type.Namespace);
                    }
                }

                // KeyInfoConfirmationData cannot be empty
                if (requireKeyInfo && isEmpty)
                    throw XmlUtil.LogReadException(GetLogMessage("IDX13127"));

                // @Address - optional
                string value = reader.GetAttribute(Saml2Constants.Attributes.Address);
                if (!string.IsNullOrEmpty(value))
                    confirmationData.Address = value;

                // @InResponseTo - optional
                value = reader.GetAttribute(Saml2Constants.Attributes.InResponseTo);
                if (!string.IsNullOrEmpty(value))
                    confirmationData.InResponseTo = new Saml2Id(value);

                // @NotBefore - optional
                value = reader.GetAttribute(Saml2Constants.Attributes.NotBefore);
                if (!string.IsNullOrEmpty(value))
                    confirmationData.NotBefore = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);

                // @NotOnOrAfter - optional
                value = reader.GetAttribute(Saml2Constants.Attributes.NotOnOrAfter);
                if (!string.IsNullOrEmpty(value))
                    confirmationData.NotOnOrAfter = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);

                // @Recipient - optional
                value = reader.GetAttribute(Saml2Constants.Attributes.Recipient);
                if (!string.IsNullOrEmpty(value))
                {
                    if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                        throw XmlUtil.LogReadException(GetLogMessage("IDX13107"), Saml2Constants.Elements.SubjectConfirmationData, Saml2Constants.Attributes.Recipient, reader.LocalName);

                    confirmationData.Recipient = new Uri(value);
                }

                // Contents
                reader.Read();
                if (!isEmpty)
                {
                    while (reader.IsStartElement(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace))
                    {
                        confirmationData.KeyInfos.Add(DSigSerializer.ReadKeyInfo(reader));
                    }

                    // If this isn't KeyInfo restricted, there might be open content here ...
                    if (!requireKeyInfo && XmlNodeType.EndElement != reader.NodeType)
                    {
                        // So throw and tell the user how to handle the open content
                        throw XmlUtil.LogReadException(GetLogMessage("IDX13128"), Saml2Constants.Elements.SubjectConfirmationData);
                    }

                    reader.ReadEndElement();
                }

                return confirmationData;
            }
            catch (Exception ex)
            {
                if (ex is Saml2SecurityTokenReadException)
                    throw;

                throw XmlUtil.LogReadException(GetLogMessage("IDX13102"), ex, Saml2Constants.Elements.SubjectConfirmationData, ex);
            }
        }
        
        protected override void WriteSubjectConfirmationData(XmlWriter writer, Saml2SubjectConfirmationData subjectConfirmationData)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (subjectConfirmationData == null)
                throw new ArgumentNullException(nameof(subjectConfirmationData));

            // <SubjectConfirmationData>
            writer.WriteStartElement(Prefix, Saml2Constants.Elements.SubjectConfirmationData, Saml2Constants.Namespace);

            // @attributes

            // @xsi:Type
            if (subjectConfirmationData.KeyInfos.Count > 0)
                writer.WriteAttributeString(EncryptedSaml2Constants.Elements.SubjectConfigurationData.Attributes.Type, XmlSignatureConstants.XmlSchemaNamespace, Saml2Constants.Types.KeyInfoConfirmationDataType);

            // @Address - optional
            if (!string.IsNullOrEmpty(subjectConfirmationData.Address))
                writer.WriteAttributeString(Saml2Constants.Attributes.Address, subjectConfirmationData.Address);

            // @InResponseTo - optional
            if (null != subjectConfirmationData.InResponseTo)
                writer.WriteAttributeString(Saml2Constants.Attributes.InResponseTo, subjectConfirmationData.InResponseTo.Value);

            // @NotBefore - optional
            if (null != subjectConfirmationData.NotBefore)
                writer.WriteAttributeString(Saml2Constants.Attributes.NotBefore, XmlConvert.ToString(subjectConfirmationData.NotBefore.Value.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ss.fffZ"));

            // @NotOnOrAfter - optional
            if (null != subjectConfirmationData.NotOnOrAfter)
                writer.WriteAttributeString(Saml2Constants.Attributes.NotOnOrAfter, XmlConvert.ToString(subjectConfirmationData.NotOnOrAfter.Value.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ss.fffZ"));

            // @Recipient - optional
            if (null != subjectConfirmationData.Recipient)
                writer.WriteAttributeString(Saml2Constants.Attributes.Recipient, subjectConfirmationData.Recipient.OriginalString);

            // Content

            // <ds:KeyInfo> 0-OO
            foreach (var keyInfo in subjectConfirmationData.KeyInfos)
                base.DSigSerializer.WriteKeyInfo(writer, keyInfo);

            // </SubjectConfirmationData>
            writer.WriteEndElement();
        }

        public override void WriteAttribute(XmlWriter writer, Saml2Attribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.AttributeValueXsiType))
            {
                base.WriteAttribute(writer, attribute);
                return;
            }

            var dictionaryWriter = null as XmlDictionaryWriter;
            if (writer is XmlDictionaryWriter w)
                dictionaryWriter = w;
            else
                dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
            var xsi = "http://www.w3.org/2001/XMLSchema-instance";
            dictionaryWriter.WriteStartElement(Saml2Constants.Elements.Attribute, Saml2Constants.Namespace);
            dictionaryWriter.WriteAttributeString(Saml2Constants.Attributes.Name, attribute.Name);
            dictionaryWriter.WriteAttributeString(Saml2Constants.Attributes.NameFormat, attribute.NameFormat?.ToString() ?? Saml2Constants.NameIdentifierFormats.UnspecifiedString);
            foreach (var value in attribute.Values)
            {
                dictionaryWriter.WriteStartElement(Saml2Constants.Elements.AttributeValue, Saml2Constants.Namespace);

                var fqtn = attribute.AttributeValueXsiType?.Split('#');
                if (fqtn?.Length == 2)
                {
                    if (string.IsNullOrEmpty(dictionaryWriter.LookupPrefix("xs")))
                        dictionaryWriter.WriteAttributeString("xmlns", "xs", null, fqtn[0]);

                    dictionaryWriter.WriteStartAttribute("xsi", "type", xsi);
                    dictionaryWriter.WriteQualifiedName(fqtn[1], fqtn[0]);
                    dictionaryWriter.WriteEndAttribute();
                }

                dictionaryWriter.WriteString(value);
                dictionaryWriter.WriteEndElement();
            }
            dictionaryWriter.WriteEndElement();
        }

        private static string GetLogMessage(string id)
        {
            if (_logMessages.TryGetValue(id, out var message)) return message;
            return "Unexpected SAML2 error";
        }
    }
}
