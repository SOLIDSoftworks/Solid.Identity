#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    public class WsSecurityPolicyConstants : WsProtocolConstants
    {
        public static WsSecurityPolicyConstants SecurityPolicy12 { get; } = new WsSecurityPolicy12();
        public static WsSecurityPolicyConstants SecurityPolicy15 { get; } = new WsSecurityPolicy15();
        public static IDictionary<string, WsSecurityPolicyConstants> KnownNamespaces { get; } = new Dictionary<string, WsSecurityPolicyConstants>
        { 
            { SecurityPolicy12.Namespace, SecurityPolicy12 },
            { SecurityPolicy15.Namespace, SecurityPolicy15 }
        };
    }

    internal class WsSecurityPolicy12 : WsSecurityPolicyConstants
    {
        public WsSecurityPolicy12()
        {
            Namespace = "http://schemas.xmlsoap.org/ws/2004/09/policy";
            DefaultPrefix = "wsp";
        }
    }

    public class WsSecurityPolicy15 : WsSecurityPolicyConstants
    {
        public WsSecurityPolicy15()
        {
            Namespace = "http://www.w3.org/ns/ws-policy";
            DefaultPrefix = "wsp";
        }
    }
}
