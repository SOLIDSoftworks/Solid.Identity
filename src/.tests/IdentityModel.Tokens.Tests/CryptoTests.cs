using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens.Crypto;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using Xunit;

namespace Solid.IdentityModel.Tokens.Tests
{
    public class CryptoTests
    {
        static readonly X509Certificate2 Certificate;

        static CryptoTests()
        {
            Certificate = GenerateCertificate();
            IdentityModelEventSource.ShowPII = true;
        }

        [Theory]
        [InlineData("http://www.w3.org/2000/09/xmldsig#sha1")]
        [InlineData(SecurityAlgorithms.Sha256Digest)]
        [InlineData(SecurityAlgorithms.Sha384Digest)]
        [InlineData(SecurityAlgorithms.Sha512Digest)]
        [InlineData("SHA1")]
        [InlineData(SecurityAlgorithms.Sha256)]
        [InlineData(SecurityAlgorithms.Sha384)]
        [InlineData(SecurityAlgorithms.Sha512)]
        public void ShouldSupportHashAlgorithm(string algorithm)
        {
            var crypto = CreateCryptoProviderFactory();
            Assert.True(crypto.IsSupportedAlgorithm(algorithm));
        }

        [Theory]
        [InlineData(SecurityAlgorithms.Aes128Encryption)]
        [InlineData(SecurityAlgorithms.Aes192Encryption)]
        [InlineData(SecurityAlgorithms.Aes256Encryption)]
        public void ShouldSupportSymmetricAlgorithm(string algorithm)
        {
            var crypto = CreateCryptoProviderFactory();
            Assert.True(crypto.IsSupportedAlgorithm(algorithm));
        }

        [Theory]
        [InlineData("http://www.w3.org/2000/09/xmldsig#rsa-sha1")]
        [InlineData(SecurityAlgorithms.RsaSha256Signature)]
        [InlineData(SecurityAlgorithms.RsaSha384Signature)]
        [InlineData(SecurityAlgorithms.RsaSha512Signature)]
        [InlineData("RS1")]
        [InlineData(SecurityAlgorithms.RsaSha256)]
        [InlineData(SecurityAlgorithms.RsaSha384)]
        [InlineData(SecurityAlgorithms.RsaSha512)]
        public void ShouldSupportAsymmetricSignatureAlgorithm(string algorithm)
        {
            var key = new X509SecurityKey(Certificate);
            var crypto = CreateCryptoProviderFactory();
            Assert.True(crypto.IsSupportedAlgorithm(algorithm, key));
        }
        
        [Theory]
        [InlineData("http://www.w3.org/2000/09/xmldsig#hmac-sha1")]
        [InlineData(SecurityAlgorithms.HmacSha256Signature)]
        [InlineData(SecurityAlgorithms.HmacSha384Signature)]
        [InlineData(SecurityAlgorithms.HmacSha512Signature)]
        [InlineData("H1")]
        [InlineData(SecurityAlgorithms.HmacSha256)]
        [InlineData(SecurityAlgorithms.HmacSha384)]
        [InlineData(SecurityAlgorithms.HmacSha512)]
        public void ShouldSupportSymmetricSignatureAlgorithm(string algorithm)
        {
            var bytes = new byte[16];
            var random = RandomNumberGenerator.Create();
            random.GetNonZeroBytes(bytes);
            var key = new SymmetricSecurityKey(bytes);
            var crypto = CreateCryptoProviderFactory();
            Assert.True(crypto.IsSupportedAlgorithm(algorithm, key));
            var s = crypto.CreateForSigning(key, algorithm);
        }

        [Theory]
        [InlineData(SecurityAlgorithms.RsaOAEP)]
        [InlineData(SecurityAlgorithms.RsaOaepKeyWrap)]
        [InlineData(KeyWrapAlgorithms.RsaOaepMgf1pAlgorithm)]
        public void ShouldSupportKeyWrapAlgorithm(string algorithm)
        {
            var key = new X509SecurityKey(Certificate);
            var crypto = CreateCryptoProviderFactory();
            Assert.True(crypto.IsSupportedAlgorithm(algorithm, key));
        }

        [Theory]
        [InlineData("http://www.w3.org/2000/09/xmldsig#hmac-sha1")]
        [InlineData("H1")]
        public void ShouldSupportKeyedHashAlgorithm(string algorithm)
        {
            var key = new X509SecurityKey(Certificate);
            var crypto = CreateCryptoProviderFactory();
            Assert.True(crypto.IsSupportedAlgorithm(algorithm, key));
        }

