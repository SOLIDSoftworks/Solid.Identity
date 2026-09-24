using System.Collections.Generic;
using System.ServiceModel.Channels;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsAddressing
{
    public class WsAddressingConstants : WsProtocolConstants
    {
        public const string Addressing200408Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
        public const string Addressing10Namespace = "http://www.w3.org/2005/08/addressing";
        
        public static WsAddressingConstants Addressing10 { get; } = new WsAddressing10();
        public static WsAddressingConstants Addressing200408 { get; } = new WsAddressing200408();
        
        public static IDictionary<string, WsAddressingConstants> KnownNamespaces { get; } = new Dictionary<string, WsAddressingConstants>
        {
            { Addressing200408Namespace, Addressing200408 },
            { Addressing10Namespace, Addressing10 }
        };
        
        public AddressingVersion AddressingVersion { get; init; }
    }

    internal class WsAddressing10 : WsAddressingConstants
    {
        public WsAddressing10()
        {
            Namespace = "http://www.w3.org/2005/08/addressing";
            DefaultPrefix = "wsa";
            AddressingVersion = AddressingVersion.WSAddressing10;
        }
    }

    internal class WsAddressing200408 : WsAddressingConstants
    {
        public WsAddressing200408()
        {
            DefaultPrefix = "wsa";
            Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
            AddressingVersion = AddressingVersion.WSAddressingAugust2004;
        }
    }
}
