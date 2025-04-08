using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    public static class DigestAlgorithms
    {
        public static readonly string Sha256Algorithm = SecurityAlgorithms.Sha256Digest;
        public static readonly string Sha384Algorithm = SecurityAlgorithms.Sha384Digest;
        public static readonly string Sha512Algorithm = SecurityAlgorithms.Sha512Digest;
        public static readonly DigestAlgorithmName Sha256 = new DigestAlgorithmName(Sha256Algorithm);
        public static readonly DigestAlgorithmName Sha384 = new DigestAlgorithmName(Sha384Algorithm);
        public static readonly DigestAlgorithmName Sha512 = new DigestAlgorithmName(Sha512Algorithm);
    }
}