        [Theory]
        [InlineData("http://www.w3.org/2000/09/xmldsig#sha1", typeof(SHA1))]
        [InlineData(SecurityAlgorithms.Sha256Digest, typeof(SHA256))]
        [InlineData(SecurityAlgorithms.Sha384Digest, typeof(SHA384))]
        [InlineData(SecurityAlgorithms.Sha512Digest, typeof(SHA512))]
        public void ShouldGetHashAlgorithm(string algorithm, Type type)
        {
            var crypto = CreateCryptoProviderFactory();
            var hashAlgorithm = crypto.CreateHashAlgorithm(algorithm);

            Assert.NotNull(hashAlgorithm);
            Assert.IsAssignableFrom(type, hashAlgorithm);
        }

        [Theory]
        [InlineData("http://www.w3.org/2000/09/xmldsig#hmac-sha1", typeof(HMACSHA1))]
        [InlineData("H1", typeof(HMACSHA1))]
        public void ShouldGetKeyedHashAlgorithm(string algorithm, Type type)
        {
            var key = new byte[16];
            RandomNumberGenerator.Fill(key);
            var crypto = CreateCryptoProviderFactory();
            var hashAlgorithm = crypto.CreateKeyedHashAlgorithm(key, algorithm);

            Assert.NotNull(hashAlgorithm);
            Assert.IsAssignableFrom(type, hashAlgorithm);
        }

        [Theory]
        [InlineData(SecurityAlgorithms.Aes128Encryption, typeof(Aes))]
        [InlineData(SecurityAlgorithms.Aes192Encryption, typeof(Aes))]
        [InlineData(SecurityAlgorithms.Aes256Encryption, typeof(Aes))]
        public void ShouldGetSymmetricAlgorithm(string algorithm, Type type)
        {
            var crypto = CreateCryptoProviderFactory();
            var symmetricAlgorithm = crypto.CreateSymmetricAlgorithm(algorithm);

            Assert.NotNull(symmetricAlgorithm);
            Assert.IsAssignableFrom(type, symmetricAlgorithm);
        }

        [Theory]
        [MemberData(nameof(SignatureProviderTestData))]
        public void ShouldGetSignatureProvider(SignatureProviderTestData data)
        {
            var crypto = CreateCryptoProviderFactory();

            if (data.Sign)
            {
                var provider = crypto.CreateForSigning(data.SigningKey, data.Algorithm);
                Assert.NotNull(provider);
                var signed = Convert.ToBase64String(provider.Sign(Encoding.UTF8.GetBytes(data.Data)));
                Assert.Equal(data.SignedData, signed);
            }
            if (data.Verify)
            {
                var provider = crypto.CreateForVerifying(data.SigningKey, data.Algorithm);
                Assert.NotNull(provider);
                var verified = provider.Verify(Encoding.UTF8.GetBytes(data.Data), Convert.FromBase64String(data.SignedData));
                Assert.True(verified);
            }
        }

        [Theory]
        [MemberData(nameof(KeyWrapProviderTestData))]
        public void ShouldGetKeyWrapProvider(KeyWrapProviderTestData data)
        {
            var crypto = CreateCryptoProviderFactory();
            var wrapped = null as string;
            if (data.Wrap)
            {
                var provider = crypto.CreateKeyWrapProvider(data.SecurityKey, data.Algorithm);
                Assert.NotNull(provider);
                wrapped = Convert.ToBase64String(provider.WrapKey(Convert.FromBase64String(data.PlainText)));
            }
            if (data.Unwrap)
            {
                var provider = crypto.CreateKeyWrapProviderForUnwrap(data.SecurityKey, data.Algorithm);
                Assert.NotNull(provider);
                var unwrapped = Convert.ToBase64String(provider.UnwrapKey(Convert.FromBase64String(data.Wrapped)));
                Assert.Equal(data.PlainText, unwrapped);

                if(wrapped != null)
                {
                    var unwrapped2 = Convert.ToBase64String(provider.UnwrapKey(Convert.FromBase64String(wrapped)));
                    Assert.Equal(data.PlainText, unwrapped2);
                }
            }
        }

