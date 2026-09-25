#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    public class WsSecurityUtilityConstants : WsProtocolConstants
    {
        public static WsSecurityUtilityConstants SecurityUtility10 { get; } = new WsSecurityUtility10();
        public static IDictionary<string, WsSecurityUtilityConstants> KnownNamespaces { get; } = new Dictionary<string, WsSecurityUtilityConstants>
        {
            { SecurityUtility10.Namespace, SecurityUtility10 }
        };
    }

    internal class WsSecurityUtility10 : WsSecurityUtilityConstants
    {
        public WsSecurityUtility10()
        {
            Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
            DefaultPrefix = "wsu";
        }
    }
}
 
