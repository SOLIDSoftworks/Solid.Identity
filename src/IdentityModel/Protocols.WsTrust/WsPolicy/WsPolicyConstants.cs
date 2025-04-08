#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsPolicy
{
    public abstract class WsPolicyConstants : WsConstantsBase
    {
        public static IList<string> KnownNamespaces { get; } = new List<string> { "http://schemas.xmlsoap.org/ws/2004/09/policy", "http://www.w3.org/ns/ws-policy" };

        public static WsPolicy12Constants Policy12 { get; } = new WsPolicy12Constants();

        public static WsPolicy15Constants Policy15 { get; } = new WsPolicy15Constants();
    }

    public class WsPolicy12Constants : WsPolicyConstants
    {
        public WsPolicy12Constants()
        {
            Namespace = "http://schemas.xmlsoap.org/ws/2004/09/policy";
            Prefix = "wsp";
        }
    }

    public class WsPolicy15Constants : WsPolicyConstants
    {
        public WsPolicy15Constants()
        {
            Namespace = "http://www.w3.org/ns/ws-policy";
            Prefix = "wsp";
        }
    }
}
