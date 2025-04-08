using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsPolicy;
using Solid.IdentityModel.Protocols.WsSecurity;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Solid.IdentityModel.Protocols.WsTrust.Tests
{
    public static class WsDefaults
    {
        public static AppliesTo AppliesTo => new AppliesTo(new EndpointReference("https://www.identitymodel.com"));

        public static KeyIdentifier KeyIdentifier => new KeyIdentifier {
            EncodingType = WsSecurityEncodingTypes.WsSecurity11.Base64,
            Id = "_A29E5F33-E108-4F85-8786-DDA9142B64BC",
            ValueType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLID"
        };

        public static string KeyType => "http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey";

        public static SecurityTokenReference SecurityTokenReference => new SecurityTokenReference {
            KeyIdentifier = KeyIdentifier,
            TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0",

        };
    }
}

#pragma warning restore CS3016 // Arrays as attribute arguments is nEndoot CLS-compliant
