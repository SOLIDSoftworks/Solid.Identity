using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.Http.Soap.Tests.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddScopedSoapService<IEchoServiceContract, EchoService>(_ => { });
        }

        public void Configure(IApplicationBuilder builder)
        {
            builder
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapSoapService<IEchoServiceContract>("/echo");
                    endpoints.MapSoapService<IEchoServiceContract>("/echo1", MessageVersion.Soap11);
                    endpoints.MapSoapService<IEchoServiceContract>("/echo2", MessageVersion.Soap11WSAddressingAugust2004);
                    endpoints.MapSoapService<IEchoServiceContract>("/echo3", MessageVersion.Soap12WSAddressingAugust2004);
                    endpoints.MapSoapService<IEchoServiceContract>("/echo4", MessageVersion.Soap12WSAddressing10);
                })
            ;
        }
    }
}
