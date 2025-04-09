using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Solid.Http;
using Solid.Testing.Abstractions;
using Solid.Testing.Abstractions.Factories;
using System;
using System.Linq;
using System.Net.Http;

namespace Solid.Http
{
    /// <summary>
    /// The testing server builder
    /// </summary>
    public class TestingServerBuilder
    {
        private Action<IServiceCollection> _servicesAction = (_ => { });
        private Type _startup;
        
        /// <summary>
        /// Adds a host factory of the generic type
        /// </summary>
        /// <typeparam name="TFactory">The implementation of IInMemoryHostFactory</typeparam>
        /// <returns>The testing server builder</returns>
        public TestingServerBuilder AddHostFactory<TFactory>()
            where TFactory : class, IInMemoryHostFactory
        {
            AddTestingServices(services =>
            {
                var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInMemoryHostFactory));
                if (descriptor != null)
                    throw new InvalidOperationException($"Host factory ({descriptor.ServiceType.FullName}) has already been added.");
                services.AddSingleton<IInMemoryHostFactory, TFactory>();
            });
            return this;
        }

        /// <summary>
        /// Adds a host factory of the generic type
        /// </summary>
        /// <param name="factory">An instance of IInMemoryHostFactory</param>
        /// <returns>The testing server builder</returns>
        public TestingServerBuilder AddHostFactory(IInMemoryHostFactory factory)
        {
            AddTestingServices(services =>
            {
                var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IInMemoryHostFactory));
                if (descriptor != null)
                    throw new InvalidOperationException($"Host factory ({descriptor.ServiceType.FullName}) has already been added.");
                services.AddSingleton<IInMemoryHostFactory>(factory);
            });
            return this;
        }

        /// <summary>
        /// Adds services to the testing server service provider
        /// <para>These are NOT services that are used internally by the in memory host</para>
        /// </summary>
        /// <param name="action">Add services action</param>
        /// <returns>The testing server builder</returns>
        public TestingServerBuilder AddTestingServices(Action<IServiceCollection> action)
        {
            _servicesAction += action;
            return this;
        }

        /// <summary>
        /// Adds the startup class
        /// </summary>
        /// <typeparam name="TStartup">The startup class type</typeparam>
        /// <returns>The testing server builder</returns>
        public TestingServerBuilder AddStartup<TStartup>()
        {
            return AddStartup(typeof(TStartup));
        }

        /// <summary>
        /// Adds the startup type
        /// </summary>
        /// <param name="type">The startup class type</param>
        /// <returns>The testing server builder</returns>
        public TestingServerBuilder AddStartup(Type type)
        {
            if (_startup != null)
                throw new InvalidOperationException($"Startup type ({_startup.FullName}) has already been added.");
            _startup = type;
            return this;
        }

        /// <summary>
        /// Configures the Solid.Http http client used to communicate with the in memory host
        /// </summary>
        /// <param name="action">The configuration action</param>
        /// <returns>The testing server builder</returns>
        [Obsolete("Use AddTestingServices(services => services.ConfigureSolidHttp(action)) instead")]
        public TestingServerBuilder ConfigureSolidHttp(Action<SolidHttpBuilder> action)
            => AddTestingServices(services => services.ConfigureSolidHttp(action));

        /// <summary>
        /// Builds the TestingServer
        /// </summary>
        /// <returns>The TestingServer</returns>
        public TestingServer Build()
        {
            var services = new ServiceCollection();
            foreach(var action in _servicesAction.GetInvocationList().Cast<Action<IServiceCollection>>())
                action(services);   
            services.AddSolidHttp();

            var provider = services.BuildServiceProvider();
            var factory = provider.GetService<IInMemoryHostFactory>();
            if (factory == null)
                throw new InvalidOperationException("Cannot build testing server without an InMemoryHostFactory");
            if (_startup == null)
                throw new InvalidOperationException("Cannot build testing server without Startup class");

            var host = factory.CreateHost(_startup);
            return new TestingServer(host, provider);
        }
    }
}
