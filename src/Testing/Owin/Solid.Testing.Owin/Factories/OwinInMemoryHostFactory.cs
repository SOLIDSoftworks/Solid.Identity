using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solid.Testing.Models;
using Microsoft.Owin.Hosting;
using System.Net.Sockets;
using System.Net;
using Solid.Testing.Abstractions.Factories;

namespace Solid.Testing.Extensions.Owin.Factories
{
    internal class OwinInMemoryHostFactory : IInMemoryHostFactory
    {
        public IInMemoryHost CreateHost(Type startup)
        {
            var listenerType = typeof(Microsoft.Owin.Host.HttpListener.OwinServerFactory);
            var port = GetFreeTcpPort();
            var options = new StartOptions
            {
                Port = port,
                AppStartup = startup.FullName
            };
            var address = new Uri($"http://localhost:{port}");
            var host = WebApp.Start(options);
            return new InMemoryHost(host, address);
        }
        
        private int GetFreeTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
