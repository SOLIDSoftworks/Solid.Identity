using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Solid.Testing.AspNetCore.Extensions.Https.Options
{
    internal class ConfigureNamedHttpClientFactoryOptions : IConfigureNamedOptions<HttpClientFactoryOptions>
    {
        public void Configure(string name, HttpClientFactoryOptions options)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            options.HttpMessageHandlerBuilderActions.Add(builder =>
            {
                if (builder.PrimaryHandler is HttpClientHandler httpClientHandler)
                    ConfigureHttpClientHandler(name, httpClientHandler);
#if NETCOREAPP3_1
                if (builder.PrimaryHandler is SocketsHttpHandler socketsHttpHandler)
                    ConfigureSocketsHttpHandler(name, socketsHttpHandler);
#endif
            });
        }

        public void Configure(HttpClientFactoryOptions options)
            => Configure(Microsoft.Extensions.Options.Options.DefaultName, options);

        private void ConfigureHttpClientHandler(string host, HttpClientHandler handler)
        {
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            handler.ServerCertificateCustomValidationCallback = (_, certificate, __, ___) =>
            {
                return certificate.Subject == $"CN={host}";
            };
        }

#if NETCOREAPP3_1
        private void ConfigureSocketsHttpHandler(string host, SocketsHttpHandler handler)
        {
            handler.SslOptions.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            handler.SslOptions.RemoteCertificateValidationCallback = (_, certificate, __, ___) =>
            {
                return certificate.Subject == $"CN={host}";
            };
        }
#endif
    }
}
