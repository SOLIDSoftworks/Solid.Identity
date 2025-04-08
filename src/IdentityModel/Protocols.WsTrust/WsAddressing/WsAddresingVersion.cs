#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsAddressing
{
    /// <summary>
    /// Classes for specifying WS-Addressing, 1.0 and 200408.
    /// </summary>
    public abstract class WsAddressingVersion
    {
        public static WsAddressingVersion Addressing10 = new WsAddressing10Version();

        public static WsAddressingVersion Addressing200408 = new WsAddressing200408Version();
    }

    /// <summary>
    /// Class for specifying WS-Addressing 10.
    /// </summary>
    public class WsAddressing10Version : WsAddressingVersion { }

    /// <summary>
    /// Class for specifying WS-Addressing 200408.
    /// </summary>
    public class WsAddressing200408Version : WsAddressingVersion { }
}
