#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.XmlEnc
{
    public abstract class XmlEncryptionConstants : WsProtocolConstants
    {
        public static XmlEncryptionConstants XmlEnc11 { get; } = new XmlEncryption11Constants();
    }

    internal class XmlEncryption11Constants : XmlEncryptionConstants
    {
        public XmlEncryption11Constants()
        {
            Namespace = "http://www.w3.org/2001/04/xmlenc#";
            DefaultPrefix = "xenc";
        }
    }
}
