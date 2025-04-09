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

namespace Solid.Testing.AspNetCore.Extensions.Https.Factories
{
    internal class SelfSignedCertificateFactory : ISelfSignedCertificateFactory
    {
        public X509Certificate2 GenerateCertificate(string hostname)
        {
            var builder = new SubjectAlternativeNameBuilder();
            builder.AddIpAddress(IPAddress.Loopback);
            builder.AddIpAddress(IPAddress.IPv6Loopback);
            builder.AddDnsName(hostname);

            var dn = new X500DistinguishedName($"CN={hostname}");
            using (var rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(dn, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                var usage = new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyAgreement, true);
                request.CertificateExtensions.Add(usage);

                var oids = new OidCollection
                {
                    new Oid("1.3.6.1.5.5.7.3.1"), // Server authentication
                    new Oid("1.3.6.1.5.5.7.3.2") // Client authentication
                };

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(oids, false));

                request.CertificateExtensions.Add(builder.Build());

                var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(1)));
                
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    certificate.FriendlyName = "Solid.Testing.AspNetCore";
                
                var bytes = certificate.Export(X509ContentType.Pfx);
                return new X509Certificate2(bytes, null as string, X509KeyStorageFlags.Exportable);
            }
        }
    }
}