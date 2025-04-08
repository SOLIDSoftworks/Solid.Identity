using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Xml
{
    public class EncryptedKeyInfo : KeyInfo
    {
        public string EncryptionMethod { get; set; }
        public string DigestMethod { get; set; }
        public KeyInfo KeyInfo { get; set; }
        public byte[] CipherValue { get; set; }
        public byte[] Decrypt(SecurityKey decryptionKey)
        {
            var keyWrap = null as KeyWrapProvider;
            try
            {
                keyWrap = CryptoProviderFactory.Default.CreateKeyWrapProviderForUnwrap(decryptionKey, EncryptionMethod);
                return keyWrap.UnwrapKey(CipherValue);
            }
            finally
            {
                CryptoProviderFactory.Default.ReleaseKeyWrapProvider(keyWrap);
            }
        }
    }
}
