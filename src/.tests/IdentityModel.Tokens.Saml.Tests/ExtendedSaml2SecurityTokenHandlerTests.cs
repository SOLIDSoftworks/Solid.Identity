using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Solid.IdentityModel.Tokens.Saml.Tests
{
    public class ExtendedSaml2SecurityTokenHandlerTests : IClassFixture<Saml2TestFixture>
    {
        private Saml2TestFixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly Saml2Options _options;
        private readonly ExtendedSaml2SecurityTokenHandler _handler;

        static ExtendedSaml2SecurityTokenHandlerTests()
        {
            IdentityModelEventSource.ShowPII = true;

            var services = new ServiceCollection()
                .AddCustomCryptoProvider(options => options.AddFullSupport())
                .BuildServiceProvider()
            ;

            CryptoProviderFactory.Default = services.GetRequiredService<CryptoProviderFactory>();
        }

        public ExtendedSaml2SecurityTokenHandlerTests(Saml2TestFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
            _options = new Saml2Options();
            _handler = new ExtendedSaml2SecurityTokenHandler(new ExtendedSaml2Serializer(), _options);
        }

        [Fact]
        public void ShouldAddHolderOfKeyConfirmation()
        {
            var proof = new byte[32];
            RandomNumberGenerator.Fill(proof);
            var proofKey = new SymmetricSecurityKey(proof);

            var descriptor = _fixture.CreateDescriptor(proofKey: proofKey);
            descriptor.EncryptingCredentials = null;

            var token = _handler.CreateToken(descriptor);
            OutputToken(_handler, token);

            Assert.NotNull(token);
            Assert.IsAssignableFrom<Saml2SecurityToken>(token);

            _fixture.AssertContainsSymmetricKey(token, proofKey);
        }

        [Fact]
        public void ShouldWriteUnencryptedHolderOfKeyConfirmation()
        {
            var proof = new byte[32];
            RandomNumberGenerator.Fill(proof);
            var proofKey = new SymmetricSecurityKey(proof);

            var descriptor = _fixture.CreateDescriptor(proofKey: proofKey, encryptProofKey: false);
            descriptor.EncryptingCredentials = null;

            var token = _handler.CreateToken(descriptor);
            OutputToken(_handler, token);

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
                    _handler.WriteToken(writer, token);
                stream.Position = 0;

                var document = XDocument.Load(stream);
                var subjectConfirmation = document.Descendants(XName.Get(Saml2Constants.Elements.SubjectConfirmation, Saml2Constants.Namespace)).SingleOrDefault();
                var method = subjectConfirmation.Attribute(Saml2Constants.Attributes.Method);

                Assert.Equal(Saml2Constants.ConfirmationMethods.HolderOfKeyString, method.Value);

                var secret = subjectConfirmation
                    ?.Elements(XName.Get(Saml2Constants.Elements.SubjectConfirmationData, Saml2Constants.Namespace)).SingleOrDefault()
                    ?.Elements(XName.Get(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace)).SingleOrDefault()
                    ?.Elements(XName.Get(WsTrustElements.BinarySecret, WsTrustConstants.Trust13.Namespace)).SingleOrDefault();
                Assert.NotNull(secret);
                Assert.Equal(Convert.ToBase64String(proof), secret.Value);
            }
        }

        [Fact]
        public void ShouldWriteEncryptedHolderOfKeyConfirmation()
        {
            var proof = new byte[32];
            RandomNumberGenerator.Fill(proof);
            var proofKey = new SymmetricSecurityKey(proof);

            var descriptor = _fixture.CreateDescriptor(proofKey: proofKey);
            descriptor.EncryptingCredentials = null;

            var token = _handler.CreateToken(descriptor);
            OutputToken(_handler, token);

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
                    _handler.WriteToken(writer, token);
                stream.Position = 0;

                var document = XDocument.Load(stream);
                var subjectConfirmation = document.Descendants(XName.Get(Saml2Constants.Elements.SubjectConfirmation, Saml2Constants.Namespace)).SingleOrDefault();
                var method = subjectConfirmation.Attribute(Saml2Constants.Attributes.Method);

                Assert.Equal(Saml2Constants.ConfirmationMethods.HolderOfKeyString, method.Value);
                var subjectConfirmationData = subjectConfirmation
                    ?.Elements(XName.Get(Saml2Constants.Elements.SubjectConfirmationData, Saml2Constants.Namespace)).SingleOrDefault();

                Assert.NotNull(subjectConfirmationData);
                Assert.Equal(Saml2Constants.Types.KeyInfoConfirmationDataType, subjectConfirmationData.Attribute(XName.Get("Type", "http://www.w3.org/2001/XMLSchema-instance"))?.Value);

                var encryptedKeyInfo = subjectConfirmationData
                    ?.Elements(XName.Get(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace)).SingleOrDefault()
                    ?.Elements(XName.Get("EncryptedKey", "http://www.w3.org/2001/04/xmlenc#")).SingleOrDefault();
                Assert.NotNull(encryptedKeyInfo);
            }
        }

        [Fact]
        public void ShouldRoundTripEncryptedHolderOfKeyConfirmation()
        {
            var proof = new byte[32];
            RandomNumberGenerator.Fill(proof);
            var proofKey = new SymmetricSecurityKey(proof);

            var descriptor = _fixture.CreateDescriptor(proofKey: proofKey);
            descriptor.EncryptingCredentials = null;

            var token = _handler.CreateToken(descriptor);
            OutputToken(_handler, token);

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
                    _handler.WriteToken(writer, token);
                stream.Position = 0;
                using (var reader = XmlReader.Create(stream))
                {
                    var parameters = _fixture.CreateTokenValidationParameters(validateLifetime: true);
                    var user = _handler.ValidateToken(reader, parameters, out var validatedToken);

                    Assert.NotNull(user);
                    Assert.NotNull(validatedToken);
                    _fixture.AssertContainsSymmetricKey(validatedToken, proofKey);
                }
            }
        }

        [Fact]
        public void ShouldRoundTripUnencryptedHolderOfKeyConfirmation()
        {
            var proof = new byte[32];
            RandomNumberGenerator.Fill(proof);
            var proofKey = new SymmetricSecurityKey(proof);

            var descriptor = _fixture.CreateDescriptor(proofKey: proofKey, encryptProofKey: false);
            descriptor.EncryptingCredentials = null;

            var token = _handler.CreateToken(descriptor);
            OutputToken(_handler, token);

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
                    _handler.WriteToken(writer, token);
                stream.Position = 0;
                using (var reader = XmlReader.Create(stream))
                {
                    var parameters = _fixture.CreateTokenValidationParameters(validateLifetime: true);
                    var user = _handler.ValidateToken(reader, parameters, out var validatedToken);

                    Assert.NotNull(user);
                    Assert.NotNull(validatedToken);
                    _fixture.AssertContainsSymmetricKey(validatedToken, proofKey);
                }
            }
        }

        [Fact]
        public void ShouldAddToBearerConfirmationData()
        {
            var recipient = new Uri("https://recipient");
            var descriptor = _fixture.CreateDescriptor();
            var token = _handler.CreateToken(descriptor) as Saml2SecurityToken;
            token.SetNotBefore();
            token.SetNotOnOrAfter();
            token.SetRecipient(recipient);
            OutputToken(_handler, token);

            var confirmation = token?.Assertion.Subject.SubjectConfirmations.FirstOrDefault(c => c.Method == Saml2Constants.ConfirmationMethods.Bearer);
            Assert.NotNull(confirmation?.SubjectConfirmationData);

            Assert.Equal(recipient, confirmation.SubjectConfirmationData.Recipient);
            Assert.Equal(descriptor.NotBefore, confirmation.SubjectConfirmationData.NotBefore);
            Assert.Equal(descriptor.Expires, confirmation.SubjectConfirmationData.NotOnOrAfter);
        }

        [Theory]
        [InlineData("2019-07-10T15:35:50.123Z", "urn:auth_method")]
        public void ShouldIncludeAuthenticationStatement(string authenticationInstant, string authenticationMethod)
        {
            var descriptor = _fixture.CreateDescriptor();
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, authenticationInstant));
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            var token = _handler.CreateToken(descriptor) as Saml2SecurityToken;
            OutputToken(_handler, token);

            var statement = token?.Assertion.Statements.OfType<Saml2AuthenticationStatement>().FirstOrDefault();
            Assert.NotNull(statement);
            Assert.Equal(DateTime.Parse(authenticationInstant), statement.AuthenticationInstant);
            Assert.Equal(new Uri(authenticationMethod), statement.AuthenticationContext.ClassReference);
            Assert.Equal(token.Assertion.Id.Value, statement.SessionIndex);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("invalid")]
        public void ShouldIncludeAuthenticationStatementWithUnspecifiedAuthenticationMethod(string authenticationMethod)
        {
            var descriptor = _fixture.CreateDescriptor();
            var instant = DateTime.UtcNow;
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(instant, XmlDateTimeSerializationMode.Utc)));
            if(!string.IsNullOrEmpty(authenticationMethod))
                descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            var token = _handler.CreateToken(descriptor) as Saml2SecurityToken;
            OutputToken(_handler, token);

            var statement = token?.Assertion.Statements.OfType<Saml2AuthenticationStatement>().FirstOrDefault();
            Assert.NotNull(statement);
            Assert.Equal(instant, statement.AuthenticationInstant);
            Assert.Equal(new Uri(AuthenticationContextClasses.Unspecified), statement.AuthenticationContext.ClassReference);
            Assert.Equal(token.Assertion.Id.Value, statement.SessionIndex);
        }

        [Theory]
        [InlineData("incoming", "uri:expected")]
        public void ShouldIncludeAuthenticationStatementWithMappedAuthenticationMethod(string authenticationMethod, string expectedAuthenticationMethod)
        {
            var expected = new Uri(expectedAuthenticationMethod);
            _options.AuthenticationMethodMap.Add(authenticationMethod, expected);

            var descriptor = _fixture.CreateDescriptor();
            var instant = DateTime.UtcNow;
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(instant, XmlDateTimeSerializationMode.Utc)));
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            var token = _handler.CreateToken(descriptor) as Saml2SecurityToken;
            OutputToken(_handler, token);

            var statement = token?.Assertion.Statements.OfType<Saml2AuthenticationStatement>().FirstOrDefault();
            Assert.NotNull(statement);
            Assert.Equal(instant, statement.AuthenticationInstant);
            Assert.Equal(expected, statement.AuthenticationContext.ClassReference);
            Assert.Equal(token.Assertion.Id.Value, statement.SessionIndex);
        }

        [Theory]
        [InlineData("some invalid date", "urn:auth_method")]
        [InlineData(null, "urn:auth_method")]
        [InlineData("some invalid date", "some invalid auth method")]
        [InlineData(null, "some invalid auth method")]
        [InlineData("some invalid date", null)]
        [InlineData(null, null)]
        public void ShouldNotIncludeAuthenticationStatement(string authenticationInstant, string authenticationMethod)
        {
            var descriptor = _fixture.CreateDescriptor();
            if (authenticationInstant != null)
                descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, authenticationInstant));
            if (authenticationMethod != null)
                descriptor.Subject.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            var token = _handler.CreateToken(descriptor) as Saml2SecurityToken;

            OutputToken(_handler, token);
            var statement = token?.Assertion.Statements.OfType<Saml2AuthenticationStatement>().FirstOrDefault();
            Assert.Null(statement);
        }

        private void OutputToken(Saml2SecurityTokenHandler handler, SecurityToken token)
            => OutputToken(handler, token as Saml2SecurityToken);

        private void OutputToken(Saml2SecurityTokenHandler handler, Saml2SecurityToken token)
        {
            Assert.NotNull(token);
            _output.WriteLine("SAML assertion:");
            if (token == null)
            {
                _output.WriteLine("null");
                return;
            }
            using (var inner = new StringWriter())
            {
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NewLineOnAttributes = false,
                    Encoding = new UTF8Encoding(false)
                };
                using (var writer = XmlWriter.Create(inner, settings))
                {
                    handler.WriteToken(writer, token);
                }
                _output.WriteLine(inner.ToString());
            }
        }
    }
}
