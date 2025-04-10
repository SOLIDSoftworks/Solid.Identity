using Solid.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Providers
{
    internal class TestingHttpClientProvider : IHttpClientProvider
    {
        private IHttpClientFactory _factory;

        public TestingHttpClientProvider(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public HttpClient Get(Uri _) => _factory.CreateClient("localhost");
    }
}
