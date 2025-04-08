using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.IdentityModel.Tokens
{
    public static class CryptoProviderFactoryExtensions
    {
        private static readonly string IDX10640 = "IDX10640: Algorithm is not supported: '{0}'.";
        private static readonly string IDX10647 = "IDX10647: A CustomCryptoProvider was set and returned 'true' for IsSupportedAlgorithm(Algorithm: '{0}'), but Create.(algorithm, args) as '{1}' == NULL.";

        private static readonly string InvalidSecurityKey = "The SecurityKey is not or cannot be converted to a SymmetricSecuritKey. SecurityKey: '{0}'.";

        public static SymmetricAlgorithm CreateSymmetricAlgorithm(this CryptoProviderFactory crypto, SecurityKey securityKey, string algorithm)
        {
            if (securityKey == null)
                throw LogHelper.LogArgumentNullException(nameof(securityKey));

            if (!(securityKey is SymmetricSecurityKey symmetricKey))
                throw LogHelper.LogExceptionMessage(new InvalidOperationException(LogHelper.FormatInvariant(InvalidSecurityKey, securityKey.GetType().Name)));

            var symmetric = crypto.CreateSymmetricAlgorithm(algorithm);
            symmetric.Key = symmetricKey.Key;
            return symmetric;
        }

        public static SymmetricAlgorithm CreateSymmetricAlgorithm(this CryptoProviderFactory crypto, string algorithm)
        {
            if (crypto.CustomCryptoProvider != null && crypto.CustomCryptoProvider.IsSupportedAlgorithm(algorithm))
            {
                if (!(crypto?.CustomCryptoProvider.Create(algorithm) is SymmetricAlgorithm symmetric))
                    throw LogHelper.LogExceptionMessage(new InvalidOperationException(LogHelper.FormatInvariant(IDX10647, algorithm, typeof(SymmetricAlgorithm))));

                return symmetric;
            }
            throw LogHelper.LogExceptionMessage(new NotSupportedException(LogHelper.FormatInvariant(IDX10640, algorithm)));
        }

        public static void ReleaseSymmetricAlgorithm(this CryptoProviderFactory _, SymmetricAlgorithm symmetricAlgorithm)
        {
            if (symmetricAlgorithm == null)
                throw LogHelper.LogArgumentNullException(nameof(symmetricAlgorithm));
            
            symmetricAlgorithm.Dispose();
        }
    }
}
