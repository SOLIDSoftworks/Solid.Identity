#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsFederation
{
    public class WsFederationConstants : WsProtocolConstants
    {
        public static WsFederationConstants Federation12 { get; } = new WsFederation12();
        public static IDictionary<string, WsFederationConstants> KnownNamespaces { get; } = new Dictionary<string, WsFederationConstants>
        {
            { Federation12.Namespace, Federation12 }
        };


        public string SchemaLocation { get; init; }
        public WsFederationPrivacyConstants Privacy { get; init; }
        public WsFederationAuthorizationConstants Authorization { get; init; }
    }

    public class WsFederationPrivacyConstants : WsProtocolConstants
    {
        public static WsFederationPrivacyConstants FederationPrivacy12 { get; } = new WsFederationPrivacy12();
        
        public static IDictionary<string, WsFederationPrivacyConstants> KnownNamespaces { get; } = new Dictionary<string, WsFederationPrivacyConstants>
        {
            { FederationPrivacy12.Namespace, FederationPrivacy12 }
        };
        public string SchemaLocation { get; init; }
    }

    public class WsFederationAuthorizationConstants : WsProtocolConstants
    {
        public static WsFederationAuthorizationConstants FederationAuthorization12 { get; } =  new WsFederationAuthorization12();
        
        public static IDictionary<string, WsFederationAuthorizationConstants> KnownNamespaces { get; } = new Dictionary<string, WsFederationAuthorizationConstants>
        {
            { FederationAuthorization12.Namespace, FederationAuthorization12 }
        };

        public string SchemaLocation { get; init; }
    }

    internal class WsFederation12 : WsFederationConstants
    {
        public WsFederation12() 
        {
            Namespace = "http://docs.oasis-open.org/wsfed/federation/200706";
            DefaultPrefix = "fed";
            SchemaLocation = "http://docs.oasis-open.org/wsfed/federation/v1.2/federation.xsd";
            Privacy = WsFederationPrivacyConstants.FederationPrivacy12;
            Authorization = WsFederationAuthorizationConstants.FederationAuthorization12;
        }
    }

    internal class WsFederationAuthorization12 : WsFederationAuthorizationConstants
    {
        public WsFederationAuthorization12()
        {
            Namespace = "http://docs.oasis-open.org/wsfed/authorization/200706";
            DefaultPrefix = "auth";
            SchemaLocation = "http://docs.oasis-open.org/wsfed/authorization/v1.2/authorization.xsd";
        }
    }

    internal class WsFederationPrivacy12 : WsFederationPrivacyConstants
    {
        public WsFederationPrivacy12()
        {
            Namespace = "http://docs.oasis-open.org/wsfed/privacy/200706";
            DefaultPrefix = "priv";
            SchemaLocation = "http://docs.oasis-open.org/wsfed/privacy/v1.2/privacy.xsd";
        }
    }
}

