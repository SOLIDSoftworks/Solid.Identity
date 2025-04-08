using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Solid_IdentityModel_Tokens_ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomCryptoProvider(this IServiceCollection services) => services.AddCustomCryptoProvider(options => options.AddFullSupport());
        public static IServiceCollection AddCustomCryptoProvider(this IServiceCollection services, Action<CryptoOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddLogging();
            services.TryAddSingleton<CustomCryptoProvider>();
            services.TryAddSingleton<ICryptoProvider>(p => p.GetRequiredService<CustomCryptoProvider>());
            services.TryAddTransient(p =>
            {
                p.InitializeDefaultCryptoProviderFactory();
                return CryptoProviderFactory.Default;
            });

            return services;
        }
    }
}
