using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Xml;
using LegacySecurityToken = System.IdentityModel.Tokens.SecurityToken;
using LegacyBinarySecretSecurityToken = System.ServiceModel.Security.Tokens.BinarySecretSecurityToken;
using LegacyGenericXmlSecurityToken = System.IdentityModel.Tokens.GenericXmlSecurityToken;
using LegacyGenericXmlSecurityKeyIdentifierClause = System.IdentityModel.Tokens.GenericXmlSecurityKeyIdentifierClause;
using LegacySecurityKeyIdentifierClause = System.IdentityModel.Tokens.SecurityKeyIdentifierClause;
using SecurityKey = Microsoft.IdentityModel.Tokens.SecurityKey;
using SymmetricSecurityKey = Microsoft.IdentityModel.Tokens.SymmetricSecurityKey;
using SecurityToken = Microsoft.IdentityModel.Tokens.SecurityToken;
using GenericXmlSecurityToken = Solid.IdentityModel.Tokens.Xml.GenericXmlSecurityToken;
using SecurityTokenHandler = Microsoft.IdentityModel.Tokens.SecurityTokenHandler;
using System.Linq;
using System.IdentityModel.Policy;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Reflection;

namespace Solid.ServiceModel
{
    internal class IssuedTokenProvider : SecurityTokenProvider
    {
        private ExtendedClientCredentials _credentials;
        private ILogger _logger;
        private static readonly ConstructorInfo _binarySecretSecurityTokenConstructor;

        static IssuedTokenProvider()
        {
            var assembly = typeof(LegacySecurityToken).Assembly;
            var binarySecretSecurityTokenType = assembly.GetTypes().FirstOrDefault(t => t.Name == "BinarySecretSecurityToken");
            var binarySecretSecurityTokenConstructor = binarySecretSecurityTokenType?.GetConstructors().FirstOrDefault(c =>
            {
                var parameters = c.GetParameters();
                if (parameters.Length != 1) return false;
                return parameters[0].ParameterType == typeof(byte[]);
            });

            _binarySecretSecurityTokenConstructor = binarySecretSecurityTokenConstructor;
        }

        public IssuedTokenProvider(ExtendedClientCredentials credentials, ILoggerFactory loggerFactory)
        {
            _credentials = credentials;
            _logger = loggerFactory?.CreateLogger<IssuedTokenProvider>(); 
        }

        protected override LegacySecurityToken GetTokenCore(TimeSpan timeout)
        {
            var token = _credentials.IssuedToken.Token;
            LogMessages.AddingSecurityTokenToRequest(_logger, token);
            foreach (var handler in _credentials.IssuedToken.Handlers)
            {
                // this if statement is failing because of a bad default implementation of SecurityTokenHandler.CanWriteSecurityToken
                // waiting on PR https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/pull/1438
                //if (!handler.CanWriteSecurityToken(token)) continue;

                // TODO: replace the following if statement with the one in the above comment if/when the PR has been accepted
                if (!handler.TokenType.IsAssignableFrom(token.GetType()) || !handler.CanWriteToken) continue;
                LogMessages.FoundSecurityTokenHandler(_logger, handler);
                var converted = ConvertToGenericXmlSecurityToken(token, handler);
                return converted;
            }

            throw new Exception($"Cannot write token type: {token.GetType().Name}");
        } 

        private LegacyGenericXmlSecurityToken ConvertToGenericXmlSecurityToken(SecurityToken token, SecurityTokenHandler handler)
        {
            var element = XmlHelper.CreateElement(writer => handler.WriteToken(writer, token));
            
            var proof = null as LegacySecurityToken;
            var internalIdentifierClause = null as LegacySecurityKeyIdentifierClause;
            var externalIdentifierClause = null as LegacySecurityKeyIdentifierClause;
            var policy = new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy>());

            if (token.SecurityKey != null)
                proof = ConvertToProofSecurityToken(token.SecurityKey);
            
            if (token is GenericXmlSecurityToken generic)
            {
                if (generic.InternalTokenReference != null)
                    internalIdentifierClause = new LegacyGenericXmlSecurityKeyIdentifierClause(generic.InternalTokenReference.Element)
                    {
                        Id = generic.InternalTokenReference.Id
                    };

                if (generic.ExternalTokenReference != null)
                    externalIdentifierClause = new LegacyGenericXmlSecurityKeyIdentifierClause(generic.ExternalTokenReference.Element)
                    {
                        Id = generic.ExternalTokenReference.Id
                    };
            }
            return new LegacyGenericXmlSecurityToken(element, proof, token.ValidFrom, token.ValidTo, internalIdentifierClause, externalIdentifierClause, policy);
        }

        private LegacySecurityToken ConvertToProofSecurityToken(SecurityKey key)
        {
            if (key is SymmetricSecurityKey symmetric)
                return _binarySecretSecurityTokenConstructor.Invoke(new object[] { symmetric.Key }) as LegacySecurityToken;
                //return new LegacyBinarySecretSecurityToken(symmetric.Key);

            throw new NotSupportedException($"Key type not supported: {key.GetType().Name}");
        }
    }
}
