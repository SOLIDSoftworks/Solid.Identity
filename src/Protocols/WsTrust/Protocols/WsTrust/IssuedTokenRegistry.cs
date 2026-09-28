using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using Solid.Identity.Protocols.WsSecurity;
using Solid.Identity.Protocols.WsTrust.Exceptions;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Bounded, in-memory lifecycle state for tokens issued by this STS.</summary>
    public class IssuedTokenRegistry
    {
        private const int MaxEntries = 1024;
        private readonly ConcurrentDictionary<string, Entry> _tokens = new ConcurrentDictionary<string, Entry>();
        private readonly object _gate = new object();
        private readonly TimeProvider _clock;

        public IssuedTokenRegistry(TimeProvider clock) => _clock = clock;

        public void Register(XmlElement token, ClaimsPrincipal owner, string appliesTo, DateTime expires, string tokenType, string keyType)
        {
            if (token == null) throw new InvalidRequestException("Issued token XML is required for lifecycle tracking.");
            var subject = GetOwner(owner);
            var digest = GetDigest(token);
            lock (_gate)
            {
                Prune();
                if (expires <= _clock.GetUtcNow().UtcDateTime)
                    throw new InvalidRequestException("Issued token has already expired.");
                if (_tokens.Count >= MaxEntries && !_tokens.ContainsKey(GetKey(digest)))
                    throw new InvalidRequestException("Issued-token registry capacity exceeded.");
                _tokens[GetKey(digest)] = new Entry(subject, appliesTo, expires, tokenType, keyType, digest);
            }
        }

        public bool TryGet(XmlElement token, out Entry entry)
        {
            entry = null;
            if (token == null) return false;
            var digest = GetDigest(token);
            lock (_gate)
            {
                Prune();
                return _tokens.TryGetValue(GetKey(digest), out entry) &&
                    CryptographicOperations.FixedTimeEquals(digest, entry.Digest);
            }
        }

        public bool IsOwner(Entry entry, ClaimsPrincipal principal)
            => entry != null && string.Equals(entry.Owner, GetOwner(principal), StringComparison.Ordinal);

        public bool TryCancel(XmlElement token, Entry entry)
        {
            if (token == null || entry == null) return false;
            lock (_gate)
            {
                var key = GetKey(GetDigest(token));
                if (!_tokens.TryGetValue(key, out var current) || !ReferenceEquals(current, entry) || entry.Cancelled)
                    return false;
                _tokens[key] = entry.Cancel();
                return true;
            }
        }

        public bool TryReplace(XmlElement oldToken, Entry entry, XmlElement newToken, ClaimsPrincipal principal)
        {
            if (oldToken == null || newToken == null || entry == null) return false;
            var owner = GetOwner(principal);
            var newDigest = GetDigest(newToken);
            var oldKey = GetKey(GetDigest(oldToken));
            var newKey = GetKey(newDigest);
            lock (_gate)
            {
                if (!_tokens.TryGetValue(oldKey, out var current) || !ReferenceEquals(current, entry) ||
                    entry.Cancelled || entry.Expires <= _clock.GetUtcNow().UtcDateTime || entry.Owner != owner)
                    return false;
                if (oldKey == newKey) return false;
                // The replacement was registered by IssueAsync. Publish its lifecycle transition only after
                // issuance has succeeded and only if the old entry is still current.
                if (!_tokens.TryGetValue(newKey, out var replacement) || replacement.Owner != owner ||
                    !CryptographicOperations.FixedTimeEquals(newDigest, replacement.Digest)) return false;
                _tokens[oldKey] = entry.Cancel();
                return true;
            }
        }

        public bool TryRemove(XmlElement token, Entry entry)
        {
            if (token == null || entry == null) return false;
            lock (_gate)
            {
                var key = GetKey(GetDigest(token));
                if (!_tokens.TryGetValue(key, out var current) || !ReferenceEquals(current, entry)) return false;
                return _tokens.TryRemove(key, out _);
            }
        }

        private void Prune()
        {
            var now = _clock.GetUtcNow().UtcDateTime;
            foreach (var pair in _tokens)
                if (pair.Value.Expires <= now)
                    _tokens.TryRemove(pair.Key, out _);
        }

        public static string GetOwner(ClaimsPrincipal principal)
        {
            var identity = principal?.Identity as ClaimsIdentity;
            var subjectClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);
            var subject = subjectClaim?.Value;
            if (identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(subject))
                throw new InvalidRequestException("Lifecycle operations require an authenticated NameIdentifier.");
            var issuer = identity.FindFirst(WsSecurityClaimTypes.Issuer)?.Value ?? subjectClaim.Issuer;
            var method = identity.FindFirst(ClaimTypes.AuthenticationMethod)?.Value ?? identity.AuthenticationType;
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(method) ||
                subject.Length > 512 || issuer.Length > 512 || method.Length > 512)
                throw new InvalidRequestException("Lifecycle identity is missing or oversized.");
            return $"{subject.Length}:{subject}{issuer.Length}:{issuer}{method.Length}:{method}";
        }

        private static string GetKey(byte[] digest) => Convert.ToHexString(digest);

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
