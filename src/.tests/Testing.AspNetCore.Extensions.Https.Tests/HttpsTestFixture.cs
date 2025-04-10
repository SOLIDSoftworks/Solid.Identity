using Microsoft.AspNetCore.Hosting;
using Solid.Http;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class HttpsTestFixture<TStartup> : TestingServerFixture<TStartup>
    {
        protected override TestingServerBuilder AddAspNetCoreHostFactory(TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => builder.AddAspNetCoreHttpsHostFactory("localhost", configure);
    }
}

