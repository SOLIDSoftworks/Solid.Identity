using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;
using Solid.Testing.AspNetCore.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    public class MultipleTestingServerFixture<TStartup> : TestingServerFixtureBase, IDisposable
    {
        private ConcurrentDictionary<string, TestingServer> _testingServers;
        private ConcurrentDictionary<string, InMemoryConfigurationSource> _configurationSources;
        private ConcurrentDictionary<Guid, ITestOutputHelper> _helpers;
        private AsyncLocal<Guid> _localGuid = new AsyncLocal<Guid>();

        public MultipleTestingServerFixture()
            : base()
        {
            _testingServers = new ConcurrentDictionary<string, TestingServer>();
            _configurationSources = new ConcurrentDictionary<string, InMemoryConfigurationSource>();
        }

        public TestingServer GetTestingServer(string key)
            => _testingServers.GetOrAdd(key, k => InitializeTestingServer(k));
        
        public void Dispose()
        {
            Disposing();
            foreach (var server in _testingServers.Values)
                server.Dispose();
        }

        public virtual void UpdateConfiguration(string name, Action<IInMemoryConfigurationBuilderRoot> configure, bool clear = false)
        {
            var builder = new InMemoryConfigurationBuilder();
            configure(builder);
            UpdateConfiguration(name, builder.InMemoryConfiguration, clear);
        }

        public virtual void UpdateConfiguration(string name, IDictionary<string, string> data, bool clear = false)
        {
            // ensure initialized
            _ = GetTestingServer(name);
            if (!_configurationSources.TryGetValue(name, out var source)) return;

            if (clear)
                source.Provider.Clear();

            source.Provider.Update(data);
            source.Provider.Load();
        }

        protected virtual void Disposing() { }
        protected virtual void ConfigureAppConfiguration(string name, WebHostBuilderContext context, IConfigurationBuilder builder) { }
        protected virtual void ConfigureAppConfiguration(string name, IInMemoryConfigurationBuilderRoot builder) { }
        protected virtual void ConfigureServices(string name, IServiceCollection services) { }
        protected virtual TestingServerBuilder AddAspNetCoreHostFactory(string name, TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHostFactory(configure);

        protected TestingServer InitializeTestingServer(string name)
        {
            return
                AddAspNetCoreHostFactory(name, new TestingServerBuilder(), host =>
                {
                    host
                        .ConfigureServices(services =>
                        {
                            ConfigureRequiredServices(services);
                            ConfigureServices(name, services);
                        })
                        .ConfigureAppConfiguration((ctx, config) =>
                        {
                            ConfigureAppConfiguration(config, b => ConfigureAppConfiguration(name, b), out var source);
                            ConfigureAppConfiguration(name, ctx, config);

                            _configurationSources.AddOrUpdate(name, source, (_, __) => source);
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
