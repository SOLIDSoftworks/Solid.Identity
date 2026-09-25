using System;
using System.Linq;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Solid.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Xml;
using XmlException = System.Xml.XmlException;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsAddressing
{
    public class WsAddressingSerializer : ProtocolSerializer, IProtocolSerializer<EndpointReference>
    {
        public WsAddressingSerializer()
        {
        }

        /// <summary>
        /// Reads an <see cref="EndpointReference"/>
        /// </summary>
        /// <param name="reader">The xml dictionary reader.</param>
        /// <returns>An <see cref="EndpointReference"/> instance.</returns>
        public virtual EndpointReference ReadEndpointReference(XmlDictionaryReader reader)
            => ReadEntity(reader, null);

        public EndpointReference ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
            => ReadEntity(reader, serializer, CreateContext(reader));

        public EndpointReference ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsAddressingElements.EndpointReference, context);
            _ = TryReadEntity(reader, serializer, context, out var reference);
            return reference;
        }

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out EndpointReference entity)
        {
            if (reader == null)
                throw LogHelper.LogArgumentNullException(nameof(reader));

            reader.MoveToContent();
            var ns = reader.NamespaceURI;
            if (!WsAddressingConstants.KnownNamespaces.TryGetValue(ns, out var constants))
                return Out.False(out entity);

            var context = new WsSerializationContext
            {
                Addressing = constants
            };
            return TryReadEntity(reader, serializer, context, out entity);
        }

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out EndpointReference entity)
        {
            if (reader == null)
                throw LogHelper.LogArgumentNullException(nameof(reader));
            if (context == null)
                throw LogHelper.LogArgumentNullException(nameof(context));
            if (context.Addressing == null)
                throw LogHelper.LogArgumentNullException(nameof(context.Addressing));

            reader.MoveToContent();
            if(reader.LocalName != WsAddressingElements.EndpointReference || reader.NamespaceURI != context.Addressing.Namespace)
                return Out.False(out entity);
            
            var empty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (empty)
                throw XmlUtil.LogReadException(LogMessages.IDX15011, context.Addressing.Namespace, WsAddressingElements.Address, reader.NamespaceURI, reader.LocalName);

            reader.MoveToContent();
            if (!reader.IsStartElement(WsAddressingElements.Address, context.Addressing.Namespace))
                throw XmlUtil.LogReadException(LogMessages.IDX15011, context.Addressing.Namespace, WsAddressingElements.Address, reader.NamespaceURI, reader.LocalName);

            var endpointReference = new EndpointReference(reader.ReadElementContentAsString());
            reader.MoveToContent();
            while (reader.NodeType == XmlNodeType.Element)
            {
                ReadAdditionalXmlElement(reader, endpointReference);
                reader.MoveToContent();
            }

            reader.ReadEndElement();

            entity = endpointReference;
            return true;
        }

        public void WriteEntity(XmlDictionaryWriter writer, EndpointReference entity, WsSerializer serializer, WsSerializationContext context)
        {
            WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));
            writer.WriteStartElement(context.Addressing.DefaultPrefix, WsAddressingElements.EndpointReference, context.Addressing.Namespace);
            
            WriteXmlOpenItemAttributes(writer, context, entity);
            
            writer.WriteStartElement(context.Addressing.DefaultPrefix, WsAddressingElements.Address, context.Addressing.Namespace);
            writer.WriteString(entity.Uri);
            writer.WriteEndElement();
            
            WriteXmlOpenItemElements(writer, context, entity);

            writer.WriteEndElement();
        }

        protected override WsProtocolConstants GetProtocolConstants(WsSerializationContext context)
            => context.Addressing;
        
        private WsSerializationContext CreateContext(XmlDictionaryReader reader)
        {
            if (reader == null)
                throw LogHelper.LogArgumentNullException(nameof(reader));

            reader.MoveToContent();
            var name = reader.LocalName;
            if (!WsAddressingElements.All.Contains(name))
                throw new XmlException("Cannot create WS serialization context for " + name);

            var ns = reader.NamespaceURI;
            return CreateContext(ns);
        }
        

        private WsSerializationContext CreateContext(string ns)
        {
            if(!WsAddressingConstants.KnownNamespaces.TryGetValue(ns, out var addressing))
                // Create IDX error
                throw new XmlException("Cannot create WS serialization context for " + ns);
            
            return new WsSerializationContext
            {
                Addressing = addressing,
                AddressingVersion = addressing.AddressingVersion
            };
        }
    }
}
