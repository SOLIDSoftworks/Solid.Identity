#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// Classes for specifying WS-Trust Feb2005, 1.3 and 1.4.
    /// </summary>
    public abstract class WsTrustVersion
    {
        public static WsTrustVersion TrustFeb2005 = new WsTrustFeb2005Version();

        public static WsTrustVersion Trust13 = new WsTrust13Version();

        public static WsTrustVersion Trust14 = new WsTrust14Version();
    }

    /// <summary>
    /// Class for specifying WS-Trust Feb2005.
    /// </summary>
    internal class WsTrustFeb2005Version : WsTrustVersion {}

    /// <summary>
    /// Class for specifying WS-Trust 1.3.
    /// </summary>
    internal class WsTrust13Version : WsTrustVersion {}

    /// <summary>
    /// Class for specifying WS-Trust 1.4.
    /// </summary>
    internal class WsTrust14Version : WsTrustVersion {}
}
