using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    public struct SignatureAlgorithmName
    {
        public SignatureAlgorithmName(string algorithm)
        {
            Algorithm = algorithm;
        }

        public string Algorithm { get; }
    }
}
