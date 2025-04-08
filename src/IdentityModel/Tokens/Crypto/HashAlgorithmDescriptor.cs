using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    internal class HashAlgorithmDescriptor : CryptoDescriptor<HashAlgorithm>
    {
        public HashAlgorithmDescriptor(string algorithm, Func<IServiceProvider, object[], HashAlgorithm> factory) 
            : base(algorithm, factory)
        {
        }
    }
}
