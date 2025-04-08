using Solid.IdentityModel.FederationMetadata.WsAddressing;
using Solid.IdentityModel.Tokens.Saml2.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata
{
    public class SecurityTokenServiceDescriptor : WebServiceDescriptor
    {
        public ICollection<EndpointReferenceCollection> SecurityTokenServiceEndpoint { get; } = new List<EndpointReferenceCollection>();
        public ICollection<EndpointReferenceCollection> SingleSignOutSubscriptionEndpoint { get; } = new List<EndpointReferenceCollection>();
        public ICollection<EndpointReferenceCollection> SingleSignOutNotificationEndpoint { get; } = new List<EndpointReferenceCollection>();
        public ICollection<EndpointReferenceCollection> PassiveRequestorEndpoint { get; } = new List<EndpointReferenceCollection>();
    }
}
