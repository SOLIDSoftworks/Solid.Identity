#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Provides constants for WS-Security 1.0 and 1.1.
    /// </summary>
    public class WsSecurityConstants : WsProtocolConstants
    {
        public const string WsSecurity10Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        public const string WsSecurity11Namespace = "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd";
        
        public static WsSecurityConstants WsSecurity10 { get; } = new WsSecurity10();
        public static WsSecurityConstants WsSecurity11 { get; } = new WsSecurity11();
        
        public static readonly IDictionary<string, WsSecurityConstants> KnownNamespaces = new Dictionary<string, WsSecurityConstants>
        {
            { WsSecurity10Namespace, WsSecurity10 },
            { WsSecurity11Namespace, WsSecurity11}
        };

        public string FragmentBaseAddress { get; init; }

        public WsSecurityEncodingTypes EncodingTypes { get; init; }
    }

    internal class WsSecurity10 : WsSecurityConstants
    {
        public WsSecurity10()
        {
            EncodingTypes = WsSecurityEncodingTypes.WsSecurity10;
            FragmentBaseAddress = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0";
            DefaultPrefix = "wsse";
            Namespace = WsSecurity10Namespace;
        }
    }

    internal class WsSecurity11 : WsSecurityConstants
    {
        public WsSecurity11()
        {
            EncodingTypes = WsSecurityEncodingTypes.WsSecurity11;
            FragmentBaseAddress = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1";
            Namespace = WsSecurity11Namespace;
            DefaultPrefix = "wsse11";
        }
    }
}
