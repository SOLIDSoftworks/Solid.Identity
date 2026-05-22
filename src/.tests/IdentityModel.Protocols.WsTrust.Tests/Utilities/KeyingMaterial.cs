using System;
using System.Collections.Concurrent;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using Solid.Testing.Certificates;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests.Utilities;

public static class KeyingMaterial
{
    private static readonly ConcurrentDictionary<(bool SelfSigned, RsaKeySize KeySize, X509ContentType ContentType), byte[]> _cache = new();
    public static string SelfSigned2048_SHA256 => Convert.ToBase64String(GetKeyingMaterialData(selfSigned: true, keySize: RsaKeySize.Medium));
    public static readonly byte[] SharedKey = Guid.NewGuid().ToByteArray();

    public static X509Certificate2 GetKeyingMaterial(bool selfSigned = true, RsaKeySize keySize = RsaKeySize.Medium, X509ContentType contentType = X509ContentType.Pfx)
        => X509CertificateLoader.LoadCertificate(GetKeyingMaterialData(selfSigned, keySize, contentType));

    public static byte[] GetKeyingMaterialData(bool selfSigned = true, RsaKeySize keySize = RsaKeySize.Medium, X509ContentType contentType = X509ContentType.Pfx)
        => _cache.GetOrAdd((selfSigned, keySize, contentType), CreateCertificate);

    private static byte[] CreateCertificate((bool SelfSigned, RsaKeySize KeySize, X509ContentType ContentType) key)
    {
        if (!key.SelfSigned)
            throw new NotSupportedException("Not supported yet");

        if (key.ContentType == X509ContentType.Cert)
        {
            var c = GetKeyingMaterial(key.SelfSigned, key.KeySize, X509ContentType.Pfx);
            return c.Export(X509ContentType.Cert);
        }
        
        var descriptor = new CertificateDescriptor
        {
            KeySize = key.KeySize,
            CommonName = Guid.NewGuid().ToString(),
            KeyUsageFlags =
            {
                X509KeyUsageFlags.KeyEncipherment,
                X509KeyUsageFlags.DigitalSignature
            },
            NotBefore = DateTime.UtcNow,
            NotAfter = DateTime.UtcNow.AddHours(1),
            Oids =
            {
                Oids.ClientAuthentication,
                Oids.ServerAuthentication,
            }
        };

        var certificate = CertificateStore.Create(descriptor);
        return certificate.Export(key.ContentType);
    }
}