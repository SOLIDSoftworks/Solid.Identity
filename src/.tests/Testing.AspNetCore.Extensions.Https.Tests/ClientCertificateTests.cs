using Solid.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class ClientCertificateTests : IClassFixture<ClientCertificateTestFixture>
    {
        private ClientCertificateTestFixture _fixture;

        public ClientCertificateTests(ClientCertificateTestFixture fixture, ITestOutputHelper output)
        {
            fixture.SetOutput(output);
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldSendClientCertificate()
        {
            await _fixture
                .TestingServer
                .GetAsync("/")
                .ShouldRespondSuccessfully()
            ;
        }
    }
}
