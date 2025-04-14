using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Tokens.Crypto;
using Xunit;

namespace Solid.IdentityModel.Tokens.Saml.Tests
{
    public class Saml2EncryptedSecurityTokenHandlerTests : IClassFixture<Saml2TestFixture>
    {
        private static readonly JsonWebKey _signatureVerificationJwk = new (@"
{
    ""kty"": ""RSA"",
    ""e"": ""AQAB"",
    ""use"": ""sig"",
    ""alg"": ""RS256"",
    ""n"": ""2nLVNpkBbpq1Ut0yLicjXeLRNp3mK3kAqYu2Mka52c5ekRBXphBrlD3DLJJcAGDpDvYyZBJOeGDk7PkBhwBKwELznwf1-pIwI28qzQsJTz5MPsUF4d6sNgpVzg5nvdf-hX5afqUH8QwecfPscoVE91wfWN65NgcB7Crrik4Om2TgMbHucFOMUBwJWxsO3sAmOWI4kTZDsFamQoNkXRcuTuHpNo7ISKQOzcUNvRtjViPZpwQiRx7pZh-S6Nvjz_PsnHcNgnQcjohR-v8zDvRp0FMDf009hjjhDj5kFSRFEofKOkzlbet9eiUcQjpAMt2I01O9et9ZYVyk2DfjPWHGkQ""
}");

        private static readonly JsonWebKey _decryptionJwk = new (@"
{
    ""p"": ""xX3QDWg3E6b00AqBFNmYV-ol7LNPlLpHscxm2UqZMN_p9taxqsRFt6PP9jDAg9XV9oKbyTJvKEzP4qlmlrgya1flElxrisipgRpqEUlfCYLoCitmJ_61FtfMoNzDPfBVECsLjYEsXS0NgvhHIzq657Y9mH3TKqDIQ_dM-UJ5CU8"",
    ""kty"": ""RSA"",
    ""q"": ""qH_QUPzOp7ZRyaRkUl6keYc_oTYNinF7OGhuy1rFYFKW5CPy7VeMI3ZIeqOLRbLHoZuxhJIdPZSn_LWnzHH9JnchMi_qXXZ-ygSLdtZ9bz5wQJDQpOIEJZRM2VaXGCVW2LrzUGSTV2HPfV0c7AGldZ0PHoNBURqzqu742z8ZWC8"",
    ""d"": ""FBGH8lih5_NM6GFFWnkTAdAVwQcIupLeNIFnbci2E__Ez52huoCK95ODiqrrSG9kbjfHiIzX8thX1cntPIzezTk6XKT8lMLfp7S1xKc8aR_LRumozVH4769BDQn8MCK9UqBUG86n4qWhpq-vAdPPdCoPBjYDnr80zsLvnxYvFWUEj1KJgg2-MeMBsoFrjSj2ErfXQxxBVjms7AziCI0vJKsQUWnfVeFQ49YXs-Z60eSeIyb9yef2isOlnxvOP_ALpQIRUbTEnRWGhhqtlcJIAfDWsWbfBZ7fM1VuV-iI-2sIehTGQ4pTGt4Pc8hrNvqdhnEKq3ps_E1bJnXIMmyGGQ"",
    ""e"": ""AQAB"",
    ""use"": ""enc"",
    ""qi"": ""RZ6p6isvg7wBwOTuLwVIH6pat2pl6usxt7KUskYwTCZoIf9WCi7lPN8Xi8xUdGYTe_k6zrz4kSMiQ_VtfTuIqkAkoN1p08X_mQis5JoncVWT1LCdQypIhEQNlKv1Gqm4e_OTVjHPm4OaJ56LB805qBIMmbk8vCcKLNzPp7Y_kHM"",
    ""dp"": ""YGTMS_72Aw2WqIS4BGlAxohvAl1zFnDl1Y6jFKQoqYZhOC4KggNS1BOMyel5zd9tk-ikCUwonU8AmO1-OUqmsWYxVQjvJMpUkcNGyjE5xfazM2ODdToJQaELK-kVEwJfQokAFo1aDhCTa72rWzKrT7XP0sJ3c3MOzL3EQFWFplk"",
    ""alg"": ""RSA-OAEP-256"",
    ""dq"": ""UKq4AT22daYcK6vO93wlw6STOsuU2fWQJyYf_KzdF0sSv-_R6fxis8t50XSgRWLcnara5nvJEeUsMxiIV6Eur46SzuMPkWUcN_zLA76V2H8M4Gwz5uvpTlBcJiSFO2MM279Mou0zeL7zxbGhGf-DxfXF-jaeO4TMBQZZDyV7LbM"",
    ""n"": ""gf0qp7ZuBoH4TN7MOg1liCY-rK1PCCMJcmc6bQCNmtdJAR_c_Hb0NmkHfmrsMququWoOruSi7vRG_8qj0EjAJDt_ngQuMa2hCHW9VwMDMnTNNmv6tiOueFaml5T0kRoY7mGhrf331dXn68ro6ad6NJBmbVQZBF9s_2_XkaFTiOOSosV7TcbWuyyuXg-bgdO_RLZQ6JeuPMohJ3hvS9VGGU7vb0mUGtL9FeuYTKJjEn6GWdbqtcQfgrGexUJxBgMhMX2MC1ncwZCGhPRPNQ95N19KifRRnyO5q2tyI1WAPFUfhvHE9Ux_tUzkcojsdNhOX6k-u_vz4cyUkZZOKSLdgQ""
}");
        
        private static readonly string _encryptedAssertion = "<saml:EncryptedAssertion xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\"><EncryptedData Type=\"http://www.w3.org/2001/04/xmlenc#Element\" xmlns=\"http://www.w3.org/2001/04/xmlenc#\"><EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\" /><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><EncryptedKey xmlns=\"http://www.w3.org/2001/04/xmlenc#\"><EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p\" /><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><KeyValue><RSAKeyValue><Modulus>gf0qp7ZuBoH4TN7MOg1liCY+rK1PCCMJcmc6bQCNmtdJAR/c/Hb0NmkHfmrsMququWoOruSi7vRG/8qj0EjAJDt/ngQuMa2hCHW9VwMDMnTNNmv6tiOueFaml5T0kRoY7mGhrf331dXn68ro6ad6NJBmbVQZBF9s/2/XkaFTiOOSosV7TcbWuyyuXg+bgdO/RLZQ6JeuPMohJ3hvS9VGGU7vb0mUGtL9FeuYTKJjEn6GWdbqtcQfgrGexUJxBgMhMX2MC1ncwZCGhPRPNQ95N19KifRRnyO5q2tyI1WAPFUfhvHE9Ux/tUzkcojsdNhOX6k+u/vz4cyUkZZOKSLdgQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue></KeyValue></KeyInfo><CipherData><CipherValue>EBGqlbXRu3e4vx33L9m2QXhJPkFmp01F5NIJT4tSXAPB7GvqqUkwuPAx1VL4iZ2fHm2tIuLiDkdWeamdz+Knee+tiXQRLJU5pTdR1iW02VC1Ids9jAJdZWBZePywsykWzMS4wNwMLYLDwD6MN2smZxt5VbZoGNg3JDO7oLRJJxOO15ZY7XJ+L9ujYKnA+gpSciRuYyrUthyXbD4FSdKyFlt4trccXWVbs5Y3OtA0uWZyTVshp5PHZwRsObw7s/npDlvmJvMK/VgkuwNkJCOAy1BwKY3YPvZkEYB9ceYB509ZZs8N5VZvvy8RLuKxA44qggXdGk7ix9QRjxPf2UE0bQ==</CipherValue></CipherData></EncryptedKey></KeyInfo><CipherData><CipherValue>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</CipherValue></CipherData></EncryptedData></saml:EncryptedAssertion>";
        private Saml2TestFixture _fixture;
        
        static Saml2EncryptedSecurityTokenHandlerTests()
        {
            IdentityModelEventSource.ShowPII = true;

            var services = new ServiceCollection()
                .AddCustomCryptoProvider(options => options.AddFullSupport())
                .BuildServiceProvider()
            ;

            CryptoProviderFactory.Default = services.GetRequiredService<CryptoProviderFactory>();
        }

        public Saml2EncryptedSecurityTokenHandlerTests(Saml2TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ShouldBeAbleToReadEncryptedToken()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            Assert.True(handler.CanReadToken(_encryptedAssertion));
        }

        [Fact]
        public void ShouldReadEncryptedToken()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            
            Assert.True(JsonWebKeyConverter.TryConvertToSecurityKey(_signatureVerificationJwk, out var signatureVerificationKey));
            Assert.True(JsonWebKeyConverter.TryConvertToSecurityKey(_decryptionJwk, out var decryptionKey));
            
            var parameters = _fixture.CreateTokenValidationParameters(decryptionKey: decryptionKey, signatureVerificationKey: signatureVerificationKey);
            var token = handler.ReadToken(_encryptedAssertion, parameters);
            Assert.NotNull(token);
            Assert.IsType<Saml2EncryptedSecurityToken>(token);

            var encrypted = token as Saml2EncryptedSecurityToken;
            Assert.NotNull(encrypted?.Assertion);
            Assert.Null(encrypted.EncryptingCredentials);
            Assert.NotNull(encrypted.EncryptedData);
        }

        [Fact]
        public void ShouldValidateEncryptedToken()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            
            Assert.True(JsonWebKeyConverter.TryConvertToSecurityKey(_signatureVerificationJwk, out var signatureVerificationKey));
            Assert.True(JsonWebKeyConverter.TryConvertToSecurityKey(_decryptionJwk, out var decryptionKey));

            var parameters = _fixture.CreateTokenValidationParameters(decryptionKey: decryptionKey, signatureVerificationKey: signatureVerificationKey);
            _ = handler.ValidateToken(_encryptedAssertion, parameters, out var token);
            Assert.NotNull(token);
            Assert.IsType<Saml2EncryptedSecurityToken>(token);

            var encrypted = token as Saml2EncryptedSecurityToken;
            Assert.NotNull(encrypted?.Assertion);
            Assert.Null(encrypted.EncryptingCredentials);
            Assert.NotNull(encrypted.EncryptedData);
        }

