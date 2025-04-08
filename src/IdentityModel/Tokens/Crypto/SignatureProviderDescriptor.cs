using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    internal class SignatureProviderDescriptor : CryptoDescriptor<SignatureProvider>
    {
        public SignatureProviderDescriptor(string algorithm, Func<IServiceProvider, object[], SignatureProvider> factory) 
            : base(algorithm, factory)
        {
        }
    }
}
