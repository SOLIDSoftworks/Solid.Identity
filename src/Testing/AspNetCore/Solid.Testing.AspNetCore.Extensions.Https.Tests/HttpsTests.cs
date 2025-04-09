using Solid.Http;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class HttpsTests : IClassFixture<HttpsTestFixture<HttpsTestsStartup>>
    {
        private HttpsTestFixture<HttpsTestsStartup> _fixture;

        public HttpsTests(HttpsTestFixture<HttpsTestsStartup> fixture, ITestOutputHelper output)
        {
            fixture.SetOutput(output);
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldServeHttps()
        {
            await _fixture
                .TestingServer
                .GetAsync("/")
                .ShouldRespondSuccessfully()
                .Should(async response =>
                {
                    var text = await response.Content.ReadAsStringAsync();
                    var https = bool.Parse(text);
                    Assert.True(https);
                })
            ;
        }
    }
}
