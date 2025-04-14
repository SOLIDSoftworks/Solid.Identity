
using System;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Soap.Server.Service;

namespace Soap.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(5000);
            //var client = new HttpClient();

            //for (var i = 0; i < 1_000_000; i++)
            //{
            //    Console.WriteLine($"Performing GET request: {i}");
            //    await client.GetAsync("http://localhost:5000");
            //}

            var binding = new CustomBinding(new BasicHttpBinding())
            {
            };
            var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
            encoding.MessageVersion = MessageVersion.Soap12WSAddressing10;

            var factory = new ChannelFactory<IEchoServiceContract>(binding, new EndpointAddress("http://localhost:5000/echo"));

            for (var i = 0; i < 1_000_000; i++)
            {
                var channel = factory.CreateChannel();
                Console.WriteLine($"Performing echo request: {i}");
                _ = channel.Echo("echo");
                //Thread.Sleep(50);
            }
        }
    }
}
