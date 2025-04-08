#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.XmlEnc
{
    internal abstract class XmlEncryptionDataTypes
    {
        public static XmlEncryption11DataTypes XmlEnc11 { get; } = new XmlEncryption11DataTypes();

        public XmlEncryptionDataTypes() {}

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
