using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Saml2
{
    public class Saml2Options
    {
        public SecurityKey DefaultDecryptionKey { get; set; }
        public bool UseSecurityTokenReference { get; set; } = true;
        public Func<SecurityKey, string> GenerateId { get; set; } = key =>
        {
            var id = key?.KeyId;
            if(string.IsNullOrEmpty(id))
            {
                if (key?.CanComputeJwkThumbprint() != true)
                    throw new NotSupportedException("Unable to generate an id for security key.");
                id = Base64UrlEncoder.Encode(key.ComputeJwkThumbprint());
            }
            return id;
        };

        public IDictionary<string, Uri> AuthenticationMethodMap { get; } = new Dictionary<string, Uri>();
    }
}
