#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsPolicy
{
    /// <summary>
    /// Classes for specifying WS-Policy, 1.2 and 1.5.
    /// </summary>
    internal abstract class WsPolicyVersion
    {
        public static WsPolicyVersion Policy12 { get; } = new WsPolicy12Version();

        public static WsPolicyVersion Policy15 { get; } = new WsPolicy15Version();
    }

    /// <summary>
    /// Class for specifying WS-Policy 1.2.
    /// </summary>
    internal class WsPolicy12Version : WsPolicyVersion { }

    /// <summary>
    /// Class for specifying WS-Policy 1.5.
    /// </summary>
    internal class WsPolicy15Version : WsPolicyVersion { }

}
