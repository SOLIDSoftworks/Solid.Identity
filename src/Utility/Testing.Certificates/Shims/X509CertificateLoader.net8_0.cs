#if NET8_0
namespace System.Security.Cryptography.X509Certificates;

public static class X509CertificateLoader
{
    public static X509Certificate2 LoadCertificate(byte[] bytes)
        => new (bytes, null as string, X509KeyStorageFlags.Exportable);
}
#endif