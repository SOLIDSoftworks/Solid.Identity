using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using Solid.Testing.AspNetCore.Extensions.Https.Abstractions;
using Solid.Testing.AspNetCore.Factories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Factories
{
    internal class HttpsWebHostFactory : DefaultWebHostFactory
    {
        private ISelfSignedCertificateFactory _certificateFactory;
        private IWebHostOptionsProvider _provider;

        public HttpsWebHostFactory(ISelfSignedCertificateFactory factory, IWebHostOptionsProvider provider)
            : base(provider)
        {
            _certificateFactory = factory;
            _provider = provider;
        }

        protected override void ConfigureKestrel(KestrelServerOptions options, string hostname)
        {
            var certificate = _certificateFactory.GenerateCertificate(hostname);
            options.Listen(IPAddress.Loopback, 0, listener => listener.UseHttps(https =>
            {
                https.ServerCertificate = certificate;
                https.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                https.AllowAnyClientCertificate();
            }));
        }
    }
}
