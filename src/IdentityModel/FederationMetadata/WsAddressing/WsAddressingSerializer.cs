using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using static Solid.IdentityModel.FederationMetadata.WsAddressing.WsAddressingConstants;

namespace Solid.IdentityModel.FederationMetadata.WsAddressing
{
    public class WsAddressingSerializer
    {
        public virtual bool TryReadEndpointReferenceCollection(XmlDictionaryReader reader, string name, string ns, out EndpointReferenceCollection endpointReferenceCollection)
        {
            if (!reader.IsStartElement(name, ns)) return Out.False(out endpointReferenceCollection);
            var collection = new EndpointReferenceCollection();
            reader.ForEachChild(r =>
            {
                if (TryReadEndpointReference(r, out var endpointReference))
                {
                    collection.Add(endpointReference);
                    return true;
                }
                return false;
            });

            endpointReferenceCollection = collection;
            return true;
        }

        public virtual bool TryReadEndpointReference(XmlDictionaryReader reader, out EndpointReference endpointReference)
        {
            if (!reader.IsStartElement(Elements.EndpointReference, Namespace)) return Out.False(out endpointReference);

            var endpoint = new EndpointReference();
            ReadEndpointReferenceAttributes(reader, endpoint);
            reader.ForEachChild(r => TryReadEndpointReferenceChild(r, endpoint));

            endpointReference = endpoint;
            return true;
        }

        public virtual void WriteEndpointReferenceCollection(XmlDictionaryWriter writer, string prefix, string name, string ns, ICollection<EndpointReference> endpointReferenceCollection)
        {
            if (endpointReferenceCollection == null) return;
            if (endpointReferenceCollection.Count == 0) return;

            if (!string.IsNullOrEmpty(prefix)) writer.WriteStartElement(prefix, name, ns);
            else writer.WriteStartElement(name, ns);

            foreach (var endpoint in endpointReferenceCollection)
                WriteEndpointReference(writer, endpoint);

            writer.WriteEndElement();
        }

        public virtual void WriteEndpointReferenceCollection(XmlDictionaryWriter writer, string name, string ns, ICollection<EndpointReference> endpointReferenceCollection)
        {
            var prefix = writer.LookupPrefix(ns);
            WriteEndpointReferenceCollection(writer, prefix, name, ns, endpointReferenceCollection);
        }

        public virtual void WriteEndpointReference(XmlDictionaryWriter writer, EndpointReference endpointReference)
        {
            if (endpointReference == null) return;

            writer.WriteStartElement(Elements.EndpointReference, Namespace);
            if (!writer.TryWriteElementValue(Elements.Address, Namespace, endpointReference.Address))
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.EndpointReference, Elements.Address);

            writer.WriteEndElement();
        }

        protected virtual void ReadEndpointReferenceAttributes(XmlDictionaryReader reader, EndpointReference endpointReference)
        {
        }

        protected virtual bool TryReadEndpointReferenceChild(XmlDictionaryReader reader, EndpointReference endpointReference)
        {
            if (TryReadAddress(reader, out var address))
            {
                if (address != null)
                    endpointReference.Address = address;
            }
            else
                return false;

            return true;
        }

        protected virtual bool TryReadAddress(XmlDictionaryReader reader, out Uri address)
        {
            if(!reader.TryReadElementContent(Elements.Address, Namespace, out var content)) return Out.False(out address);

            if (!Uri.IsWellFormedUriString(content, UriKind.Absolute)) return Out.True(null, out address);

            address = new Uri(content, UriKind.Absolute);
            return true;
        }
    }
}
