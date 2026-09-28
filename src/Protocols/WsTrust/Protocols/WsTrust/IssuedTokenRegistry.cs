using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.IO;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Tracks issued tokens for lifecycle operations within this STS instance.</summary>
    public class IssuedTokenRegistry
    {
        private readonly ConcurrentDictionary<string, Entry> _tokens = new ConcurrentDictionary<string, Entry>();

        public void Register(XmlElement token, ClaimsPrincipal owner, string appliesTo, DateTime expires, string tokenType, string keyType)
        {
            if (token == null || owner?.Identity?.IsAuthenticated != true) return;
            _tokens[GetKey(token)] = new Entry(owner.Identity.Name, appliesTo, expires, tokenType, keyType, GetDigest(token));
        }

        public bool TryGet(XmlElement token, out Entry entry)
        {
            if (!_tokens.TryGetValue(GetKey(token), out entry)) return false;
            return CryptographicOperations.FixedTimeEquals(GetDigest(token), entry.Digest);
        }

        public bool TryCancel(XmlElement token, Entry entry)
        {
            if (token == null || entry == null) return false;
            return _tokens.TryUpdate(GetKey(token), entry.Cancel(), entry);
        }

        private static string GetKey(XmlElement token)
            => token?.GetAttribute("ID") is string id && id.Length > 0 ? token.NamespaceURI + ":" + id : token?.OuterXml ?? string.Empty;

        private static byte[] GetDigest(XmlElement token)
        {
            var document = new XmlDocument { PreserveWhitespace = true };
            document.LoadXml(token.OuterXml);
            var transform = new XmlDsigExcC14NTransform();
            transform.LoadInput(document);
            using var stream = (Stream)transform.GetOutput(typeof(Stream));
            return SHA256.HashData(stream);
        }

        public sealed class Entry
        {
            public Entry(string owner, string appliesTo, DateTime expires, string tokenType, string keyType, byte[] digest, bool cancelled = false)
            {
                Owner = owner;
                AppliesTo = appliesTo;
                Expires = expires;
                TokenType = tokenType;
                KeyType = keyType;
                Cancelled = cancelled;
                Digest = digest;
            }

            public string Owner { get; }
            public string AppliesTo { get; }
            public DateTime Expires { get; }
            public bool Cancelled { get; }
            public string TokenType { get; }
            public string KeyType { get; }
            internal byte[] Digest { get; }
            internal Entry Cancel() => new Entry(Owner, AppliesTo, Expires, TokenType, KeyType, Digest, true);
        }
    }
}
