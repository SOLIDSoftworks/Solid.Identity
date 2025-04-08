#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Classes for specifying WS-Security, 1.0 and 1.1.
    /// </summary>
    public abstract class WsSecurityVersion
    {
        public static WsSecurityVersion Security10 = new WsSecurity10Version();

        public static WsSecurityVersion Security11 = new WsSecurity11Version();
    }

    /// <summary>
    /// Class for specifying WS-Security 10.
    /// </summary>
    internal class WsSecurity10Version : WsSecurityVersion { }

    /// <summary>
    /// Class for specifying WS-Security 11.
    /// </summary>
    internal class WsSecurity11Version : WsSecurityVersion { }

}
