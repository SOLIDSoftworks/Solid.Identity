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
        {
            XmlUtil.CheckReaderOnEntry(reader, WsAddressingElements.EndpointReference);
            var ns = reader.NamespaceURI;
            if (!WsAddressingConstants.KnownNamespaces.TryGetValue(ns, out var constants))
                throw new InvalidOperationException("Unknown namespace: " + ns);

            var context = new WsSerializationContext
            {
                Addressing = constants
            };
            
            
            foreach (string @namespace in WsAddressingConstants.KnownNamespaces)
            {
                if (reader.IsNamespaceUri(@namespace))
                {
                    bool isEmptyElement = reader.IsEmptyElement;
                    reader.ReadStartElement();
                    var endpointReference = new EndpointReference(reader.ReadElementContentAsString());
                    while (reader.IsStartElement())
                    {
                        bool isInnerEmptyElement = reader.IsEmptyElement;
                        XmlReader subtreeReader = reader.ReadSubtree();
                        var doc = new XmlDocument
                        {
                            PreserveWhitespace = true
                        };

                        doc.Load(subtreeReader);
                        endpointReference.AdditionalXmlElements.Add(doc.DocumentElement);
                        if (!isInnerEmptyElement)
                            reader.ReadEndElement();
                    }

                    if (!isEmptyElement)
                        reader.ReadEndElement();

                    return endpointReference;
                }
            }

            throw LogHelper.LogExceptionMessage(new XmlReadException(LogHelper.FormatInvariant(LogMessages.IDX15001, WsAddressingElements.EndpointReference, WsAddressingConstants.Addressing200408.Namespace, WsAddressingConstants.Addressing10.Namespace, reader.NamespaceURI)));
        }

        public EndpointReference ReadEndpointReference(XmlDictionaryReader reader, WsSerializer serializer)
        {
            
            AssertReader(reader, WsAddressingElements.EndpointReference);
            _ = TryReadEntity(reader, serializer, context, out var reference);
            return reference;
        }

        public EndpointReference ReadEndpointReference(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        {
            AssertReader(reader, WsAddressingElements.EndpointReference, context);
            _ = TryReadEntity(reader, serializer, context, out var reference);
            return reference;
        }

        public bool TryReadEndpointReference(XmlDictionaryReader reader, WsSerializer serializer, out EndpointReference entity)
        {
            var ns = reader.NamespaceURI;
            if (!WsAddressingConstants.KnownNamespaces.TryGetValue(ns, out var constants))
                return Out.False(out entity);

            var context = new WsSerializationContext
            {
                Addressing = constants
            };
            return TryReadEndpointReference(reader, serializer, context, out entity);
        }

        public bool TryReadEndpointReference(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out EndpointReference entity)
        {
            if(reader.LocalName != WsAddressingElements.EndpointReference || reader.NamespaceURI != context.Addressing.Namespace)
                return Out.False(out entity);
            
            var empty = reader.IsEmptyElement;
            reader.ReadStartElement();
            var endpointReference = new EndpointReference(reader.ReadElementContentAsString());
            
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }
                ReadAdditionalXmlElement(reader, endpointReference);
            }
            
            if (!empty)
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
