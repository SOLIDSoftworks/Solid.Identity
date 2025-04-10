using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Solid.Testing.Certificates;

public static class CertificateStore
{
    private static ConcurrentDictionary<CertificateDescriptor, X509Certificate2> _store = new ();
    
    public static X509Certificate2 GetOrCreate(CertificateDescriptor descriptor)
        => _store.GetOrAdd(descriptor, Create);

    public static X509Certificate2 Create(CertificateDescriptor descriptor)
    {
        var name = descriptor.CommonName ?? Guid.NewGuid().ToString("N");
        var size = descriptor.KeySize ?? RsaKeySize.Medium;
        var builder = new SubjectAlternativeNameBuilder();
        builder.AddDnsName(name);
        foreach (var alternative in descriptor.AlternativeNames)
        {
            if(IPAddress.TryParse(alternative, out var address))
                builder.AddIpAddress(address);
            else if(Uri.TryCreate(alternative, UriKind.Absolute, out var uri))
                builder.AddUri(uri);
            else if(MailAddress.TryCreate(alternative, out var mailAddress))
                builder.AddEmailAddress(mailAddress.Address);
            else
                builder.AddDnsName(alternative);
        }

        var dn = new X500DistinguishedName($"CN={name}");
        using var rsa = RSA.Create((int)size);
        var request = new CertificateRequest(dn, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var flags = descriptor.KeyUsageFlags.Aggregate((X509KeyUsageFlags)0, (current, flag) => current | flag);
        var usage = new X509KeyUsageExtension(flags, true);
        request.CertificateExtensions.Add(usage);

        var oids = new OidCollection();
        foreach (var oid in descriptor.Oids)
            oids.Add(oid);

        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(oids, false));

        request.CertificateExtensions.Add(builder.Build());

        var certificate = request.CreateSelfSigned(
            new DateTimeOffset(descriptor.NotBefore ?? DateTime.UtcNow.AddMinutes(-5)), 
            new DateTimeOffset(descriptor.NotAfter ?? DateTime.UtcNow.AddMinutes(10)));
        return certificate;
    }
}