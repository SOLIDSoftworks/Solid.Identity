using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Xml
{
    public static class XmlEncConstants
    {
        public static readonly string Namespace = "http://www.w3.org/2001/04/xmlenc#";
        public static class Elements
        {
            public static readonly string EncryptedKey = nameof(EncryptedKey);
            public static readonly string EncryptionMethod = nameof(EncryptionMethod);
            public static readonly string CipherData = nameof(CipherData);
            public static readonly string CipherValue = nameof(CipherValue);
        }
    }
}
