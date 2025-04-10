using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Solid.Http;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class ClientCertificateTestFixture : HttpsTestFixture<ClientCertificateStartup>
    {
        private static readonly string ClientCertificateBase64 = "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";
        
        protected override TestingServerBuilder AddAspNetCoreHostFactory(TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => base.AddAspNetCoreHostFactory(builder
                .AddTestingServices(services =>
                {
                    services
                        .AddHttpClient("localhost")
                        .ConfigureHttpMessageHandlerBuilder(builder =>
                        {
                            var certificate = new X509Certificate2(Convert.FromBase64String(ClientCertificateBase64), null as string);
                            if (builder.PrimaryHandler is HttpClientHandler httpClientHandler)
                            {
                                WriteLine("Adding client certificate to HttpClientHandler");
                                httpClientHandler.ClientCertificates.Add(certificate);
                                httpClientHandler.SslProtocols = SslProtocols.Tls12;
                                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                                WriteLine("Client certificate added");
                            }

                            if (builder.PrimaryHandler is SocketsHttpHandler socketsHttpHandler)
                            {
                                WriteLine("Adding client certificate to SocketsHttpHandler");
                                socketsHttpHandler.SslOptions = new SslClientAuthenticationOptions
                                {
                                    ClientCertificates = new X509CertificateCollection()
                                    {
                                        certificate
                                    },
                                    LocalCertificateSelectionCallback = (sender, target, localCertificates,
                                        remoteCertificates, acceptedIssuers) => localCertificates.OfType<X509Certificate2>().FirstOrDefault()
                                };
                                WriteLine("Client certificate added");
                            }
                        })
                    ;
                }), configure)
        ;
    }
}
