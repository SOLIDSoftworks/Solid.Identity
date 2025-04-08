#pragma warning disable 1591

using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Provides constants for WS-Security 1.0 and 1.1.
    /// </summary>
    public abstract class WsSecurityConstants : WsConstantsBase
    {
        public static readonly IList<string> KnownNamespaces = new List<string> { "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" };

        public static WsSecurity10Constants WsSecurity10 => new WsSecurity10Constants();

        public static WsSecurity11Constants WsSecurity11 => new WsSecurity11Constants();

        public string FragmentBaseAddress { get; protected set; }

        public WsSecurityEncodingTypes EncodingTypes { get; protected set; }
    }

    /// <summary>
    /// Provides constants for WS-Security 1.0.
    /// </summary>
    public class WsSecurity10Constants : WsSecurityConstants
    {
        public WsSecurity10Constants()
        {
            EncodingTypes = WsSecurityEncodingTypes.WsSecurity10;
            FragmentBaseAddress = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0";
            Prefix = "wsse";
            Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        }
    }

    /// <summary>
    /// Provides constants for WS-Security 1.1.
    /// </summary>
    public class WsSecurity11Constants : WsSecurityConstants
    {
        public WsSecurity11Constants()
        {
            EncodingTypes = WsSecurityEncodingTypes.WsSecurity11;
            FragmentBaseAddress = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1";
            Namespace = "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd";
            Prefix = "wsse11";
        }
    }
}
