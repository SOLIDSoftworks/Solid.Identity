using System;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class Endpoint
    {
        public Uri Binding { get; set; }
        public Uri Location { get; set; }
        public Uri ResponseLocation { get; set; }
    }
}
