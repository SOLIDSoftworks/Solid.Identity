using Solid.IdentityModel.FederationMetadata.WsAddressing;
using Solid.IdentityModel.Tokens.Saml2.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata
{
    public class ApplicationServiceDescriptor : WebServiceDescriptor
    {
        public ICollection<EndpointReferenceCollection> ApplicationServiceEndpoint { get; } = new List<EndpointReferenceCollection>();
        public ICollection<EndpointReferenceCollection> SingleSignOutNotificationEndpoint { get; } = new List<EndpointReferenceCollection>();
        public ICollection<EndpointReferenceCollection> PassiveRequestorEndpoint { get; } = new List<EndpointReferenceCollection>();
    }
}
