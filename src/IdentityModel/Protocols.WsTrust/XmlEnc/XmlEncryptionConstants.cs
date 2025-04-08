#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.XmlEnc
{
    internal abstract class XmlEncryptionConstants : WsConstantsBase
    {
        public static XmlEncryption11Constants XmlEnc11 { get; } = new XmlEncryption11Constants();

        public XmlEncryptionConstants() {}
    }

    internal class XmlEncryption11Constants : XmlEncryptionConstants
    {
        public XmlEncryption11Constants()
        {
            Namespace = "http://www.w3.org/2001/04/xmlenc#";
            Prefix = "xenc";
        }
    }
}
