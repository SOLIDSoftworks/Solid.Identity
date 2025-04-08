using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Tokens.Crypto;
using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using static Microsoft.IdentityModel.Tokens.Saml.SamlConstants;

namespace Solid.IdentityModel.Tokens.Saml2
{
    public class ExtendedSaml2SecurityTokenHandler : Saml2SecurityTokenHandler, IDisposable
    {
        protected Encoding Utf8 { get; } = new UTF8Encoding(false);

        private IDisposable _optionsChangeToken;
        protected Saml2Options Options { get; private set; }

        public ExtendedSaml2SecurityTokenHandler(ExtendedSaml2Serializer serializer, Saml2Options options)
            : base()
        {
            Serializer = serializer;
            Options = options;
        }

        public ExtendedSaml2SecurityTokenHandler()
            : this(new ExtendedSaml2Serializer(), new Saml2Options())
        {
        }

        public ExtendedSaml2SecurityTokenHandler(IOptionsMonitor<Saml2Options> monitor, ExtendedSaml2Serializer serializer = null)
        {
            Options = monitor.CurrentValue;
            _optionsChangeToken = monitor.OnChange((options, _) => Options = options);
            Serializer = serializer ?? new ExtendedSaml2Serializer();
        }

        public void Dispose() => _optionsChangeToken?.Dispose();

        public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor)
        {
            var information = CreateAuthenticationInformation(tokenDescriptor);
            return CreateToken(tokenDescriptor, information);
        }

        public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor, AuthenticationInformation authenticationInformation)
        {
            var saml2 = base.CreateToken(tokenDescriptor, authenticationInformation) as Saml2SecurityToken;

            var authnStatement = saml2.Assertion.Statements.OfType<Saml2AuthenticationStatement>().FirstOrDefault();
            if (authnStatement != null)
                authnStatement.SessionIndex = saml2.Id;

            return saml2;
        }

        protected override Saml2Subject CreateSubject(SecurityTokenDescriptor tokenDescriptor)
        {
            var subject = base.CreateSubject(tokenDescriptor);

            if (subject.NameId != null && subject.NameId.Format == null)
                subject.NameId.Format = Saml2Constants.NameIdentifierFormats.Unspecified;

            if (tokenDescriptor is RequestedSecurityTokenDescriptor requestedTokenDescriptor)
            {
                var keyInfo = CreateProofKeyInfo(requestedTokenDescriptor);
                if (keyInfo != null)
                {
                    var data = new Saml2SubjectConfirmationData
                    {
                    };
                    data.KeyInfos.Add(keyInfo);
                    var holderOfKey = new Saml2SubjectConfirmation(Saml2Constants.ConfirmationMethods.HolderOfKey, data);

                    var bearer = subject.SubjectConfirmations.FirstOrDefault(c => c.Method == Saml2Constants.ConfirmationMethods.Bearer);
                    if (bearer != null)
                        subject.SubjectConfirmations.Remove(bearer);

                    subject.SubjectConfirmations.Add(holderOfKey);
                }
            }

            return subject;
        }

        protected virtual KeyInfo CreateProofKeyInfo(RequestedSecurityTokenDescriptor tokenDescriptor)
        {
            if (tokenDescriptor.ProofKey == null) return null;

            if (tokenDescriptor.ProofKey is SymmetricSecurityKey symmetric)
            {
                var credentials = tokenDescriptor.ProofKeyEncryptingCredentials;
                if (credentials == null)
                    return new BinarySecretKeyInfo { Key = symmetric.Key };

                var crypto = credentials.CryptoProviderFactory ?? credentials.Key.CryptoProviderFactory ?? CryptoProviderFactory.Default;
                var keyWrap = null as KeyWrapProvider;
                try
                {
                    keyWrap = crypto.CreateKeyWrapProvider(credentials.Key, credentials.Alg);
                    var wrapped = keyWrap.WrapKey(symmetric.Key);
                    var cipherValue = wrapped;
                    return new EncryptedKeyInfo
                    {
                        CipherValue = cipherValue,
                        EncryptionMethod = credentials.Alg,
                        KeyInfo = CreateKeyInfo(credentials.Key)
                    };
                }
                finally
                {
                    if(keyWrap != null)
                        crypto?.ReleaseKeyWrapProvider(keyWrap);
                }
            }
            else if (tokenDescriptor.ProofKey is AsymmetricSecurityKey)
            {
                throw new NotSupportedException($"Asymmetric proof keys not supported for now.");
            }
            else
            {
                throw new NotSupportedException($"Key type '{tokenDescriptor.ProofKey.GetType().Name}' not supported.");
            }
        }

        protected virtual KeyInfo CreateKeyInfo(SecurityKey key)
        {
            if (key is X509SecurityKey x509)
            {
                if (Options.UseSecurityTokenReference)
                {
                    var publicKey = x509.Certificate.Export(X509ContentType.Cert);
                    using (var sha1 = SHA1.Create())
                    {
                        var hashed = sha1.ComputeHash(publicKey);
                        return new SecurityTokenReferenceKeyInfo
                        {
                            KeyIdValueType = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#ThumbprintSHA1",
                            KeyId = Convert.ToBase64String(hashed)
                        };
                    }
                }

                return new KeyInfo
                {
                    X509Data =
                    {
                        new X509Data(x509.Certificate)
                    }
                };
            }
            else if (key is RsaSecurityKey rsa)
            {
                var id = Options.GenerateId(rsa);
                return new KeyInfo { KeyName = id };
            }

            throw new NotSupportedException($"Key type '{key.GetType().Name}' not supported.");
        }

        protected virtual TokenValidationParameters CreateDefaultTokenValidationParameters()
        {
            if (Options.DefaultDecryptionKey == null) return null;
            return new TokenValidationParameters
            {
                TokenDecryptionKey = Options.DefaultDecryptionKey
            };
        }

        protected virtual AuthenticationInformation CreateAuthenticationInformation(SecurityTokenDescriptor tokenDescriptor)
        {
            var user = tokenDescriptor.Subject;
            var authenticationMethod = user.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            var authenticationInstant = user.FindFirst(ClaimTypes.AuthenticationInstant)?.Value;

            if (authenticationInstant == null) return null;
            if (!DateTime.TryParse(authenticationInstant, out var date)) return null;

            var authenticationContext = CreateAuthenticationContext(authenticationMethod);
            if (authenticationContext == null) return null;

            return new AuthenticationInformation(authenticationContext, date);
        }

        protected virtual Uri CreateAuthenticationContext(string authenticationMethod)
        {
            if (!string.IsNullOrWhiteSpace(authenticationMethod))
            {
                if (Options.AuthenticationMethodMap.TryGetValue(authenticationMethod, out var context))
                    return context;
                if (Uri.IsWellFormedUriString(authenticationMethod, UriKind.Absolute))
                    return new Uri(authenticationMethod);
            }
            return new Uri(AuthenticationContextClasses.Unspecified);
        }
    }
}
