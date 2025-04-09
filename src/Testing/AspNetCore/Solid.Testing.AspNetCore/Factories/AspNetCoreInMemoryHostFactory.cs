using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Testing.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Options;
using System.Threading.Tasks;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.Abstractions.Factories;
using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Logging;

namespace Solid.Testing.Extensions.AspNetCore.Factories
{
    internal class AspNetCoreInMemoryHostFactory : IInMemoryHostFactory, IDisposable
    {
        private IWebHostFactory _factory;
        private LogMessageReader _reader;
        private AspNetCoreHostOptions _options;
        private IDisposable _optionsChangeToken;

        public AspNetCoreInMemoryHostFactory(IWebHostFactory factory, LogMessageReader reader, IOptionsMonitor<AspNetCoreHostOptions> monitor)
        {
            _factory = factory;
            _reader = reader;
            _options = monitor.CurrentValue;
            _optionsChangeToken = monitor.OnChange((options, _) => _options = options);
        }

        public IInMemoryHost CreateHost<TStartup>()
        {
            return CreateHost(typeof(TStartup));
        }

        public IInMemoryHost CreateHost(Type startup)
        {
            EnsureLocal();

            var host = _factory.CreateWebHost(startup, _options.HostName);

            var urls = host.ServerFeatures.Get<IServerAddressesFeature>();
            var baseAddresses = urls.Addresses.Select(s => new Uri(s));
            var baseAddress = baseAddresses.First();
            //var baseAddress = urls.Addresses.Select(s => new Uri(s)).First();
            var url = new Uri($"{baseAddress.Scheme}://{_options.HostName}:{baseAddress.Port}");

            _reader.Start();

            return new InMemoryHost(host, url);
        }

        public void Dispose() => _optionsChangeToken?.Dispose();

        private void EnsureLocal()
        {
            var addresses = Dns.GetHostAddresses(_options.HostName);
            if (!addresses.Any(a => a.Equals(IPAddress.Loopback)))
                throw new ArgumentException("Parameter must be a local host name", "hostname");
        }
    }
}
