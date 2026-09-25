#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Elements for WS-Security 1.0 and 1.1.
    /// </summary>
    public static class WsSecurityElements
    {
        public static readonly string[] All =
        [
            BinarySecurityToken,
            Created,
            EncryptedHeader,
            KeyIdentifier,
            Nonce,
            Password,
            Reference,
            Security,
            SecurityTokenReference,
            Username,
            UsernameToken
        ];
        
        public const string BinarySecurityToken = "BinarySecurityToken";

        public const string Created = "Created";

        public const string EncryptedHeader = "EncryptedHeader";

        public const string KeyIdentifier = "KeyIdentifier";

        public const string Nonce = "Nonce";

        public const string Password = "Password";

        public const string Reference = "Reference";
        public const string Security = nameof(Security);

        public const string SecurityTokenReference = "SecurityTokenReference";

        public const string Username = "Username";

        public const string UsernameToken = "UsernameToken";
    }
}
