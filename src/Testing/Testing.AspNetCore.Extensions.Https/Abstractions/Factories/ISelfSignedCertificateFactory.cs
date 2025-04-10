using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Extensions.Https.Abstractions
{
    internal interface ISelfSignedCertificateFactory
    {
        X509Certificate2 GenerateCertificate(string hostname);
    }
}
