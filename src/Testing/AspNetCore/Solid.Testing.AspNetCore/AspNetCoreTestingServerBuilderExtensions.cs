using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Solid.Testing;
using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using Solid.Testing.AspNetCore.Factories;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Options;
using Solid.Testing.AspNetCore.Providers;
using Solid.Testing.Extensions.AspNetCore.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Solid.Http
{
    public static class AspNetCoreTestingServerBuilderExtensions
    {
        private static readonly string _defaultHostName = "localhost";

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder)
            => builder.AddAspNetCoreHostFactory(_defaultHostName);

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="hostname">The hostname for the in memory host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, string hostname)
            => builder.AddAspNetCoreHostFactory(hostname, _ => { });

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="configure">Configuration delegate for the web host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(_defaultHostName, configure);

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="hostname">The hostname for the in memory host</param>
        /// <param name="configure">Configuration delegate for the web host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, string hostname, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(options => options.HostName = hostname, configure);

        /// <summary>
        /// Adds the host factory for asp net core
        /// </summary>
        /// <param name="builder">The testing server builder</param>
        /// <param name="configureOptions">Configures options.</param>
        /// <param name="configure">Configuration delegate for the web host</param>
        /// <returns>The testing server builder</returns>
        public static TestingServerBuilder AddAspNetCoreHostFactory(this TestingServerBuilder builder, Action<AspNetCoreHostOptions> configureOptions, Action<IWebHostBuilder> configure)
        {
            var channel = new LogMessageChannel();

            var c = new Action<IWebHostBuilder>(webHost =>
            {
                configure(webHost);
                webHost
                    .ConfigureServices((context, services) =>
                    {
                        var descriptor = ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(new TestingServiceProviderFactory(context.Configuration, channel));
                        services.Replace(descriptor);
                    })
                ;
            });

            builder
                .ConfigureAspNetCoreHost(configureOptions)
                .AddTestingServices(services =>
                {
                    services.RemoveAll<LogMessageChannel>();
                    services.RemoveAll<LogMessageReader>();
                    services.AddSingleton(channel);
                    services.AddSingleton<LogMessageReader>();
                    services.AddSingleton<IWebHostOptionsProvider>(new WebHostOptionsProvider { Configure = c });
                    services.TryAddSingleton<IWebHostFactory, DefaultWebHostFactory>();
                })
            ;

            return builder.AddHostFactory<AspNetCoreInMemoryHostFactory>();
        }

        public static TestingServerBuilder ConfigureAspNetCoreHost(this TestingServerBuilder builder, Action<AspNetCoreHostOptions> configureOptions)
        {
            return builder.AddTestingServices(services =>
            {
                services.Configure(configureOptions);
            }); 
        }
    }
}
