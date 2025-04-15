using System;
using System.Collections.Concurrent;
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
        public const string ValidName = "test.valid";
        public const string InvalidName = "test.invalid";
        public const string ExpiredName = "test.expired"; 
        public const string RelyingPartyValidName = "test-relyingparty.valid"; 
        public const string ClientCertificateName = "wstrust.tests.client";

        public static CertificateDescriptor GetCertificateDescriptor(string name)
            => name switch
            {
                ValidName => Valid,
                InvalidName => Invalid,
                ExpiredName => Expired,
                RelyingPartyValidName => RelyingPartyValid,
                ClientCertificateName => ClientCertificate,
                _ => throw new ArgumentException()
            };
        
        public static CertificateDescriptor Valid { get; } = CertificateDescriptor.Create(name: ValidName);
        public static CertificateDescriptor Invalid { get; } = CertificateDescriptor.Create(name: InvalidName);
        public static CertificateDescriptor Expired { get; } = new ()
        { 
            CommonName = ExpiredName, 
            Oids =
            {
                Oids.ClientAuthentication,
                Oids.ServerAuthentication
            },
            NotBefore = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)),
            NotAfter = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))
        };

        public static CertificateDescriptor RelyingPartyValid { get; } = CertificateDescriptor.Create(name: RelyingPartyValidName);

        public static CertificateDescriptor ClientCertificate { get; } = CertificateDescriptor.Create(name: ClientCertificateName);
    }
}
