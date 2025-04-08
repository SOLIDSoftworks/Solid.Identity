using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    public struct EncryptionAlgorithmName
    {
        public EncryptionAlgorithmName(string algorithm)
        {
            Algorithm = algorithm;
        }

        public string Algorithm { get; }
    }
}
