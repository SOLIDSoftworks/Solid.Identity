using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    public static class SignatureAlgorithms
    {
        public static readonly string RsaSha256Algorithm = SecurityAlgorithms.RsaSha256Signature;
        public static readonly string RsaSha384Algorithm = SecurityAlgorithms.RsaSha384Signature;
        public static readonly string RsaSha512Algorithm = SecurityAlgorithms.RsaSha512Signature;
        public static readonly SignatureAlgorithmName RsaSha256 = new SignatureAlgorithmName(RsaSha256Algorithm);
        public static readonly SignatureAlgorithmName RsaSha384 = new SignatureAlgorithmName(RsaSha384Algorithm);
        public static readonly SignatureAlgorithmName RsaSha512 = new SignatureAlgorithmName(RsaSha512Algorithm);
    }
}
