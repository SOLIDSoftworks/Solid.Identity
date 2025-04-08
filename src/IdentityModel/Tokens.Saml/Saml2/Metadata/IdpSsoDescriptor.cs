using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{

    public class IdpSsoDescriptor : SsoDescriptor
    {
        public ICollection<Endpoint> SingleSignOnService { get; } = new List<Endpoint>();
        public ICollection<Endpoint> NameIdMappingService { get; } = new List<Endpoint>();
        public ICollection<Endpoint> AssertionIdRequestService { get; } = new List<Endpoint>();
        public ICollection<Uri> AttributeProfile { get; } = new List<Uri>();
        public ICollection<Saml2Attribute> Attributes { get; } = new List<Saml2Attribute>();
        public bool? WantAuthnRequestsSigned { get; set; }
    }
}
