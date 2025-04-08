using Solid.IdentityModel.FederationMetadata.WsAddressing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata
{
    public class AttributeServiceDescriptor : WebServiceDescriptor
    {
        public ICollection<EndpointReferenceCollection> AttributeServiceEndpoint { get; } = new List<EndpointReferenceCollection>();
        public ICollection<EndpointReferenceCollection> SingleSignOutNotificationEndpoint { get; } = new List<EndpointReferenceCollection>();
    }
}
