using Solid.Extensions.ServiceModel;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class ServiceCollectionExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Adds a proxy that uses <see cref="DefaultProxyInitializer"/> for initialization.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add the proxy to.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="ProxyOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddProxy<TProxy>(this IServiceCollection services, Action<ProxyOptions> configureOptions)
            => services.AddProxy<TProxy, DefaultProxyInitializer>(configureOptions);

        /// <summary>
        /// Adds a proxy that uses <typeparamref name="TProxyInitializer"/> for intialization.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy.</typeparam>
        /// <typeparam name="TProxyInitializer">The type of proxy initializer that initialized the proxy.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add the proxy to.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="ProxyOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddProxy<TProxy, TProxyInitializer>(this IServiceCollection services, Action<ProxyOptions> configureOptions)
            where TProxyInitializer : class, IProxyInitializer
            => services.AddAndConfigureProxy<TProxy, TProxyInitializer>(KeyFactory.CreateKey<TProxy>(), configureOptions);

        /// <summary>
        /// Configures a proxy using a delegate.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance the proxy has been added to.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="ProxyOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection ConfigureProxy<TProxy>(this IServiceCollection services, Action<ProxyOptions> configureOptions)
            => services.ConfigureProxyOptions(KeyFactory.CreateKey<TProxy>(), configureOptions);

        /// <summary>
        /// Configures all proxies using a delegate.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance the proxies have been added to.</param>
        /// <param name="configureOptions">A delegate to configure <see cref="ProxyOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection ConfigureAllProxies(this IServiceCollection services, Action<ProxyOptions> configureOptions)
            => services.ConfigureAll(configureOptions);

        /// <summary>
        /// Adds an <see cref="ISoapSecurityTokenProvider"/> that can provide a security token from the current context.
        /// <para>This service is added in <see cref="ServiceLifetime.Transient"/> scope.</para>
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSoapSecurityTokenProvider<TProvider>(this IServiceCollection services)
            where TProvider : class, ISoapSecurityTokenProvider
        {
            services.TryAddTransient<ISoapSecurityTokenProvider, TProvider>();
            return services;
        }

        internal static IServiceCollection ConfigureProxyOptions(this IServiceCollection services, string key, Action<ProxyOptions> configureOptions)
            => services.Configure(key, configureOptions);

        internal static IServiceCollection AddAndConfigureProxy<TService, TProxyInitializer>(this IServiceCollection services, string key, Action<ProxyOptions> configureOptions)
            where TProxyInitializer : class, IProxyInitializer
        {
            services.Configure<ProxyOptions>(key, options => options.ProxyInitializerType = typeof(TProxyInitializer));
            services.Configure(key, configureOptions);
            services.PostConfigure<ProxyOptions>(key, options =>
            {
                if (string.IsNullOrEmpty(options.Endpoint))
                    throw new ArgumentNullException(nameof(options.Endpoint));
            });

            return services.AddRequiredServices<TProxyInitializer>();
        }

        internal static IServiceCollection AddRequiredServices<TProxyInitializer>(this IServiceCollection services)
            where TProxyInitializer : class, IProxyInitializer
        {
            services.TryAddScoped<IProxyFactory, ProxyFactory>();
            services.TryAddTransient<TProxyInitializer>();
            return services;
        }
    }
}
