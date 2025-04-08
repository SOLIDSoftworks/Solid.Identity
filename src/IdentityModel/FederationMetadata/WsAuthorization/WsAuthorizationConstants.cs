using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata.WsAuthorization
{
    public static class WsAuthorizationConstants
    {
        public static readonly string Namespace = "http://docs.oasis-open.org/wsfed/authorization/200706";
        public static class Elements
        {
            public static readonly string ClaimType = nameof(ClaimType);
            public static readonly string DisplayName = nameof(DisplayName);
            public static readonly string Description = nameof(Description);
            public static readonly string DisplayValue = nameof(DisplayValue);
            public static readonly string Value = nameof(Value);
            //public static readonly string EncryptedValue = nameof(EncryptedValue);
            //public static readonly string StructuredValue = nameof(StructuredValue);
            //public static readonly string ConstrainedValue = nameof(ConstrainedValue);
        }

        public static class Attributes
        {
            public static readonly string Uri = nameof(Uri);
            public static readonly string Optional = nameof(Optional);
        }
    }
}
