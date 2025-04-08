using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens
{
    public class SignatureMethod
    {
        public string SignatureAlgortihm { get; }
        public string DigestAlgorithm { get; }

        public SignatureMethod(SignatureAlgorithmName signatureAlgorithmName, DigestAlgorithmName digestAlgorithmName)
            : this(signatureAlgorithmName)
        {
            DigestAlgorithm = digestAlgorithmName.Algorithm;
        }

        public SignatureMethod(SignatureAlgorithmName signatureAlgorithmName)
        {
            SignatureAlgortihm = signatureAlgorithmName.Algorithm;
        }

        public SigningCredentials CreateCredentials(SecurityKey key)
        {
            if (DigestAlgorithm != null)
                return new SigningCredentials(key, SignatureAlgortihm, DigestAlgorithm);
            return new SigningCredentials(key, SignatureAlgortihm);
        }

        public static readonly SignatureMethod RsaSha256 = new SignatureMethod(SignatureAlgorithms.RsaSha256, DigestAlgorithms.Sha256);
        public static readonly SignatureMethod RsaSha384 = new SignatureMethod(SignatureAlgorithms.RsaSha384, DigestAlgorithms.Sha384);
        public static readonly SignatureMethod RsaSha512 = new SignatureMethod(SignatureAlgorithms.RsaSha512, DigestAlgorithms.Sha512);
    }
}
