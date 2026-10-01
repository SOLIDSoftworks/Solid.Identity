using System;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Xml;
using XmlException = System.Xml.XmlException;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsAddressing
{
    public class WsAddressingSerializer : ProtocolSerializer, IProtocolSerializer<EndpointReference>
    {
        /// <summary>
        /// Reads an <see cref="EndpointReference"/>
        /// </summary>
        /// <param name="reader">The xml dictionary reader.</param>
        /// <returns>An <see cref="EndpointReference"/> instance.</returns>
        public virtual EndpointReference ReadEndpointReference(XmlDictionaryReader reader)
            => ReadEntity(reader, null);

        protected override string[] SupportedEntities => [WsAddressingElements.EndpointReference];

        public EndpointReference ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
            => ReadEndpointReference(reader, serializer, CreateContext(reader));

        public EndpointReference ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
            => ReadEndpointReference(reader, serializer, context);

        private EndpointReference ReadEndpointReference(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsAddressingElements.EndpointReference, context);
            _ = TryReadEndpointReference(reader, serializer, context, out var reference);
            return reference;
        }

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out EndpointReference entity)
        {
            if (reader == null)
                throw LogHelper.LogArgumentNullException(nameof(reader));
            reader.MoveToContent();
            if (reader.LocalName != WsAddressingElements.EndpointReference || !WsAddressingConstants.KnownNamespaces.ContainsKey(reader.NamespaceURI))
                return Out.False(out entity);
            return TryReadEndpointReference(reader, serializer, CreateContext(reader), out entity);
        }

        public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out EndpointReference entity)
            => TryReadEndpointReference(reader, serializer, context, out entity);

        protected virtual bool TryReadEndpointReference(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out EndpointReference entity)
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
            
            var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
            var result = CreateEndpointReference(context, attributes);
            ReadNode<EndpointReference, EndpointReferenceReadState>(reader, serializer, context, result, ReadEndpointReferenceChildNode);

            if (string.IsNullOrEmpty(result.Uri))
                throw XmlUtil.LogReadException(LogMessages.IDX15011, context.Addressing.Namespace, WsAddressingElements.Address, reader.NamespaceURI, reader.LocalName);

            entity = result;
            return true;
        }

        public void WriteEntity(XmlDictionaryWriter writer, EndpointReference entity, WsSerializer serializer, WsSerializationContext context)
            => WriteEndpointReference(writer, entity, serializer, context);

        protected virtual void WriteEndpointReference(XmlDictionaryWriter writer, EndpointReference entity, WsSerializer serializer, WsSerializationContext context)
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

        protected virtual EndpointReference CreateEndpointReference(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
        {
            var reference = new EndpointReference();
            foreach (var attribute in attributes)
                reference.AdditionalXmlAttributes.Add(attribute);
            return reference;
        }

        protected virtual void ReadEndpointReferenceChildNode(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, EndpointReference reference, EndpointReferenceReadState state)
        {
            if (reader.LocalName == WsAddressingElements.Address && reader.NamespaceURI == context.Addressing.Namespace)
            {
                if (state.FirstElementRead)
                    throw new XmlException("The WS-Addressing Address element must occur exactly once as the first child of EndpointReference.");

                var uri = reader.ReadElementContentAsString();
                if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                    throw LogHelper.LogExceptionMessage(new ArgumentException(LogHelper.FormatInvariant($"uri is not absolute: {uri}")));
                
                reference.Uri = uri;
            }
            else
                ReadAdditionalXmlElement(reader, reference);

            state.FirstElementRead = true;
        }

        protected override WsSerializationContext CreateContext(string ns)
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
    
    public class EndpointReferenceReadState
    {
        public bool FirstElementRead { get; set; }
    }
}
