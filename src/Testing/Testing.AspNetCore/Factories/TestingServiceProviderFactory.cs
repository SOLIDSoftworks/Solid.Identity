using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Solid.Testing.AspNetCore.Logging;
using Solid.Testing.AspNetCore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Solid.Testing.AspNetCore.Factories
{
    internal class TestingServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private IConfiguration _configuration;
        private LogMessageChannel _channel;

        public TestingServiceProviderFactory(IConfiguration configuration, LogMessageChannel channel)
        {
            _configuration = configuration;
            _channel = channel;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services) => services;

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var options = new ServiceProviderOptions
            {
                ValidateScopes = true
            };

            // These services get injected dead last. After Startup.ConfigureServices

            services.RemoveAll<LogMessageChannel>();
            services.AddSingleton(_channel);
            services.AddSingleton<ILoggerProvider, ChannelLoggerProvider>();
            services.AddLogging(logging => logging.AddConfiguration(_configuration.GetSection("Logging")));

            if (services.Any(s => s.ServiceType.Name.StartsWith("MvcMarker")))
            {
                var assemblies = GetApplicationParts(services);
                var builder = services.AddMvcCore();
                foreach (var assembly in assemblies)
                {
                    if (builder.PartManager.ApplicationParts.OfType<AssemblyPart>().Any(p => p.Assembly == assembly)) 
                        continue;

                    builder.AddApplicationPart(assembly);
                }
            }

            return services.BuildServiceProvider(options);
        }

        private IEnumerable<Assembly> GetApplicationParts(IServiceCollection services)
        {
            var assemblies = new Dictionary<string, Assembly>();
            using (var temp = services.BuildServiceProvider())
            {
                var provider = temp.GetService<StartupTypeProvider>();
                var type = provider.StartupType;
                while(type != typeof(object))
                {
                    var assembly = type.Assembly;
                    if (!assemblies.ContainsKey(assembly.FullName))
                        assemblies.Add(assembly.FullName, assembly);
                    type = type.BaseType;
                }
            }
            return assemblies.Values;
        }
    }
}
