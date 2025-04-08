using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    internal class AuthenticatedEncryptionProviderDescriptor : CryptoDescriptor<AuthenticatedEncryptionProvider>
    {
        public AuthenticatedEncryptionProviderDescriptor(string algorithm, Func<IServiceProvider, object[], AuthenticatedEncryptionProvider> factory)
            : base(algorithm, factory)
        {
        }
    }
}
