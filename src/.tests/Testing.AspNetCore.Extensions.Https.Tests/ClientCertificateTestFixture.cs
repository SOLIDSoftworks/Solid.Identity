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
using Solid.Testing.Certificates;

namespace Solid.Testing.AspNetCore.Extensions.Https.Tests
{
    public class ClientCertificateTestFixture : HttpsTestFixture<ClientCertificateStartup>
    {
        protected override TestingServerBuilder AddAspNetCoreHostFactory(TestingServerBuilder builder, Action<IWebHostBuilder> configure)
            => base.AddAspNetCoreHostFactory(builder
                .AddTestingServices(services =>
                {
                    services
                        .AddHttpClient("localhost")
                        .ConfigureHttpMessageHandlerBuilder(builder =>
                        {
                            var descriptor = new CertificateDescriptor
                            {
                                CommonName = "client",
                                Oids =
                                {
                                    Oids.ClientAuthentication,
                                    Oids.ServerAuthentication,
                                }
                            };
                            var certificate = CertificateStore.GetOrCreate(descriptor);
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
