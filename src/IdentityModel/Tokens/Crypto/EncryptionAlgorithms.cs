using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto
{
    public static class EncryptionAlgorithms
    {
        public static readonly string Aes128CbcAlgorithm = SecurityAlgorithms.Aes128Encryption;
        public static readonly string Aes192CbcAlgorithm = SecurityAlgorithms.Aes192Encryption;
        public static readonly string Aes256CbcAlgorithm = SecurityAlgorithms.Aes256Encryption;

        public static readonly EncryptionAlgorithmName Aes128Cbc = new EncryptionAlgorithmName(Aes128CbcAlgorithm);
        public static readonly EncryptionAlgorithmName Aes192Cbc = new EncryptionAlgorithmName(Aes192CbcAlgorithm);
        public static readonly EncryptionAlgorithmName Aes256Cbc = new EncryptionAlgorithmName(Aes256CbcAlgorithm);
    }
}
