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
using Xunit;

namespace Solid.IdentityModel.Tokens.Saml.Tests
{
    public class Saml2EncryptedSecurityTokenHandlerTests : IClassFixture<Saml2TestFixture>
    {
        private static readonly string _encryptedAssertion = "<saml:EncryptedAssertion xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\"><xenc:EncryptedData Type=\"http://www.w3.org/2001/04/xmlenc#Element\" xmlns:xenc=\"http://www.w3.org/2001/04/xmlenc#\"><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes128-cbc\" /><ds:KeyInfo xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\">\n<xenc:EncryptedKey><xenc:EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p\"><ds:DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /></xenc:EncryptionMethod><ds:KeyInfo>\n<wsse:SecurityTokenReference xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\"><wsse:KeyIdentifier ValueType=\"http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1#ThumbprintSHA1\">GdAB+K/AJCgNzGaBXiXr1b0n6Pk=</wsse:KeyIdentifier></wsse:SecurityTokenReference>\n</ds:KeyInfo><xenc:CipherData><xenc:CipherValue>bB5ma0e9gx6Wb7ZQiNvqhPSf9sp6bbOMOzvDsLfuODvhsGbiaCPpsgjVZjGULpXIjnTl22AQKQgE\nvReBREC2KTtQc5O9JAufQ40WbFlovpwQdPV1JK2RzpLHz6Yt2OLX0gEkp/oFw3W5VPXSJkjrW4Tj\n5goC3R981zltRjj0n+wk6BlWX6WU8Xr7rFHesrI+qLMFuhXj9yf+uXQRsqk9rPpn1rm/+Xksalum\nEVN+S5CVCLw1QxXQMG9xJwt1vJMUxkGvZh4XlSI9m1Gw3t/g1I7ttme80s3Cguux6m+yjzaSR51L\njiUab1DUEDCdNUoW+o48CQVK3+zsU8BQVwgp8Q==</xenc:CipherValue></xenc:CipherData></xenc:EncryptedKey></ds:KeyInfo><xenc:CipherData><xenc:CipherValue>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</xenc:CipherValue></xenc:CipherData></xenc:EncryptedData></saml:EncryptedAssertion>";
        private Saml2TestFixture _fixture;
        private const string _decryptionCertificateBase64 = "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";
        private const string _signatureVerificationCertificateBase64 = "MIIDBjCCAe6gAwIBAgIGAXMUkrVPMA0GCSqGSIb3DQEBCwUAMEQxCzAJBgNVBAYTAklTMRUwEwYDVQQKEwxMYW5kc2Jhbmtpbm4xHjAcBgNVBAMTFVdTVHJ1c3RDaGFubmVsRmFjdG9yeTAeFw0yMDA3MDMxMjA3MDVaFw0yMTA3MDMxMjA3MDVaMEQxCzAJBgNVBAYTAklTMRUwEwYDVQQKEwxMYW5kc2Jhbmtpbm4xHjAcBgNVBAMTFVdTVHJ1c3RDaGFubmVsRmFjdG9yeTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAJAhPg6699LZx+OVfnC49b7btnYXSs9+oGx8uSU1geTV5+bW3l5EQBZaLuMAGBy4KXft7fUXm2/NG3UnjVKX+mK84bJn68mF9DroVglm+bWgvgy/ayFJmLj3NRby9ZrN/qNin8zmeZVBIR0xi6eR3XlAbJkgf1BtK27qzSP0qeYcpBf2qLFcZEuPO//1jTxYj/7EbyAAULSdJBR81pDskUWoV4Sux1uFyUXA+XLirhuBOagJXJqJEmgnAAhbOtoO5q8xvYyplagnOyEmwu9C3lwO6r3fdEJVTHKGE1pnL0CwoAhyyQtWVzbZuUQpSylcqqiBNtoak6oN8lsIb4dc1psCAwEAATANBgkqhkiG9w0BAQsFAAOCAQEAg1KkwPAAsrSuJ3eOIrAjgE/dlKqa/OdMwVFz13sbBp106Yk7UPRww/Bf/nnHwW7nK0613wvT86jhMZG7LS9xa6q8mmx5m5XnEec5g8uSws8pjiR8CRWrm/FnXYMUnJV60LC4SNFpRE5x0pyjoxkYjZ4DkMtmVFZWX8Vw7K9TAwWcwD8QB03qbVVhkAi87lt+hgnce5Wkiq1c5Q6TIYBRwNnxu4aHjY9TC3TLeUCRC+GVuLhRmRS4T4NJ/UVd1D6b+thCJEMPUDH6LyAkTpereC/CItOKPiYWV37D8slhJiThkpNqJipuIjfkvHCRBlJ4nz9tUj1fft4YLcQTpjq4MQ==";        

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
            using var signatureVerificationCertificate = new X509Certificate2(Convert.FromBase64String(_signatureVerificationCertificateBase64));
            using var decryptionCertificate = new X509Certificate2(Convert.FromBase64String(_decryptionCertificateBase64));
            var parameters = _fixture.CreateTokenValidationParameters(decryptionKey: new X509SecurityKey(decryptionCertificate), signatureVerificationKey: new X509SecurityKey(signatureVerificationCertificate));
            var token = handler.ReadToken(_encryptedAssertion, parameters);
            Assert.NotNull(token);
            Assert.IsType<Saml2EncryptedSecurityToken>(token);

            var encrypted = token as Saml2EncryptedSecurityToken;
            Assert.NotNull(encrypted.Assertion);
            Assert.Null(encrypted.EncryptingCredentials);
            Assert.NotNull(encrypted.EncryptedData);
        }

        [Fact]
        public void ShouldValidateEncryptedToken()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            using var signatureVerificationCertificate = new X509Certificate2(Convert.FromBase64String(_signatureVerificationCertificateBase64));
            using var decryptionCertificate = new X509Certificate2(Convert.FromBase64String(_decryptionCertificateBase64));
            var parameters = _fixture.CreateTokenValidationParameters(decryptionKey: new X509SecurityKey(decryptionCertificate), signatureVerificationKey: new X509SecurityKey(signatureVerificationCertificate));
            var user = handler.ValidateToken(_encryptedAssertion, parameters, out var token);
            Assert.NotNull(token);
            Assert.IsType<Saml2EncryptedSecurityToken>(token);

            var encrypted = token as Saml2EncryptedSecurityToken;
            Assert.NotNull(encrypted.Assertion);
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
            Assert.NotNull(encrypted.Assertion);
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
            Assert.NotNull(encrypted.Assertion);
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

            using (var stream = new MemoryStream())
            {
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
        }

        [Fact]
        public void ShouldRoundTripEncryptedTokenToXmlWriter()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();
            var descriptor = _fixture.CreateDescriptor();

            var token = handler.CreateToken(descriptor);

            using (var stream = new MemoryStream())
            {
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
        }

        [Fact]
        public void ShouldRoundTripEncryptedTokenToXmlWriterUsingCertificates()
        {
            var handler = new Saml2EncryptedSecurityTokenHandler();

            var signingCertificate = _fixture.GenerateCertificate();
            var decryptionCertificate = _fixture.GenerateCertificate();
            var verificationCertificate = new X509Certificate2(signingCertificate.Export(X509ContentType.Cert));
            var encryptionCertificate = new X509Certificate2(decryptionCertificate.Export(X509ContentType.Cert));

            var descriptor = _fixture.CreateDescriptor(signingKey: new X509SecurityKey(signingCertificate), encryptionKey: new X509SecurityKey(encryptionCertificate));

            var token = handler.CreateToken(descriptor);

            using (var stream = new MemoryStream())
            {
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
}
