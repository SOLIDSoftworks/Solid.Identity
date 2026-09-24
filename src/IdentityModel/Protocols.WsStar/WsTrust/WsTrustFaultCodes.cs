#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// Provides string values for WsTrust fault codes.
    /// <para>Fault codes values for WsTrust Feb2005, 1.3 and 1.4 are the same.</para>
    /// </summary>
    public static class WsTrustFaultCodes
    {
        /// <summary>
        /// Gets the 'FailedAuthentication' fault code.
        /// </summary>
        public const string FailedAuthentication = "FailedAuthentication";

        /// <summary>
        /// Gets the 'FailedCheck' fault code.
        /// </summary>
        public const string FailedCheck = "FailedCheck";

        /// <summary>
        /// Gets the 'InvalidSecurity' fault code.
        /// </summary>
        public const string InvalidSecurity = "InvalidSecurity";

        /// <summary>
        /// Gets the 'InvalidSecurityToken' fault code.
        /// </summary>
        public const string InvalidSecurityToken = "InvalidSecurityToken";

        /// <summary>
        /// Gets the 'MessageExpired' fault code.
        /// </summary>
        public const string MessageExpired = "MessageExpired";

        /// <summary>
        /// Gets the 'SecurityTokenUnavailable' fault code.
        /// </summary>
        public const string SecurityTokenUnavailable = "SecurityTokenUnavailable";

        /// <summary>
        /// Gets the 'UnsupportedAlgorithm' fault code.
        /// </summary>
        public const string UnsupportedAlgorithm = "UnsupportedAlgorithm";

        /// <summary>
        /// Gets the 'UnsupportedSecurityToken' fault code.
        /// </summary>
        public const string UnsupportedSecurityToken = "UnsupportedSecurityToken";
    }
}
