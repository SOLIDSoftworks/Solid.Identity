#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Provides keytypes for WS-Security 1.0 and 1.1.
    /// </summary>
    public abstract class WsSecurityKeyTypes
    {
        public static WsSecurity10KeyTypes WsSecurity10 { get; } = new WsSecurity10KeyTypes();

        public static WsSecurity11KeyTypes WsSecurity11 { get; } = new WsSecurity11KeyTypes();

        public string Sha1Thumbprint { get; protected set; }
    }

    /// <summary>
    /// Provides keytypes for WS-Security 1.0.
    /// </summary>
    public class WsSecurity10KeyTypes : WsSecurityKeyTypes
    {
        public WsSecurity10KeyTypes()
        {
            Sha1Thumbprint = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0/#ThumbprintSHA1";
        }
    }

    /// <summary>
    /// Provides keytypes for WS-Security 1.1.
    /// </summary>
    public class WsSecurity11KeyTypes : WsSecurityKeyTypes
    {
        public WsSecurity11KeyTypes()
        {
            Sha1Thumbprint = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1/#ThumbprintSHA1";
        }
    }
}
