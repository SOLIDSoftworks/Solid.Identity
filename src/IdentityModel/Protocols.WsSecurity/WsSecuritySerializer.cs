using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out KeyIdentifier entity)
            => TryReadKeyIdentifier(reader, CreateContext(reader), serializer, out entity);

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
            out KeyIdentifier entity)
            => TryReadKeyIdentifier(reader, context, serializer, out entity);

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out SecurityHeader entity)
            => TryReadSecurityHeader(reader, CreateContext(reader), serializer, out entity);

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
            out SecurityHeader entity)
            => TryReadSecurityHeader(reader, context, serializer, out entity);

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out SecurityTokenReference entity)
            => TryReadSecurityTokenReference(reader, CreateContext(reader), serializer, out entity);

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
            out SecurityTokenReference entity)
            => TryReadSecurityTokenReference(reader, context, serializer, out entity);
        
        protected virtual bool TryReadSecurityTokenReference(XmlDictionaryReader reader, WsSerializationContext context, WsSerializer serializer, out SecurityTokenReference reference)
        {
            if(reader.LocalName != WsSecurityElements.SecurityTokenReference || reader.NamespaceURI != context.Security.Namespace)
                return Out.False(out reference);

            var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
            var r = new SecurityTokenReference
            {
                Id = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace),
                // TokenType is always WS-Security 1.1
                TokenType = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityAttributes.TokenType, WsSecurityConstants.WsSecurity11.Namespace),
                Usage = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityAttributes.Usage, context.Security.Namespace)
            };

            reader.ReadStartElement();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                ReadSecurityTokenReferenceChildNode(reader, context, serializer, r);
            }
            reader.ReadEndElement();

            reference = r;
            return true;
        }

        protected virtual void ReadSecurityTokenReferenceChildNode(XmlDictionaryReader reader, WsSerializationContext context, WsSerializer serializer, SecurityTokenReference reference)
        {
            if (TryReadKeyIdentifier(reader, context, serializer, out var identifier))
                reference.KeyIdentifier = identifier;
            else
                reader.Skip();
        }

        protected virtual bool TryReadKeyIdentifier(XmlDictionaryReader reader, WsSerializationContext context, WsSerializer serializer, out KeyIdentifier identifier)
        {
            if(reader.LocalName != WsSecurityElements.KeyIdentifier || reader.NamespaceURI != context.Security.Namespace)
                return Out.False(out identifier);
            
            //  <wsse:KeyIdentifier wsu:Id="..."
            //                      ValueType="..."
            //                      EncodingType="...">
            //      ...
            //  </wsse:KeyIdentifier>

            var empty = reader.IsEmptyElement;
            var attributes = XmlAttributeDescriptor.ReadAttributes(reader);

            var keyIdentifier = new KeyIdentifier
            {
                Id = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace),
                ValueType = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityAttributes.ValueType, context.Security.Namespace)
            };

            reader.ReadStartElement();
            if (!empty)
            {
                keyIdentifier.Value = reader.ReadContentAsString();
                reader.ReadEndElement();
            }

            identifier = keyIdentifier;
            return true;
        }

        protected virtual bool TryReadSecurityHeader(XmlDictionaryReader reader, WsSerializationContext context, WsSerializer serializer, out SecurityHeader header)
        {
            //  <wsse:Security wsu:Id="...">
            //    <wsu:Timestamp>
            //      ...
            //    </wsu:Timestamp>
            //    ...
            //  </wsse:Security>
            
            if(reader.LocalName != WsSecurityElements.Security || reader.NamespaceURI != context.Security.Namespace)
                return Out.False(out header);
            
            var h = new SecurityHeader();
            foreach(var attribute in XmlAttributeDescriptor.ReadAttributes(reader))
                h.AdditionalXmlAttributes.Add(attribute);
            
            reader.ReadStartElement();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                ReadSecurityHeaderChildNode(reader, context, serializer, h);
            }

            reader.ReadEndElement();
            
            header = h;
            return true;
        }

        protected virtual void ReadSecurityHeaderChildNode(XmlDictionaryReader reader, WsSerializationContext context, WsSerializer serializer, SecurityHeader header)
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

        protected IProtocolSerializer<SecurityHeader> SecurityHeaderSerializer
            => this;

        protected IProtocolSerializer<SecurityTokenReference> SecurityTokenReferenceSerializer
            => this;

        protected IProtocolSerializer<KeyIdentifier> KeyIdentifierSerializer
            => this;

        private WsSerializationContext CreateContext(XmlDictionaryReader reader)
        {
            if (reader == null)
                throw LogHelper.LogArgumentNullException(nameof(reader));
            
            var name = reader.LocalName;
            if (!WsSecurityElements.All.Contains(name))
                throw new XmlException("Cannot create WS serialization context for " + name);

            var ns = reader.NamespaceURI;
            return CreateContext(ns);
        }

        private WsSerializationContext CreateContext(string ns)
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
        {
            var context = CreateContext(reader);
            return KeyIdentifierSerializer.ReadEntity(reader, serializer, context);
        }
        
        KeyIdentifier IProtocolSerializer<KeyIdentifier>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsSecurityElements.SecurityTokenReference, context);
            _ = TryReadKeyIdentifier(reader, context, serializer, out var identifier);
            return identifier;
        }
        
        void IProtocolSerializer<KeyIdentifier>.WriteEntity(XmlDictionaryWriter writer, KeyIdentifier entity, WsSerializer serializer,
            WsSerializationContext context)
        {
            //  <wsse:KeyIdentifier wsu:Id="..."
            //                      ValueType="..."
            //                      EncodingType="...">
            //      ...
            //  </wsse:KeyIdentifier>

            writer.WriteStartElement(context.Security.DefaultPrefix, WsSecurityElements.KeyIdentifier, context.Security.Namespace);

            if (!string.IsNullOrEmpty(entity.Id))
                writer.WriteAttributeString(WsSecurityUtilityAttributes.Id, entity.Id);

            if (!string.IsNullOrEmpty(entity.ValueType))
                writer.WriteAttributeString(WsSecurityAttributes.ValueType, entity.ValueType);

            if (!string.IsNullOrEmpty(entity.Value))
                writer.WriteString(entity.Value);

            writer.WriteEndElement();
        }

        SecurityHeader IProtocolSerializer<SecurityHeader>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
        {
            var context = CreateContext(reader);
            return SecurityHeaderSerializer.ReadEntity(reader, serializer, context);
        }

        SecurityHeader IProtocolSerializer<SecurityHeader>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsSecurityElements.Security, context);
            _ = TryReadSecurityHeader(reader, context, serializer, out var header);
            return header;
        }

        void IProtocolSerializer<SecurityHeader>.WriteEntity(XmlDictionaryWriter writer, SecurityHeader entity, WsSerializer serializer,
            WsSerializationContext context)
        {
            throw new NotImplementedException();
        }

        SecurityTokenReference IProtocolSerializer<SecurityTokenReference>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
        {
            var context = CreateContext(reader);
            return SecurityTokenReferenceSerializer.ReadEntity(reader, serializer, context);
        }

        SecurityTokenReference IProtocolSerializer<SecurityTokenReference>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsSecurityElements.SecurityTokenReference, context);
            _ = TryReadSecurityTokenReference(reader, context, serializer, out var reference);
            return reference;
        }

        void IProtocolSerializer<SecurityTokenReference>.WriteEntity(XmlDictionaryWriter writer, SecurityTokenReference entity, WsSerializer serializer, WsSerializationContext context)
        {
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
                writer.WriteAttributeString(WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace, entity.Id);

            if (entity.KeyIdentifier != null)
                KeyIdentifierSerializer.WriteEntity(writer, entity.KeyIdentifier, serializer, context);

            writer.WriteEndElement();
        }
    }
}
