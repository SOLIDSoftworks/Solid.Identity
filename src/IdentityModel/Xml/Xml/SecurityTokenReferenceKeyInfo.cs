using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Xml
{
    public class SecurityTokenReferenceKeyInfo : KeyInfo
    {
        public string KeyId { get; set; }
        public string KeyIdValueType { get; set; }
    }
}
