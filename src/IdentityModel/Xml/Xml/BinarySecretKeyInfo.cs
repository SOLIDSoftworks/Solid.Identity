using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Xml
{
    public class BinarySecretKeyInfo : KeyInfo
    {
        public byte[] Key { get; set; }
    }
}
