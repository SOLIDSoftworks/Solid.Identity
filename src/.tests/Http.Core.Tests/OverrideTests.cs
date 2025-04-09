using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace Solid.Http.Core.Tests
{
    public class OverrideTests
    {
        [Fact]
        public async Task ShouldOverrideHttpClientFactory()
        {
            var handler = Substitute.For<HttpClientHandler>();
            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(handler));
            var services = new ServiceCollection()
                .AddSingleton<IHttpClientFactory>(factory)
                .AddSolidHttpCore()
                .BuildServiceProvider()
            ;

            var client = services.GetService<ISolidHttpClientFactory>().Create();
            try
            {
                _ = await client.GetAsync("http://notused");
            }
            catch(InvalidOperationException) { }

            factory.Received(Quantity.Exactly(1)).CreateClient(Arg.Any<string>());
        }
    }
}
