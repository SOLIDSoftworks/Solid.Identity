using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.IdentityModel.Logging;

#pragma warning disable 1591
namespace Solid.IdentityModel.Protocols.WsAddressing
{
    public class EndpointReference
    {
        public EndpointReference(string uri)
        {
            if (uri == null)
                throw LogHelper.LogArgumentNullException(nameof(uri));

            if (!System.Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                throw LogHelper.LogExceptionMessage(new ArgumentException(LogHelper.FormatInvariant($"uri is not absolute: {uri}")));

            Uri = uri;
            AdditionalXmlElements = new Collection<XmlElement>();
        }

        public ICollection<XmlElement> AdditionalXmlElements { get; }

        public string Uri {get;}
    }
}
