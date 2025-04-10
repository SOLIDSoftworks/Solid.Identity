using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.WsSecurity.Middleware;
using Solid.Identity.Protocols.WsTrust.WsTrust13;
using Solid.Identity.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Solid.Identity.DependencyInjection;
using Solid.Identity.Protocols.WsTrust;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWsTrust13AsyncService(this IApplicationBuilder builder)
            => builder.UseWsTrust13AsyncService(WsTrustDefaults.DefaultTrust13Path);
        public static IApplicationBuilder UseWsTrust13AsyncService(this IApplicationBuilder builder, PathString path)
            => builder.UseWsTrust13AsyncService(path, WsTrustContractOptions.DefaultTrust13Contract);
        public static IApplicationBuilder UseWsTrust13AsyncService(this IApplicationBuilder builder, WsTrustContractOptions options)
            => builder.UseWsTrust13AsyncService(WsTrustDefaults.DefaultTrust13Path, options);

        public static IApplicationBuilder UseWsTrust13AsyncService(this IApplicationBuilder builder, PathString path, WsTrustContractOptions options)
        {
            builder.ApplicationServices.InitializeCustomCryptoProvider();
            builder.MapSoapService<IWsTrust13AsyncContract>(path, app =>
            {
                app.UseTrust13(options);
            });
            return builder;
        }

        internal static void InitializeCustomCryptoProvider(this IServiceProvider services)
        {
            var cryptoProvider = services.GetService<ICryptoProvider>();
            CryptoProviderFactory.Default.CustomCryptoProvider = cryptoProvider;
        }
    }
}
