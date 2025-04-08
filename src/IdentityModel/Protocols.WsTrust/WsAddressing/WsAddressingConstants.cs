using System.Collections.Generic;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsAddressing
{
    public abstract class WsAddressingConstants : WsConstantsBase
    {
        public static IList<string> KnownNamespaces { get; } = new List<string> { "http://www.w3.org/2005/08/addressing", "http://schemas.xmlsoap.org/ws/2004/08/addressing" };

        public static WsAddressing10Constants Addressing10 { get; } = new WsAddressing10Constants();

        public static WsAddressing200408Constants Addressing200408 { get; } = new WsAddressing200408Constants();

        public WsAddressingConstants() {}

        public string Type { get; protected set; }

        public string ValueType { get; protected set; }
    }

    public class WsAddressing10Constants : WsAddressingConstants
    {
        public WsAddressing10Constants()
        {
            Namespace = "http://www.w3.org/2005/08/addressing";
            Prefix = "wsa";
        }
    }

    public class WsAddressing200408Constants : WsAddressingConstants
    {
        public WsAddressing200408Constants()
        {
            Prefix = "wsa";
            Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
        }
    }
}
