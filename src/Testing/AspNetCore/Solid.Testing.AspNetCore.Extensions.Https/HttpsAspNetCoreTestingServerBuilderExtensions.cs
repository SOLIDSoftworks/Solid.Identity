using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Extensions.Https.Abstractions;
using Solid.Testing.AspNetCore.Extensions.Https.Factories;
using Solid.Testing.AspNetCore.Extensions.Https.Options;
using Solid.Testing.AspNetCore.Extensions.Https.Providers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Solid.Http
{
    /// <summary>
    /// Https asp net core testing server builder extensions
    /// </summary>
    public static class HttpsAspNetCoreTestingServerBuilderExtensions
    {
        private static readonly string _defaultHostName = "localhost";

        /// <summary>
        /// Adds the host factory for asp net core hosted on https
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder)
            => builder.AddAspNetCoreHttpsHostFactory(_defaultHostName);

        /// <summary>
        /// Adds the host factory for asp net core hosted on https
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="hostname">The hostname for the in memory host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder, string hostname)
            => builder.AddAspNetCoreHttpsHostFactory(hostname, _ => { });

        /// <summary>
        /// Adds the host factory for asp net core hosted on https
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="configure">Configuration delegate for the web host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHttpsHostFactory(_defaultHostName, configure);

        /// <summary>
        /// Adds the host factory for asp net core hosted on https
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="hostname">The hostname for the in memory host</param>
        /// <param name="configure">Configuration delegate for the web host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHttpsHostFactory(this TestingServerBuilder builder, string hostname, Action<IWebHostBuilder> configure)
        {
            builder
                .AddTestingServices(services =>
                { 
                    services
                        .AddSingleton<ISelfSignedCertificateFactory, SelfSignedCertificateFactory>()
                        .AddSingleton<IWebHostFactory, HttpsWebHostFactory>()
                        .AddSingleton<IConfigureOptions<HttpClientFactoryOptions>, ConfigureNamedHttpClientFactoryOptions>()
                    ;
                    services.TryAddTransient<IHttpClientProvider, TestingHttpClientProvider>();
                })
            ;
            return builder.AddAspNetCoreHostFactory(hostname, configure);
        }
    }
}