        [Theory]
        [MemberData(nameof(RsaKeyWrapInteropTestData))]
        public void ShouldInteropWithEncryptedXml(KeyWrapProviderTestData data)
        {
            var crypto = CreateCryptoProviderFactory();
            var wrapped = null as string;
            if (data.Wrap && data.Unwrap)
            {
                var provider = crypto.CreateKeyWrapProvider(data.SecurityKey, data.Algorithm);
                Assert.NotNull(provider);
                wrapped = Convert.ToBase64String(provider.WrapKey(Convert.FromBase64String(data.PlainText)));
            }
            if (data.Unwrap)
            {
                var provider = crypto.CreateKeyWrapProviderForUnwrap(data.SecurityKey, data.Algorithm);
                Assert.NotNull(provider);
                var unwrapped = Convert.ToBase64String(provider.UnwrapKey(Convert.FromBase64String(data.Wrapped)));
                Assert.Equal(data.PlainText, unwrapped);

                if (wrapped != null)
                {
                    var rsa = null as RSA;
                    if (data.SecurityKey is X509SecurityKey x509SecurityKey)
                        rsa = x509SecurityKey.Certificate.GetRSAPrivateKey();
                    if (data.SecurityKey is RsaSecurityKey rsaSecurityKey)
                        rsa = rsaSecurityKey.Rsa ?? RSA.Create(rsaSecurityKey.Parameters);
                    var unwrapped2 = Convert.ToBase64String(EncryptedXml.DecryptKey(Convert.FromBase64String(wrapped), rsa, true));
                    Assert.Equal(data.PlainText, unwrapped2);
                }
            }
        }

