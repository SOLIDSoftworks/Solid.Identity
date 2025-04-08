using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Soap.Server.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soap.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging => logging.ClearProviders());
            services.AddRouting();
            services.AddScopedSoapService<IEchoServiceContract, EchoService>(_ => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .Use((context, next) => next())
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapSoapService<IEchoServiceContract>("/echo");
                    endpoints.Map("/", endpoints.CreateApplicationBuilder().Use(async (HttpContext context, RequestDelegate next) =>
                    {
                        context.Items.Add("guid", Guid.NewGuid().ToString());
                        await context.Response.WriteAsync(context.Items["guid"] as string);
                    }).Build());
                })
            ;
        }
    }
}
