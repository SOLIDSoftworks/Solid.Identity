using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Extensions.AspNetCore.Soap;
using Solid.Extensions.AspNetCore.Soap.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions methods for adding SOAP services to <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static Action<SoapServiceBuilder> _noop = _ => { };

        /// <summary>
        /// Adds a SOAP service to the <see cref="IServiceCollection" />.
        /// <para>This SOAP service will be registered with the <seealso cref="ServiceLifetime.Scoped" /> lifetime.</para>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> instance.</param>
        /// <param name="factory">A factory method for instantiating the SOAP service implementation.</param>
        /// <param name="configure">A delegate used to configure the SOAP service.</param>
        /// <typeparam name="TService">The contract of the SOAP service.</typeparam>
        /// <returns>The <see cref="IServiceCollection" /> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddScopedSoapService<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory, Action<SoapServiceBuilder> configure = null)
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Scoped<TService>(factory), configure ?? _noop);

        /// <summary>
        /// Adds a SOAP service to the <see cref="IServiceCollection" />.
        /// <para>This SOAP service will be registered with the <seealso cref="ServiceLifetime.Scoped" /> lifetime.</para>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> instance.</param>
        /// <param name="configure">A delegate used to configure the SOAP service.</param>
        /// <typeparam name="TService">The contract of the SOAP service.</typeparam> 
        /// <typeparam name="TImplementation">The implementation type of the SOAP service.</typeparam>
        /// <returns>The <see cref="IServiceCollection" /> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddScopedSoapService<TService, TImplementation>(this IServiceCollection services, Action<SoapServiceBuilder> configure = null)
            where TImplementation : class, TService
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Scoped<TService, TImplementation>(), configure ?? _noop);

        /// <summary>
        /// Adds a SOAP service to the <see cref="IServiceCollection" />.
        /// <para>This SOAP service will be registered with the <seealso cref="ServiceLifetime.Singleton" /> lifetime.</para>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> instance.</param>
        /// <param name="instance">A singleton instance of <typeparamref name="TService" />.</param>
        /// <param name="configure">A delegate used to configure the SOAP service.</param>
        /// <typeparam name="TService">The contract of the SOAP service.</typeparam>
        /// <returns>The <see cref="IServiceCollection" /> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSingletonSoapService<TService>(this IServiceCollection services, TService instance, Action<SoapServiceBuilder> configure = null)
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Singleton<TService>(instance), configure ?? _noop);

        /// <summary>
        /// Adds a SOAP service to the <see cref="IServiceCollection" />.
        /// <para>This SOAP service will be registered with the <seealso cref="ServiceLifetime.Singleton" /> lifetime.</para>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> instance.</param>
        /// <param name="factory">A factory method for instantiating the SOAP service implementation.</param>
        /// <param name="configure">A delegate used to configure the SOAP service.</param>
        /// <typeparam name="TService">The contract of the SOAP service.</typeparam>
        /// <returns>The <see cref="IServiceCollection" /> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSingletonSoapService<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory, Action<SoapServiceBuilder> configure = null)
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Singleton<TService>(factory), configure ?? _noop);

        /// <summary>
        /// Adds a SOAP service to the <see cref="IServiceCollection" />.
        /// <para>This SOAP service will be registered with the <seealso cref="ServiceLifetime.Singleton" /> lifetime.</para>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> instance.</param>
        /// <param name="configure">A delegate used to configure the SOAP service.</param>
        /// <typeparam name="TService">The contract of the SOAP service.</typeparam> 
        /// <typeparam name="TImplementation">The implementation type of the SOAP service.</typeparam>
        /// <returns>The <see cref="IServiceCollection" /> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSingletonSoapService<TService, TImplementation>(this IServiceCollection services, Action<SoapServiceBuilder> configure = null)
            where TImplementation : class, TService
            where TService : class
            => services.AddSoapService(ServiceDescriptor.Singleton<TService, TImplementation>(), configure ?? _noop);

        /// <summary>
        /// Configures options for <typeparamref name="TService" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> instance.</param>
        /// <param name="configureOptions">A delegate used to configure options for <typeparamref name="TService" />.</param>
        /// <typeparam name="TService">The contract of the SOAP service.</typeparam> 
        /// <returns>The <see cref="IServiceCollection" /> instance so that additional calls can be chained.</returns>
        public static IServiceCollection ConfigureSoapService<TService>(this IServiceCollection services, Action<SoapServiceOptions> configureOptions)
            => services.Configure(typeof(TService).FullName, configureOptions);

        static IServiceCollection AddSoapService(this IServiceCollection services, ServiceDescriptor descriptor, Action<SoapServiceBuilder> configure)
        {
            var type = descriptor.ServiceType;
            services.Configure<SoapServiceOptions>(type.FullName, options =>
            {
                options.Name = type.GetServiceName();
                options.Namespace = type.GetServiceNamespace();
            });

            var builder = new SoapServiceBuilder(type, services);
            configure(builder);

            services.AddHttpContextAccessor();
            services.AddLogging();

            services.TryAdd(descriptor);
            services.TryAddSingleton<OperationDescriptorFactory>();
            services.TryAddSingleton<MethodLocator>();
            services.TryAddSingleton<MethodInvoker>();
            services.TryAddSingleton<MethodParameterReader>();
            services.TryAddSingleton<SoapContextAccessor>();
            services.TryAddSingleton<ISoapContextAccessor, SoapContextAccessor>();

            return services;
        }
    }
}
