#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.XmlEnc
{
    public abstract class XmlEncryptionDataTypes
    {
        public static XmlEncryptionDataTypes XmlEnc11 { get; } = new XmlEncryption11DataTypes();

        public string Content { get; protected set; }
        
        public string Element { get; protected set; }
    }

    internal class XmlEncryption11DataTypes : XmlEncryptionDataTypes
    {

        public XmlEncryption11DataTypes()
        {
            Content = "http://www.w3.org/2001/04/xmlenc#Content";
            Element  = "http://www.w3.org/2001/04/xmlenc#Element";
        }
    }
}
