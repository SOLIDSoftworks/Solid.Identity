using Microsoft.IdentityModel.Tokens;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// This class are used in defining Entropy and RequestProofToken element inside the 
    /// RequestSecurityToken and RequestSecurityTokenResponse.
    /// </summary>
    public class ProtectedKey
    {        
        /// <summary>
        /// Use this constructor if we want to send the key material encrypted.
        /// </summary>
        /// <param name="secret">The key material that needs to be protected.</param>
        /// <param name="wrappingCredentials">The encrypting credentials used to encrypt the key material.</param>
        public ProtectedKey(byte[] secret, EncryptingCredentials wrappingCredentials)
        {
            Secret = secret;
            WrappingCredentials = wrappingCredentials;
        }

        /// <summary>
        /// Gets the key material.
        /// </summary>
        public byte[] Secret { get; }

        /// <summary>
        /// Gets the encrypting credentials. Null means that the keys are not encrypted.
        /// </summary>
        public EncryptingCredentials WrappingCredentials { get; }
    }
}

