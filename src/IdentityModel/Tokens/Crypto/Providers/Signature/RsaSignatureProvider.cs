using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.IdentityModel.Tokens.Crypto.Providers.Signature
{
    internal class RsaSignatureProvider : SignatureProvider
    {
        private readonly ILogger<RsaSignatureProvider> _logger;
        private RSA _rsaPrivateKey;
        private RSA _rsaPublicKey;

        public RsaSignatureProvider(SecurityKey key, string algorithm, HashAlgorithmName hashAlgorithm, RSASignaturePadding signaturePadding, ILogger<RsaSignatureProvider> logger)
            : base(key, algorithm)
        {
            if (key is RsaSecurityKey rsa)
            {
                if (rsa.Rsa != null && rsa.PrivateKeyStatus == PrivateKeyStatus.Unknown)
                {
                    try
                    {
                        rsa.Rsa.SignData(Encoding.UTF8.GetBytes(nameof(RsaSignatureProvider)), hashAlgorithm, signaturePadding);
                        _rsaPrivateKey = rsa.Rsa;
                    }
                    catch
                    {
                        _rsaPublicKey = rsa.Rsa;
                    }
                }
                if (rsa.PrivateKeyStatus == PrivateKeyStatus.Exists)
                {
                    if(rsa.Rsa != null)
                        _rsaPrivateKey = rsa.Rsa;
                    else
                    {
                        _rsaPrivateKey = new RSACryptoServiceProvider();
                        _rsaPrivateKey.ImportParameters(rsa.Parameters);
                    }
                    
                }
                if (rsa.PrivateKeyStatus == PrivateKeyStatus.DoesNotExist)
                {
                    if (rsa.Rsa != null)
                        _rsaPublicKey = rsa.Rsa;
                    else
                    {
                        _rsaPublicKey = new RSACryptoServiceProvider();
                        _rsaPublicKey.ImportParameters(rsa.Parameters);
                    }

                }
            }
            if (key is X509SecurityKey x509)
            {
                if(x509.Certificate.HasPrivateKey)
                    _rsaPrivateKey = x509.Certificate.GetRSAPrivateKey();
                _rsaPublicKey = x509.Certificate.GetRSAPublicKey();
            }

            if (_rsaPublicKey == null && _rsaPrivateKey == null)
                throw new ArgumentException("Could not get RSA instance from SecurityKey", nameof(key));

            _logger = logger;

            HashAlgorithm = hashAlgorithm;
            SignaturePadding = signaturePadding;
        }

        public HashAlgorithmName HashAlgorithm { get; }
        public RSASignaturePadding SignaturePadding { get; }

        public override byte[] Sign(byte[] input)
        {
            if (_rsaPrivateKey == null) throw new InvalidOperationException("Cannot sign without private key.");
            //_logger.LogDebug($"Signing with SHA1 using {_certificate.Subject}");
            return _rsaPrivateKey.SignData(input, HashAlgorithm, SignaturePadding);
        }

        public override bool Verify(byte[] input, byte[] signature)
        {
            if (_rsaPublicKey == null) throw new InvalidOperationException("Cannot verify without public key.");
            //_logger.LogDebug($"Validating SHA1 using {_certificate.Subject}");
            return _rsaPublicKey.VerifyData(input, signature, HashAlgorithm, SignaturePadding);
        }

        protected override void Dispose(bool disposing)
        {
            _rsaPrivateKey?.Dispose();
            _rsaPublicKey?.Dispose();
        }
    }
}
