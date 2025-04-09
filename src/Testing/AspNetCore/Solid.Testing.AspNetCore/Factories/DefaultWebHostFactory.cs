using Microsoft.AspNetCore.Hosting;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net;
using Solid.Testing.AspNetCore.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Solid.Testing.AspNetCore.Providers;

namespace Solid.Testing.AspNetCore.Factories
{
    public class DefaultWebHostFactory : WebHostFactory
    {
        public DefaultWebHostFactory(IWebHostOptionsProvider provider)
            : base(provider)
        {
        }
        protected override IWebHostBuilder InitializeWebHostBuilder(Type startup, string hostname) =>
            new WebHostBuilder()
                .UseKestrel(o => ConfigureKestrel(o, hostname))
                .ConfigureServices(services => services.AddSingleton(new StartupTypeProvider { StartupType = startup }))
                .UseStartup(startup)
            ;

        protected virtual void ConfigureKestrel(KestrelServerOptions options, string hostname) => options.Listen(IPAddress.Loopback, 0);
    }
}
