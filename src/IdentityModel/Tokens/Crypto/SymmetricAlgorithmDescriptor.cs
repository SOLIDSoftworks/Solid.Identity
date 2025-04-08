using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    internal class SymmetricAlgorithmDescriptor : CryptoDescriptor<SymmetricAlgorithm>
    {
        public SymmetricAlgorithmDescriptor(string algorithm, Func<IServiceProvider, object[], SymmetricAlgorithm> factory)
            : base(algorithm, factory)
        {
        }
    }
}
