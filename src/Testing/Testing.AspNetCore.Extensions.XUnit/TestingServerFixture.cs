using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public class TestingServerFixture<TStartup> : TestingServerFixtureBase, IDisposable
    {
        private Lazy<TestingServer> _lazyTestingServer;
        private InMemoryConfigurationSource _configurationSource;

        public TestingServerFixture()
            : base()
        {
            _lazyTestingServer = new Lazy<TestingServer>(InitializeTestingServer, LazyThreadSafetyMode.ExecutionAndPublication);
        }
        public TestingServer TestingServer => _lazyTestingServer.Value;
        
        public void Dispose()
        {
            Disposing();
            if (_lazyTestingServer.IsValueCreated)
                _lazyTestingServer.Value.Dispose();
        }

        /// <summary>
        /// Updates the configuration for the TestingServer.
        /// <para>Disclaimer: Not all services/extensions register callbacks on the configuration change token.</para>
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="clear"></param>
        public virtual void UpdateConfiguration(Action<IInMemoryConfigurationBuilderRoot> configure, bool clear = false)
        {
            var builder = new InMemoryConfigurationBuilder();
            configure(builder);
            UpdateConfiguration(builder.InMemoryConfiguration, clear);
        }

        public virtual void UpdateConfiguration(IDictionary<string, string> data, bool clear = false)
        {
            // ensure initialized
            _ = TestingServer;
            if (_configurationSource == null) return;

            //if(_requests > 0)
            //    _wait = true;
            if (clear)
                _configurationSource.Provider.Clear();

            _configurationSource.Provider.Update(data);
            _configurationSource.Provider.Load();
        }

        protected virtual void Disposing() { }
        protected virtual void ConfigureAppConfiguration(IInMemoryConfigurationBuilderRoot builder) { }
        protected virtual void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder) { }
        protected virtual void ConfigureServices(IServiceCollection services) { }
        protected virtual TestingServerBuilder AddAspNetCoreHostFactory(TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(configure);

        protected TestingServer InitializeTestingServer()
        {
            return 
                AddAspNetCoreHostFactory(new TestingServerBuilder(), host =>
                {
                    host
                        .ConfigureServices(services =>
                        {
                            ConfigureRequiredServices(services);
                            ConfigureServices(services);
                        })
                        .ConfigureAppConfiguration((ctx, config) =>
                        {
                            ConfigureAppConfiguration(config, ConfigureAppConfiguration, out var source);
                            ConfigureAppConfiguration(ctx, config);
                            _configurationSource = source;
                        })
                    ;
                })
                .ConfigureAspNetCoreHost(ConfigureAspNetCoreHost)
                .AddTestingServices(ConfigureRequiredTestingServices)
                .AddStartup<TStartup>()
                .Build()
            ;
        }
    }
}
