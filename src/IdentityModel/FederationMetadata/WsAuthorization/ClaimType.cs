using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata.WsAuthorization
{
    public class ClaimType
    {
        public ClaimType(Uri uri) => Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        public Uri Uri { get; }
        public bool? Optional { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string DisplayValue { get; set; }
        public string Value { get; set; }

        // TODO: Add other elements in xsd:choice
    }
}
