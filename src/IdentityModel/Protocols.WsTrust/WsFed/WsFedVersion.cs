#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsFed
{
    /// <summary>
    /// Classes for specifying WS-Fed 1.2.
    /// </summary>
    internal abstract class WsFedVersion
    {
        public static WsFedVersion Fed12 = new WsFed12Version();
    }

    /// <summary>
    /// Class for specifying WS-Addressing 10.
    /// </summary>
    internal class WsFed12Version : WsFedVersion { }
}
