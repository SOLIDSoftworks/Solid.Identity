using Solid.Testing.AspNetCore.Abstractions;
using Solid.Testing.AspNetCore.Extensions.Https.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Solid.Testing.Certificates;

namespace Solid.Testing.AspNetCore.Extensions.Https.Factories
{
    internal class SelfSignedCertificateFactory : ISelfSignedCertificateFactory
    {
        public X509Certificate2 GenerateCertificate(string hostname)
        {
            var descriptor = new CertificateDescriptor
            {
                CommonName = hostname,
                NotBefore = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                NotAfter = DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                AlternativeNames =
                {
                    "127.0.0.1",
                    "::1"
                },
                Oids =
                {
                    Oids.ServerAuthentication,
                    Oids.ClientAuthentication
                }
            };

            var certificate = CertificateStore.GetOrCreate(descriptor);
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                certificate.FriendlyName = "Solid.Testing.AspNetCore";
            return certificate; 
        }
    }
}