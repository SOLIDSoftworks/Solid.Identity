using Solid.IdentityModel.FederationMetadata.WsAddressing;
using Solid.IdentityModel.FederationMetadata.WsAuthorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata
{
    public abstract class WebServiceDescriptor : Tokens.Saml2.Metadata.RoleDescriptor
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ICollection<Uri> IssuerNames { get; } = new List<Uri>();
        public ICollection<Uri> TokenTypesOffered { get; } = new List<Uri>();
        public ICollection<Uri> ClaimDialectsOffered { get; } = new List<Uri>();
        public ICollection<ClaimType> ClaimTypesOffered { get; } = new List<ClaimType>();
        public ICollection<ClaimType> ClaimTypesRequested { get; } = new List<ClaimType>();
        public bool? AutomaticPseudonyms { get; set; }
        public ICollection<EndpointReference> TargetScopes { get; } = new List<EndpointReference>();
        public  virtual string GetXmlTypeName()
        {
            var name = this.GetType().Name;
            return $"{name.Replace("Descriptor", string.Empty)}Type";
        }
    }
}
