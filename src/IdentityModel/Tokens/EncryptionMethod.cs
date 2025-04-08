using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens
{
    public class EncryptionMethod
    {
        public EncryptionMethod(EncryptionAlgorithmName encryptionAlgorithmName, KeyWrapAlgorithmName keyWrapAlgorithmName)
            : this(encryptionAlgorithmName)
        {
            KeyWrapAlgorithm = keyWrapAlgorithmName.Algorithm;
        }

        public EncryptionMethod(EncryptionAlgorithmName encryptionAlgorithmName)
        {
            EncryptionAlgorithm = encryptionAlgorithmName.Algorithm;
        }

        public string KeyWrapAlgorithm { get; }
        public string EncryptionAlgorithm { get; }

        public EncryptingCredentials CreateCredentials(SecurityKey key)
        {
            if (KeyWrapAlgorithm == null)
            {
                if (!(key is SymmetricSecurityKey symmetric))
                    throw new ArgumentException("Symmetric key required when not using key wrap.");
                return new EncryptingCredentials(symmetric, EncryptionAlgorithm);
            }
            return new EncryptingCredentials(key, KeyWrapAlgorithm, EncryptionAlgorithm);
        }

        public static EncryptionMethod Aes128CbcWithRsaOaepKeyWrap = new EncryptionMethod(EncryptionAlgorithms.Aes128Cbc, KeyWrapAlgorithms.RsaOaepMgf1p);
        public static EncryptionMethod Aes192CbcWithRsaOaepKeyWrap = new EncryptionMethod(EncryptionAlgorithms.Aes192Cbc, KeyWrapAlgorithms.RsaOaepMgf1p);
        public static EncryptionMethod Aes256CbcWithRsaOaepKeyWrap = new EncryptionMethod(EncryptionAlgorithms.Aes256Cbc, KeyWrapAlgorithms.RsaOaepMgf1p);
    }
}
