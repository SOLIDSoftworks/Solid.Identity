using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.IdentityModel.Tokens
{
    public static class TokenValidationParametersExtensions
    {
        public static IEnumerable<SecurityKey> ResolveDecryptionKeys(this TokenValidationParameters parameters, string token = null, SecurityToken securityToken = null, string kid = null)
        {
            if (parameters == null) yield break;

            if (parameters.TokenDecryptionKey != null) yield return parameters.TokenDecryptionKey;
            if (parameters.TokenDecryptionKeys != null)
            {
                foreach (var key in parameters.TokenDecryptionKeys)
                    yield return key;
            }
            if (parameters.TokenDecryptionKeyResolver != null)
            {
                foreach (var key in parameters.TokenDecryptionKeyResolver(token, securityToken, kid, parameters))
                    yield return key;
            }
        }
    }
}
