using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Solid.IdentityModel.Tokens
{
    public class CryptoOptions
    {
        public CryptoOptions MapHashAlgorithm(string algorithmFrom, string algorithmTo)
        {
            HashAlgorithmMap[algorithmFrom] = algorithmTo;
            return this;
        }
        public CryptoOptions MapSignatureAlgorithm(string algorithmFrom, string algorithmTo)
        {
            SignatureAlgorithmMap[algorithmFrom] = algorithmTo;
            return this;
        }
        public CryptoOptions MapKeyedHashAlgorithm(string algorithmFrom, string algorithmTo)
        {
            KeyedHashAlgorithmMap[algorithmFrom] = algorithmTo;
            return this;
        }
        public CryptoOptions MapKeyWrapAlgorithm(string algorithmFrom, string algorithmTo)
        {
            KeyWrapAlgorithmMap[algorithmFrom] = algorithmTo;
            return this;
        }
        public CryptoOptions MapEncryptionAlgorithm(string algorithmFrom, string algorithmTo)
        {
            EncryptionAlgorithmMap[algorithmFrom] = algorithmTo;
            return this;
        }

        public CryptoOptions AddSupportedHashAlgorithm(string algorithm, Func<IServiceProvider, HashAlgorithm> factory)
        {
            SupportedHashAlgorithms[algorithm] = new HashAlgorithmDescriptor(algorithm, (services, _) => factory(services));
            return this;
        }

        public CryptoOptions AddSupportedSymmetricAlgorithm(string algorithm, Func<IServiceProvider, SymmetricAlgorithm> factory)
        {
            SupportedSymmetricAlgorithms[algorithm] = new SymmetricAlgorithmDescriptor(algorithm, (services, _) => factory(services));
            return this;
        }

        public CryptoOptions AddSupportedSignatureAlgorithm(string algorithm, Func<IServiceProvider, SecurityKey, bool, SignatureProvider> factory)
        {
            SupportedSignatureAlgorithms[algorithm] = new SignatureProviderDescriptor(algorithm, (services, args) => factory(services, args.FirstOrDefault() as SecurityKey, (bool)args.ElementAtOrDefault(1)));
            return this;
        }

        public CryptoOptions AddSupportedKeyWrapAlgorithm(string algorithm, Func<IServiceProvider, SecurityKey, bool, KeyWrapProvider> factory)
        {
            SupportedKeyWrapAlgorithms[algorithm] = new KeyWrapProviderDescriptor(algorithm, (services, args) => factory(services, args.FirstOrDefault() as SecurityKey, args.ElementAtOrDefault(1) is bool unwrap && unwrap));
            return this;
        }

        public CryptoOptions AddSupportedKeyedHashAlgorithm(string algorithm, Func<IServiceProvider, byte[], KeyedHashAlgorithm> factory)
        {
            SupportedKeyedHashAlgorithms[algorithm] = new KeyedHashAlgorithmDescriptor(algorithm, (services, args) => factory(services, args.FirstOrDefault() as byte[]));
            return this;
        }

        public CryptoOptions AddSupportedEncryptionAlgorithm(string algorithm, Func<IServiceProvider, SecurityKey, string, AuthenticatedEncryptionProvider> factory)
        {
            SupportedEncryptionAlgorithms[algorithm] = new AuthenticatedEncryptionProviderDescriptor(algorithm, (services, args) => factory(services, args.FirstOrDefault() as SecurityKey, algorithm));
            return this;
        }

        internal IDictionary<string, string> HashAlgorithmMap { get; } = new Dictionary<string, string>();

        internal IDictionary<string, string> SignatureAlgorithmMap { get; } = new Dictionary<string, string>();

        internal IDictionary<string, string> KeyWrapAlgorithmMap { get; } = new Dictionary<string, string>();

        internal IDictionary<string, string> KeyedHashAlgorithmMap { get; } = new Dictionary<string, string>();

        internal IDictionary<string, string> EncryptionAlgorithmMap { get; } = new Dictionary<string, string>();

        internal IDictionary<string, SymmetricAlgorithmDescriptor> SupportedSymmetricAlgorithms { get; } = new Dictionary<string, SymmetricAlgorithmDescriptor>();
        internal IDictionary<string, HashAlgorithmDescriptor> SupportedHashAlgorithms { get; } = new Dictionary<string, HashAlgorithmDescriptor>();
        internal IDictionary<string, SignatureProviderDescriptor> SupportedSignatureAlgorithms { get; } = new Dictionary<string, SignatureProviderDescriptor>();
        internal IDictionary<string, KeyWrapProviderDescriptor> SupportedKeyWrapAlgorithms { get; } = new Dictionary<string, KeyWrapProviderDescriptor>();
        internal IDictionary<string, KeyedHashAlgorithmDescriptor> SupportedKeyedHashAlgorithms { get; } = new Dictionary<string, KeyedHashAlgorithmDescriptor>();
        internal IDictionary<string, AuthenticatedEncryptionProviderDescriptor> SupportedEncryptionAlgorithms { get; } = new Dictionary<string, AuthenticatedEncryptionProviderDescriptor>();
    }
}
