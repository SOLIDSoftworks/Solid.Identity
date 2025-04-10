using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Solid.Testing.Certificates;

public sealed class CertificateDescriptor : IEquatable<CertificateDescriptor>
{
    public string? CommonName { get; init; }
    public DateTime? NotBefore { get; init; }
    public DateTime? NotAfter { get; init; }
    public RsaKeySize? KeySize { get; init; }
    public List<Oid> Oids { get; } = new ();
    public List<string> AlternativeNames { get; } = new ();

    public List<X509KeyUsageFlags> KeyUsageFlags { get; } = new ()
    {
        X509KeyUsageFlags.DataEncipherment,
        X509KeyUsageFlags.KeyEncipherment,
        X509KeyUsageFlags.DigitalSignature,
        X509KeyUsageFlags.KeyAgreement
    };

    public static CertificateDescriptor Create(bool addServerAuthentication = true, bool addClientAuthentication = true)
    {
        var descriptor = new CertificateDescriptor();
        if(addClientAuthentication)
            descriptor.Oids.Add(Certificates.Oids.ClientAuthentication);
        if(addServerAuthentication)
            descriptor.Oids.Add(Certificates.Oids.ServerAuthentication);
        return descriptor;
    }

    public override int GetHashCode()
        => HashCode.Combine(CommonName, NotBefore, NotAfter, KeySize, AlternativeNames, Oids, KeyUsageFlags);

    public bool Equals(CertificateDescriptor? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return CommonName == other.CommonName && 
               Nullable.Equals(NotBefore, other.NotBefore) && 
               Nullable.Equals(NotAfter, other.NotAfter) && 
               KeySize == other.KeySize && 
               Equals(AlternativeNames, other.AlternativeNames) &&
               Equals(Oids, other.Oids) && 
               Equals(KeyUsageFlags, other.KeyUsageFlags);
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is CertificateDescriptor other && Equals(other);
    
    public static bool operator ==(CertificateDescriptor? left, CertificateDescriptor? right)
    {
        // Handle null on both sides or one side
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(CertificateDescriptor? left, CertificateDescriptor? right)
    {
        return !(left == right);
    }
}