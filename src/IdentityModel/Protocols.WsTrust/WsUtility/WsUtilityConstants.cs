#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsUtility
{
    public abstract class WsUtilityConstants : WsConstantsBase
    {
        public static IList<string> KnownNamespaces { get; } = new List<string> { "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" };

        public static WsUtility10Constants WsUtility10 { get; } = new  WsUtility10Constants();

        public WsUtilityConstants() { }
    }

    public class WsUtility10Constants : WsUtilityConstants
    {
        public WsUtility10Constants()
        {
            Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
            Prefix = "wsu";
        }
    }
}
 