        [Fact]
        public void ShouldCreateEncryptedTokenWithoutNameId()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            var descriptor = _fixture.CreateDescriptor(nameIdentifier: null);

            var token = handler.CreateToken(descriptor);

            Assert.NotNull(token);
            Assert.IsType<Saml2EncryptedSecurityToken>(token);

            var encrypted = token as Saml2EncryptedSecurityToken;
            Assert.NotNull(encrypted?.Assertion);
            Assert.NotNull(encrypted.EncryptingCredentials);
            Assert.Null(encrypted.EncryptedData);
        }

        [Fact]
        public void ShouldCreateEncryptedToken()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            var descriptor = _fixture.CreateDescriptor();

            var token = handler.CreateToken(descriptor);

            Assert.NotNull(token);
            Assert.IsType<Saml2EncryptedSecurityToken>(token);

            var encrypted = token as Saml2EncryptedSecurityToken;
            Assert.NotNull(encrypted?.Assertion);
            Assert.NotNull(encrypted.EncryptingCredentials);
            Assert.Null(encrypted.EncryptedData);
        }

        [Fact]
        public void ShouldWriteEncryptedToken()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            var descriptor = _fixture.CreateDescriptor();

            var token = handler.CreateToken(descriptor);
            var value = handler.WriteToken(token);
            Assert.NotNull(value);
            Assert.StartsWith("<saml:EncryptedAssertion", value);
        }

        [Fact]
        public void ShouldWriteEncryptedTokenToXmlWriter()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            var descriptor = _fixture.CreateDescriptor();

            var token = handler.CreateToken(descriptor);

            using var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
                handler.WriteToken(writer, token);
            stream.Position = 0;
            using (var reader = XmlReader.Create(stream))
            {
                reader.MoveToContent();
                Assert.True(reader.IsStartElement("EncryptedAssertion", Saml2Constants.Namespace));
                Assert.False(reader.IsEmptyElement);
            }
        }

        [Fact]
        public void ShouldRoundTripEncryptedTokenToXmlWriter()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            var descriptor = _fixture.CreateDescriptor();

            var token = handler.CreateToken(descriptor);

            using var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
                handler.WriteToken(writer, token);
            stream.Position = 0;
            using (var reader = XmlReader.Create(stream))
            {
                var parameters = _fixture.CreateTokenValidationParameters(validateLifetime: true);
                var user = handler.ValidateToken(reader, parameters, out var validatedToken);

                Assert.NotNull(user);
                Assert.NotNull(validatedToken);
            }
        }

        [Fact]
        public void ShouldRoundTripEncryptedTokenToXmlWriterUsingCertificates()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();

            var signingCertificate = _fixture.GenerateCertificate();
            var decryptionCertificate = _fixture.GenerateCertificate();
            var verificationCertificate = X509CertificateLoader.LoadCertificate(signingCertificate.Export(X509ContentType.Cert));
            var encryptionCertificate = X509CertificateLoader.LoadCertificate(decryptionCertificate.Export(X509ContentType.Cert));

            var descriptor = _fixture.CreateDescriptor(signingKey: new X509SecurityKey(signingCertificate), encryptionKey: new X509SecurityKey(encryptionCertificate));

            var token = handler.CreateToken(descriptor);

            using var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
                handler.WriteToken(writer, token);
            stream.Position = 0;
            using (var reader = XmlReader.Create(stream))
            {
                var parameters = _fixture.CreateTokenValidationParameters(signatureVerificationKey: new X509SecurityKey(verificationCertificate), decryptionKey: new X509SecurityKey(decryptionCertificate), validateLifetime: true);
                var user = handler.ValidateToken(reader, parameters, out var validatedToken);

                Assert.NotNull(user);
                Assert.NotNull(validatedToken);
            }
        }
    }
}