        public static TheoryData<KeyWrapProviderTestData> RsaKeyWrapInteropTestData
        {
            get
            {
                var privateKey = Certificate.GetRSAPrivateKey();
                var privateKeyParameters = privateKey.ExportParameters(true);

                var key = new byte[32];
                RandomNumberGenerator.Fill(key);
                var plainText = Convert.ToBase64String(key);

                var theoryData = new TheoryData<KeyWrapProviderTestData>();

                var data = new[]
                {
                    new KeyWrapData
                    {
                        Algorithms = new []
                        {
                            SecurityAlgorithms.RsaOAEP,
                            SecurityAlgorithms.RsaOaepKeyWrap,
                            "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"
                        }
                    }
                };

                foreach (var item in data)
                {
                    var wrapped = Convert.ToBase64String(EncryptedXml.EncryptKey(key, Certificate.GetRSAPublicKey(), true));
                    foreach (var algorithm in item.Algorithms)
                    {
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new X509SecurityKey(Certificate),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = true,
                            Wrap = true
                        });
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new RsaSecurityKey(privateKey),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = true,
                            Wrap = true
                        });
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new RsaSecurityKey(privateKeyParameters),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = true,
                            Wrap = true
                        });
                    }
                }
                return theoryData;
            }
        }

        public static TheoryData<KeyWrapProviderTestData> KeyWrapProviderTestData
        {
            get
            {
                var privateKey = Certificate.GetRSAPrivateKey();
                var publicKey = Certificate.GetRSAPublicKey();
                var privateKeyParameters = privateKey.ExportParameters(true);
                var publicKeyParamters = publicKey.ExportParameters(false);

                var key = new byte[32];
                RandomNumberGenerator.Fill(key);
                var plainText = Convert.ToBase64String(key);

                var theoryData = new TheoryData<KeyWrapProviderTestData>();

                var data = new[]
                {
                    new KeyWrapData
                    {
                        Algorithms = new []
                        {
                            SecurityAlgorithms.RsaOAEP,
                            SecurityAlgorithms.RsaOaepKeyWrap,
                            "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"
                        }
                    }
                };

                foreach(var item in data)
                {
                    var provider = new RsaKeyWrapProvider(new X509SecurityKey(Certificate), SecurityAlgorithms.RsaOAEP, false);
                    var wrapped = Convert.ToBase64String(provider.WrapKey(key));
                    foreach (var algorithm in item.Algorithms)
                    {
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new X509SecurityKey(Certificate),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = true,
                            Wrap = true
                        });
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new RsaSecurityKey(privateKey),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = true,
                            Wrap = true
                        });
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new RsaSecurityKey(privateKeyParameters),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = true,
                            Wrap = true
                        });
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new RsaSecurityKey(publicKey),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = false,
                            Wrap = true
                        });
                        theoryData.Add(new KeyWrapProviderTestData
                        {
                            SecurityKey = new RsaSecurityKey(publicKeyParamters),
                            Algorithm = algorithm,
                            PlainText = plainText,
                            Wrapped = wrapped,
                            Unwrap = false,
                            Wrap = true
                        });
                    }
                }
                return theoryData;
            }
        }

        class KeyWrapData
        {
            public string[] Algorithms { get; set; }
            public Func<SecurityKey, KeyWrapProvider> CreateProvider { get; set; }
        }


        public static TheoryData<SignatureProviderTestData> SignatureProviderTestData
        {
            get
            {
                var privateKey = Certificate.GetRSAPrivateKey();
                var publicKey = Certificate.GetRSAPublicKey();
                var privateKeyParameters = privateKey.ExportParameters(true);
                var publicKeyParamters = publicKey.ExportParameters(false);
                var plaintext = "Test data";
                var asymmetric = new[]
                {
                    new AsymmetricSignatureData
                    {
                        Data = plaintext,
                        Algorithms = new []
                        {
                            "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
                            "RS1"
                        },
                        HashAlgorithm = HashAlgorithmName.SHA1,
                        Padding = RSASignaturePadding.Pkcs1
                    },
                    new AsymmetricSignatureData
                    {
                        Data = plaintext,
                        Algorithms = new []
                        {
                            SecurityAlgorithms.RsaSha256,
                            SecurityAlgorithms.RsaSha256Signature
                        },
                        HashAlgorithm = HashAlgorithmName.SHA256,
                        Padding = RSASignaturePadding.Pkcs1
                    },
                    new AsymmetricSignatureData
                    {
                        Data = plaintext,
                        Algorithms = new []
                        {
                            SecurityAlgorithms.RsaSha384,
                            SecurityAlgorithms.RsaSha384Signature
                        },
                        HashAlgorithm = HashAlgorithmName.SHA384,
                        Padding = RSASignaturePadding.Pkcs1
                    },
                    new AsymmetricSignatureData
                    {
                        Data = plaintext,
                        Algorithms = new []
                        {
                            SecurityAlgorithms.RsaSha512,
                            SecurityAlgorithms.RsaSha512Signature
                        },
                        HashAlgorithm = HashAlgorithmName.SHA512,
                        Padding = RSASignaturePadding.Pkcs1
                    }
                };

                var theoryData = new TheoryData<SignatureProviderTestData>();
                foreach(var data in asymmetric)
                {
                    var signed = Convert.ToBase64String(privateKey.SignData(Encoding.UTF8.GetBytes(data.Data), data.HashAlgorithm, data.Padding));
                    foreach(var algorithm in data.Algorithms)
                    {
                        theoryData.Add(new SignatureProviderTestData
                        {
                            SigningKey = new X509SecurityKey(Certificate),
                            Algorithm = algorithm,
                            Verify = true,
                            Sign = true,
                            Data = data.Data,
                            SignedData = signed
                        });
                        theoryData.Add(new SignatureProviderTestData
                        {
                            SigningKey = new RsaSecurityKey(privateKey),
                            Algorithm = algorithm,
                            Sign = true,
                            Data = data.Data,
                            SignedData = signed
                        });
                        theoryData.Add(new SignatureProviderTestData
                        {
                            SigningKey = new RsaSecurityKey(publicKey),
                            Algorithm = algorithm,
                            Verify = true,
                            Data = data.Data,
                            SignedData = signed
                        });
                        theoryData.Add(new SignatureProviderTestData
                        {
                            SigningKey = new RsaSecurityKey(privateKeyParameters),
                            Algorithm = algorithm,
                            Sign = true,
                            Data = data.Data,
                            SignedData = signed
                        });
                        theoryData.Add(new SignatureProviderTestData
                        {
                            SigningKey = new RsaSecurityKey(publicKeyParamters),
                            Algorithm = algorithm,
                            Verify = true,
                            Data = data.Data,
                            SignedData = signed
                        });
                    }
                }

                return theoryData;
            }
        }
        
        class AsymmetricSignatureData
        {
            public string Data { get; set; }
            public HashAlgorithmName HashAlgorithm { get; set; }
            public RSASignaturePadding Padding { get; set; }
            public string[] Algorithms { get; set; }
        }

        private CryptoProviderFactory CreateCryptoProviderFactory()
        {
            var services = new ServiceCollection()
                .AddCustomCryptoProvider()
                .BuildServiceProvider()
            ;

            services.InitializeDefaultCryptoProviderFactory();

            return services.GetService<CryptoProviderFactory>();
        }
        static X509Certificate2 GenerateCertificate(DateTime? notBefore = null, DateTime? notAfter = null, bool addServerAuthentication = true, bool addClientAuthentication = true)
        {
            var name = Guid.NewGuid().ToString("N");
            var builder = new SubjectAlternativeNameBuilder();
            builder.AddDnsName(name);

            var dn = new X500DistinguishedName($"CN={name}");
            using (var rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(dn, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                var usage = new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyAgreement, true);
                request.CertificateExtensions.Add(usage);

                var oids = new OidCollection();
                if (addServerAuthentication)
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.1"));
                if (addClientAuthentication)
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.2"));

                request.CertificateExtensions.Add(
                   new X509EnhancedKeyUsageExtension(oids, false));

                request.CertificateExtensions.Add(builder.Build());

                return request.CreateSelfSigned(new DateTimeOffset(notBefore ?? DateTime.UtcNow.AddMinutes(-5)), new DateTimeOffset(notAfter ?? DateTime.UtcNow.AddMinutes(5)));
            }
        }
    }
}
