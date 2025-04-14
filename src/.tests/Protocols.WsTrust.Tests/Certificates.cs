using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Solid.Testing.Certificates;

namespace Solid.Identity.Protocols.WsTrust.Tests
{
    public static class Certificates
    {
        public static CertificateDescriptor Valid { get; } = CertificateDescriptor.Create(name: "test.valid");
        public static CertificateDescriptor Invalid { get; } = CertificateDescriptor.Create(name: "test.invalid");
        public static CertificateDescriptor Expired { get; } = new ()
        { 
            CommonName = "test.expired", 
            Oids =
            {
                Oids.ClientAuthentication,
                Oids.ServerAuthentication
            },
            NotBefore = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)),
            NotAfter = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))
        };

        public static CertificateDescriptor RelyingPartyValid { get; } = CertificateDescriptor.Create(name: "test-relyingparty.valid");

        public static CertificateDescriptor ClientCertificate { get; } = CertificateDescriptor.Create(name: "wstrust.tests.client");
    }
}
