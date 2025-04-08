using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Solid.IdentityModel.Tokens.Saml2
{
    public class Saml2EncryptedSecurityToken : Saml2SecurityToken
    {
        public Saml2EncryptedSecurityToken(Saml2SecurityToken unencrypted, EncryptedData encryptedData)
            : this(unencrypted.Assertion, encryptedData)
        {
        }

        public Saml2EncryptedSecurityToken(EncryptedData encryptedData)
            : this(CreatePlaceholderAssertion(), encryptedData)
        {
        }

        public Saml2EncryptedSecurityToken(Saml2SecurityToken unencrypted, EncryptingCredentials encryptingCredentials)
            : base(unencrypted.Assertion)
        {
            EncryptingCredentials = encryptingCredentials;
        }

        private Saml2EncryptedSecurityToken(Saml2Assertion assertion, EncryptedData encryptedData)
            : base(assertion)
        {
            EncryptedData = encryptedData;
        }

        public EncryptedData EncryptedData { get; }
        public EncryptingCredentials EncryptingCredentials { get; }

        private static Saml2Assertion CreatePlaceholderAssertion()
        {
            var issuer = new Saml2NameIdentifier("__encrypted__");
            var assertion = new Saml2Assertion(issuer);
            return assertion;
        }
    }
}
