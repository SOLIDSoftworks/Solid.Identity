
using System;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Soap.Server.Service;
using Solid.Extensions.ServiceModel;

namespace Soap.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(5000);
            var services = new ServiceCollection()
                .AddProxy<IEchoServiceContract, EchoProxyInitializer>(options =>
                {
                    options.Endpoint = "http://localhost:5000/echo";
                })
                .BuildServiceProvider();
            
            //var client = new HttpClient();

            //for (var i = 0; i < 1_000_000; i++)
            //{
            //    Console.WriteLine($"Performing GET request: {i}");
            //    await client.GetAsync("http://localhost:5000");
            //}

            // var binding = new CustomBinding(new BasicHttpBinding());
            // var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
            // encoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
            //
            // var factory = new ChannelFactory<IEchoServiceContract>(binding, new EndpointAddress("http://localhost:5000/echo"));

            var factory = services.GetRequiredService<IProxyFactory>();
            for (var i = 0; i < 1_000_000; i++)
            {
                var channel = await factory.CreateProxyAsync<IEchoServiceContract>();
                // var channel = factory.CreateChannel();
                Console.WriteLine($"Performing echo request: {i}");
                _ = channel.Echo("echo");
                //Thread.Sleep(50);
            }
        }
        
        class EchoProxyInitializer : IProxyInitializer
        {
            public ValueTask<TProxy> InitializeProxyAsync<TProxy>(ProxyOptions options, SecurityToken token, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public ValueTask<TProxy> InitializeProxyAsync<TProxy>(ProxyOptions options, CancellationToken cancellationToken)
            {
                var basic = new BasicHttpBinding();
                basic.Apply(options);
                var binding = new CustomBinding(basic)
                {
                };
                var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
                encoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
                
                var factory = new ChannelFactory<TProxy>(binding, new EndpointAddress(options.Endpoint));
                return new ValueTask<TProxy>(factory.CreateChannel());
            }
        }
    }
}
