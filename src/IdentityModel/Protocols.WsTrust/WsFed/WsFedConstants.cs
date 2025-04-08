#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsFed
{
    public abstract class WsFedConstants : WsConstantsBase
    {
        public static IList<string> KnownNamespaces { get; } = new List<string> { "http://docs.oasis-open.org/wsfed/federation/200706" };

        public static IList<string> KnownAuthNamespaces { get; } = new List<string> { "http://docs.oasis-open.org/wsfed/authorization/200706" };

        public static WsFed12Constants Fed12 { get; } = new WsFed12Constants();

        public WsFedConstants() {}

        public string AuthNamespace { get; protected set; }

        public string AuthPrefix { get; protected set; }

        public string PrivacyNamespace { get; protected set; }

        public string PrivacyPrefix { get; protected set; }

        public string SchemaLocation { get; protected set; }
    }

    public class WsFed12Constants : WsFedConstants
    {
        public WsFed12Constants() 
        {
            AuthNamespace = "http://docs.oasis-open.org/wsfed/authorization/200706";
            AuthPrefix = "auth";
            Prefix = "fed";
            PrivacyNamespace = "http://docs.oasis-open.org/wsfed/privacy/200706";
            PrivacyPrefix = "priv";
            Namespace = "http://docs.oasis-open.org/wsfed/federation/200706";
            SchemaLocation = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3.xsd";
        }
    }
}



