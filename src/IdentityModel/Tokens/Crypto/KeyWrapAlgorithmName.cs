using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    public struct KeyWrapAlgorithmName
    {
        public KeyWrapAlgorithmName(string algorithm)
        {
            Algorithm = algorithm;
        }

        public string Algorithm { get; }
    }
}
