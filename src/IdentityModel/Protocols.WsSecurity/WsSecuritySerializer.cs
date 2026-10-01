using System;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Xml;
using XmlException = Microsoft.IdentityModel.Xml.XmlException;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Base class for support of serializing versions of WS-Security.
    /// see: https://www.oasis-open.org/committees/download.php/16790/wss-v1.1-spec-os-SOAPMessageSecurity.pdf (1.1)
    /// see: http://docs.oasis-open.org/wss-m/wss/v1.1.1/os/wss-SOAPMessageSecurity-v1.1.1-os.html (1.1.1)
    /// </summary>
    public class WsSecuritySerializer : ProtocolSerializer, 
        IProtocolSerializer<SecurityTokenReference>, 
        IProtocolSerializer<SecurityHeader>,
        IProtocolSerializer<KeyIdentifier>
    {
        protected override string[] SupportedEntities => [WsSecurityElements.SecurityTokenReference, WsSecurityElements.Security, WsSecurityElements.KeyIdentifier];

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out KeyIdentifier entity)
        {
            if (!CanRead(reader, WsSecurityElements.KeyIdentifier))
                return Out.False(out entity);
            return TryReadKeyIdentifier(reader, serializer, CreateContext(reader), out entity);
        }

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
            out KeyIdentifier entity)
            => TryReadKeyIdentifier(reader, serializer, context, out entity);

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out SecurityHeader entity)
        {
            if (!CanRead(reader, WsSecurityElements.Security))
                return Out.False(out entity);
            return TryReadSecurityHeader(reader, serializer, CreateContext(reader), out entity);
        }

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
            out SecurityHeader entity)
            => TryReadSecurityHeader(reader, serializer, context, out entity);

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out SecurityTokenReference entity)
        {
            if (!CanRead(reader, WsSecurityElements.SecurityTokenReference))
                return Out.False(out entity);
            return TryReadSecurityTokenReference(reader, serializer, CreateContext(reader), out entity);
        }

        private static bool CanRead(XmlDictionaryReader reader, string element)
        {
            if (reader == null)
                throw LogHelper.LogArgumentNullException(nameof(reader));
            reader.MoveToContent();
            return reader.LocalName == element && WsSecurityConstants.KnownNamespaces.ContainsKey(reader.NamespaceURI);
        }

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
            out SecurityTokenReference entity)
            => TryReadSecurityTokenReference(reader, serializer, context, out entity);
        
        protected virtual bool TryReadSecurityTokenReference(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out SecurityTokenReference reference)
        {
            if(reader.LocalName != WsSecurityElements.SecurityTokenReference || reader.NamespaceURI != context.Security.Namespace)
                return Out.False(out reference);

            var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
            var r = CreateSecurityTokenReference(context, attributes);
            ReadNode(reader, serializer, context, r, ReadSecurityTokenReferenceChildNode);

            reference = r;
            return true;
        }

        protected virtual SecurityTokenReference CreateSecurityTokenReference(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
            => new SecurityTokenReference
            {
                Id = GetQualifiedAttribute(attributes, WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace),
                // WS-Security 1.1 defines TokenType in its namespace; older messages also use an unqualified attribute.
                TokenType = GetQualifiedAttribute(attributes, WsSecurityAttributes.TokenType, WsSecurityConstants.WsSecurity11.Namespace)
                    ?? GetQualifiedAttribute(attributes, WsSecurityAttributes.TokenType, string.Empty),
                Usage = GetQualifiedAttribute(attributes, WsSecurityAttributes.Usage, string.Empty)
            };

        private static string GetQualifiedAttribute(XmlAttributeDescriptor[] attributes, string name, string ns)
        {
            foreach (var attribute in attributes)
                if (attribute.LocalName == name && attribute.NamespaceUri == ns)
                    return attribute.Value;
            return null;
        }

        protected virtual void ReadSecurityTokenReferenceChildNode(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, SecurityTokenReference reference)
        {
            if (TryReadKeyIdentifier(reader, serializer, context, out var identifier))
                reference.KeyIdentifier = identifier;
            else
                reader.Skip();
        }

        protected virtual bool TryReadKeyIdentifier(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out KeyIdentifier identifier)
        {
            if(reader.LocalName != WsSecurityElements.KeyIdentifier || reader.NamespaceURI != context.Security.Namespace)
                return Out.False(out identifier);
            
            //  <wsse:KeyIdentifier wsu:Id="..."
            //                      ValueType="..."
            //                      EncodingType="...">
            //      ...
            //  </wsse:KeyIdentifier>

            var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
            var keyIdentifier = CreateKeyIdentifier(context, attributes);
            ReadNode(reader, keyIdentifier, ReadKeyIdentifierValue);

            identifier = keyIdentifier;
            return true;
        }

        protected virtual KeyIdentifier CreateKeyIdentifier(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
            => new KeyIdentifier
            {
                Id = GetQualifiedAttribute(attributes, WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace),
                ValueType = GetQualifiedAttribute(attributes, WsSecurityAttributes.ValueType, string.Empty),
                EncodingType = GetQualifiedAttribute(attributes, WsSecurityAttributes.EncodingType, string.Empty)
            };

        private void ReadKeyIdentifierValue(XmlDictionaryReader reader, KeyIdentifier entity)
            => entity.Value = reader.ReadString();

        protected virtual bool TryReadSecurityHeader(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out SecurityHeader header)
        {
            //  <wsse:Security wsu:Id="...">
            //    <wsu:Timestamp>
            //      ...
            //    </wsu:Timestamp>
            //    ...
            //  </wsse:Security>
            
            if(reader.LocalName != WsSecurityElements.Security || reader.NamespaceURI != context.Security.Namespace)
                return Out.False(out header);
            
            var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
            var h = CreateSecurityHeader(context, attributes);
            ReadNode(reader, serializer, context, h, ReadSecurityHeaderChildNode);
            
            header = h;
            return true;
        }

        protected virtual SecurityHeader CreateSecurityHeader(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
        {
            var header = new SecurityHeader();
            foreach (var attribute in attributes)
                header.AdditionalXmlAttributes.Add(attribute);
            return header;
        }

        protected virtual void ReadSecurityHeaderChildNode(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, SecurityHeader header)
        {
            if (serializer.TryReadEntity<Timestamp>(reader, context, out var timestamp))
                header.Timestamp = timestamp;
            else
                ReadAdditionalXmlElement(reader, header);
        }

        protected override WsProtocolConstants GetProtocolConstants(WsSerializationContext context)
        {
            var constants = context.Security;
            if (constants == null)
                // TODO: Create IDX error
                throw new ArgumentNullException(nameof(context.Security));
            return constants;
        }

        protected override WsSerializationContext CreateContext(string ns)
        {
            if(!WsSecurityConstants.KnownNamespaces.TryGetValue(ns, out var ws))
                // Create IDX error
                throw new XmlException("Cannot create WS serialization context for " + ns);
            
            return new WsSerializationContext
            {
                Security = ws,
                SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10,
            };
        }

        KeyIdentifier IProtocolSerializer<KeyIdentifier>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
            => ReadKeyIdentifier(reader, serializer, CreateContext(reader));
        
        KeyIdentifier IProtocolSerializer<KeyIdentifier>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
            => ReadKeyIdentifier(reader, serializer, context);

        private KeyIdentifier ReadKeyIdentifier(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsSecurityElements.KeyIdentifier, context);
            _ = TryReadKeyIdentifier(reader, serializer, context, out var identifier);
            return identifier;
        }
        
        void IProtocolSerializer<KeyIdentifier>.WriteEntity(XmlDictionaryWriter writer, KeyIdentifier entity, WsSerializer serializer,
            WsSerializationContext context)
            => WriteKeyIdentifier(writer, entity, serializer, context);

        protected virtual void WriteKeyIdentifier(XmlDictionaryWriter writer, KeyIdentifier entity, WsSerializer serializer, WsSerializationContext context)
        {
            WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));
            //  <wsse:KeyIdentifier wsu:Id="..."
            //                      ValueType="..."
            //                      EncodingType="...">
            //      ...
            //  </wsse:KeyIdentifier>

            writer.WriteStartElement(context.Security.DefaultPrefix, WsSecurityElements.KeyIdentifier, context.Security.Namespace);

            if (!string.IsNullOrEmpty(entity.Id))
                writer.WriteAttributeString(context.SecurityUtility.DefaultPrefix, WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace, entity.Id);

            if (!string.IsNullOrEmpty(entity.ValueType))
                writer.WriteAttributeString(WsSecurityAttributes.ValueType, entity.ValueType);

            if (!string.IsNullOrEmpty(entity.EncodingType))
                writer.WriteAttributeString(WsSecurityAttributes.EncodingType, entity.EncodingType);

            if (!string.IsNullOrEmpty(entity.Value))
                writer.WriteString(entity.Value);

            writer.WriteEndElement();
        }

        SecurityHeader IProtocolSerializer<SecurityHeader>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
            => ReadSecurityHeader(reader, serializer, CreateContext(reader));

        SecurityHeader IProtocolSerializer<SecurityHeader>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
            => ReadSecurityHeader(reader, serializer, context);

        private SecurityHeader ReadSecurityHeader(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsSecurityElements.Security, context);
            _ = TryReadSecurityHeader(reader, serializer, context, out var header);
            return header;
        }

        void IProtocolSerializer<SecurityHeader>.WriteEntity(XmlDictionaryWriter writer, SecurityHeader entity, WsSerializer serializer,
            WsSerializationContext context)
            => WriteSecurityHeader(writer, entity, serializer, context);

        protected virtual void WriteSecurityHeader(XmlDictionaryWriter writer, SecurityHeader entity, WsSerializer serializer, WsSerializationContext context)
        {
            WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));
            writer.WriteStartElement(context.Security.DefaultPrefix, WsSecurityElements.Security, context.Security.Namespace);
            WriteXmlOpenItemAttributes(writer, context, entity);
            if (entity.Timestamp != null)
                serializer.WriteEntity(writer, entity.Timestamp, context);
            WriteXmlOpenItemElements(writer, context, entity);
            writer.WriteEndElement();
        }

        SecurityTokenReference IProtocolSerializer<SecurityTokenReference>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
            => ReadSecurityTokenReference(reader, serializer, CreateContext(reader));

        SecurityTokenReference IProtocolSerializer<SecurityTokenReference>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
            => ReadSecurityTokenReference(reader, serializer, context);

        private SecurityTokenReference ReadSecurityTokenReference(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsSecurityElements.SecurityTokenReference, context);
            _ = TryReadSecurityTokenReference(reader, serializer, context, out var reference);
            return reference;
        }

        void IProtocolSerializer<SecurityTokenReference>.WriteEntity(XmlDictionaryWriter writer, SecurityTokenReference entity, WsSerializer serializer, WsSerializationContext context)
            => WriteSecurityTokenReference(writer, entity, serializer, context);

        protected virtual void WriteSecurityTokenReference(XmlDictionaryWriter writer, SecurityTokenReference entity, WsSerializer serializer, WsSerializationContext context)
        {
            WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));
            // <wsse:SecurityTokenReference>
            //      <wsse:KeyIdentifier wsu:Id="..."
            //                          ValueType="..."
            //                          EncodingType="...">
            //          ...
            //      </wsse:KeyIdentifier>
            //  </wsse:SecurityTokenReference>

            writer.WriteStartElement(context.Security.DefaultPrefix, WsSecurityElements.SecurityTokenReference, context.Security.Namespace);

            // For Saml2 tokens, the 'TokenType' was defined in must be in wsse1.1 namespace
            if (!string.IsNullOrEmpty(entity.TokenType))
                writer.WriteAttributeString(WsSecurityAttributes.TokenType, WsSecurityConstants.WsSecurity11.Namespace, entity.TokenType);

            if (!string.IsNullOrEmpty(entity.Id))
                writer.WriteAttributeString(context.SecurityUtility.DefaultPrefix, WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace, entity.Id);

            if (!string.IsNullOrEmpty(entity.Usage))
                writer.WriteAttributeString(WsSecurityAttributes.Usage, entity.Usage);

            if (entity.KeyIdentifier != null)
                WriteKeyIdentifier(writer, entity.KeyIdentifier, serializer, context);

            writer.WriteEndElement();
        }
    }
}
