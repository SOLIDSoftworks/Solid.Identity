using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider InitializeDefaultCryptoProviderFactory(this IServiceProvider services)
        {
            var provider = services.GetRequiredService<ICryptoProvider>();
            CryptoProviderFactory.Default.CustomCryptoProvider = provider;
            return services;
        }
    }
}
