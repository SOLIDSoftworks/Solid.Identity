using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    internal class KeyedHashAlgorithmDescriptor : CryptoDescriptor<KeyedHashAlgorithm>
    {
        public KeyedHashAlgorithmDescriptor(string algorithm, Func<IServiceProvider, object[], KeyedHashAlgorithm> factory) 
            : base(algorithm, factory)
        {
        }
    }
}
