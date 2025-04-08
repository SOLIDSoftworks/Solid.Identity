#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Provides encodingtypes for WS-Security 1.0 and 1.1.
    /// </summary>
    public abstract class WsSecurityEncodingTypes : WsConstantsBase
    {
        public static WsSecurity10EncodingTypes WsSecurity10 { get; } = new WsSecurity10EncodingTypes();

        public static WsSecurity11EncodingTypes WsSecurity11 { get; } = new WsSecurity11EncodingTypes();

        public WsSecurityEncodingTypes() {}

        public string Base64 { get; protected set; }

        public string HexBinary { get; protected set; }

        public string Text { get; protected set; }
    }

    /// Provides encodingtypes for WS-Security 1.0.
    public class WsSecurity10EncodingTypes : WsSecurityEncodingTypes
    {
        public WsSecurity10EncodingTypes()
        {
            Base64 = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0/#Base64Binary";
            HexBinary = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0/#HexBinary";
            Text = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0/#Text";
        }
    }

    /// Provides encodingtypes for WS-Security 1.1.
    public class WsSecurity11EncodingTypes : WsSecurityEncodingTypes
    {
        public WsSecurity11EncodingTypes()
        {
            Base64 = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1/#Base64Binary";
            HexBinary = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1/#HexBinary";
            Text = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1/#Text";
        }
    }
}
