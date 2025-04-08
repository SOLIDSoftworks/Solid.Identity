using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Solid.IdentityModel.Xml
{
    internal static class SymmetricAlgorithmProvider
    {
        public static SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm, SymmetricSecurityKey key)
        {
            var symmetric = GetSymmetricAlgorithm(algorithm);
            symmetric.Key = key.Key;
            return symmetric;

        }
        public static SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm)
        {
            var symmetric = null as SymmetricAlgorithm;
            if (AesEncryption.Contains(algorithm))
                symmetric = Aes.Create();

            if (symmetric == null)
                throw new NotSupportedException($"Encryption algorithm '{algorithm}' not supported.");

            symmetric.Mode = GetCipherMode(algorithm);
            symmetric.Padding = PaddingMode.ISO10126;
            symmetric.KeySize = GetKeySize(algorithm);
            symmetric.GenerateKey();
            symmetric.GenerateIV();

            return symmetric;
        }

        public static CipherMode GetCipherMode(string algorithm)
        {
            switch (algorithm)
            {
                case SecurityAlgorithms.Aes128Encryption:
                case SecurityAlgorithms.Aes192Encryption:
                case SecurityAlgorithms.Aes256Encryption:
                    return CipherMode.CBC;
            }

            throw new NotSupportedException($"Encryption algorithm '{algorithm}' not supported.");
        }

        static int GetKeySize(string algorithm)
        {
            if (algorithm == SecurityAlgorithms.Aes128Encryption) return 128;
            if (algorithm == SecurityAlgorithms.Aes192Encryption) return 192;
            if (algorithm == SecurityAlgorithms.Aes256Encryption) return 256;

            throw new NotSupportedException($"Encryption algorithm '{algorithm}' not supported.");
        }

        static readonly IReadOnlyCollection<string> AesEncryption = new List<string>
        {
            SecurityAlgorithms.Aes128Encryption,
            SecurityAlgorithms.Aes192Encryption,
            SecurityAlgorithms.Aes256Encryption
        }.AsReadOnly();
    }
}
