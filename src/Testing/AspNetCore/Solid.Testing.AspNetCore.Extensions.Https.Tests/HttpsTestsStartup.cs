using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class HttpsTestsStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder builder)
        {
            var middleware = new Func<HttpContext, Func<Task>, Task>(async (context, next) =>
            {
                var bytes = Encoding.UTF8.GetBytes(context.Request.IsHttps.ToString());
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = context.Request.IsHttps ? 4 : 5;
                context.Response.StatusCode = 200;
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            });
            builder.Use(middleware);
        }
    }
}
