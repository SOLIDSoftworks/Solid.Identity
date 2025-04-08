using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// The Entropy used in both token request message and token response message. 
    /// </summary>
    public class Entropy
    {
        /// <summary>
        /// 
        /// </summary>
        internal Entropy()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binarySecret"></param>
        public Entropy(BinarySecret binarySecret)
        {
            BinarySecret = binarySecret;
        }

        /// <summary>
        /// 
        /// </summary>
        public BinarySecret BinarySecret { get; internal set; }

        /// <summary>
        /// Constructs an entropy instance with the protected key.
        /// </summary>
        /// <param name="protectedKey">The protected key which can be either binary secret or encrypted key.</param>
        public Entropy( ProtectedKey protectedKey )
        {
            ProtectedKey = protectedKey ?? throw LogHelper.LogArgumentNullException(nameof(protectedKey));
        }

        /// <summary>
        /// Get the <see cref="ProtectedKey"/>
        /// </summary>
        public ProtectedKey ProtectedKey { get; }

        static byte[] GetKeyBytesFromProtectedKey( ProtectedKey protectedKey )
        {
            if (protectedKey == null)
                LogHelper.LogArgumentNullException(nameof(protectedKey));

            return protectedKey.Secret;
        }

        static EncryptingCredentials GetWrappingCredentialsFromProtectedKey( ProtectedKey protectedKey )
        {
            if (protectedKey == null)
                LogHelper.LogArgumentNullException(nameof(protectedKey));

            return protectedKey.WrappingCredentials;
        }
    }
}
