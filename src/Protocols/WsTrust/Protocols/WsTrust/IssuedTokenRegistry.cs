using System;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using Solid.Identity.Protocols.WsSecurity;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Tracks lifecycle state for tokens issued by this STS using a configurable store.</summary>
    public class IssuedTokenRegistry
    {
        private readonly TimeProvider _clock;
        public IIssuedTokenStore Store { get; }

        public IssuedTokenRegistry(TimeProvider clock) : this(clock, new InMemoryIssuedTokenStore(clock)) { }

        public IssuedTokenRegistry(TimeProvider clock, IIssuedTokenStore store)
        {
            _clock = clock;
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public void Register(XmlElement token, ClaimsPrincipal owner, string appliesTo, DateTime expires, string tokenType, string keyType)
        {
            if (token == null) throw new InvalidRequestException("Issued token XML is required for lifecycle tracking.");
            var subject = GetOwner(owner);
            var digest = GetDigest(token);
            Store.Register(GetKey(digest), new Entry(subject, appliesTo, expires, tokenType, keyType, digest));
        }

        public bool TryGet(XmlElement token, out Entry entry)
        {
            entry = null;
            if (token == null) return false;
            var digest = GetDigest(token);
            return Store.TryGet(GetKey(digest), out entry) &&
                CryptographicOperations.FixedTimeEquals(digest, entry.Digest);
        }

        public bool IsOwner(Entry entry, ClaimsPrincipal principal)
            => entry != null && string.Equals(entry.Owner, GetOwner(principal), StringComparison.Ordinal);

        public bool TryCancel(XmlElement token, Entry entry)
        {
            if (token == null || entry == null) return false;
            return !entry.Cancelled && Store.TryUpdate(GetKey(GetDigest(token)), entry, entry.Cancel());
        }

        public bool TryReplace(XmlElement oldToken, Entry entry, XmlElement newToken, ClaimsPrincipal principal)
        {
            if (oldToken == null || newToken == null || entry == null) return false;
            var owner = GetOwner(principal);
            var newDigest = GetDigest(newToken);
            var oldKey = GetKey(GetDigest(oldToken));
            var newKey = GetKey(newDigest);
            if (entry.Cancelled || entry.Expires <= _clock.GetUtcNow().UtcDateTime || entry.Owner != owner || oldKey == newKey)
                return false;
            // The replacement was registered by IssueAsync. The store arbitrates competing renewals/cancellations.
            if (!Store.TryGet(newKey, out var replacement) || replacement.Owner != owner ||
                !CryptographicOperations.FixedTimeEquals(newDigest, replacement.Digest)) return false;
            return Store.TryUpdate(oldKey, entry, entry.Cancel());
        }

        public bool TryRemove(XmlElement token, Entry entry)
        {
            if (token == null || entry == null) return false;
            return Store.TryRemove(GetKey(GetDigest(token)), entry);
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
            public Entry(string owner, string appliesTo, DateTime expires, string tokenType, string keyType, byte[] digest, bool cancelled = false, Guid revision = default)
            {
                Owner = owner;
                AppliesTo = appliesTo;
                Expires = expires;
                TokenType = tokenType;
                KeyType = keyType;
                Cancelled = cancelled;
                _digest = (byte[])digest.Clone();
                Revision = revision == Guid.Empty ? Guid.NewGuid() : revision;
            }

            public string Owner { get; }
            public string AppliesTo { get; }
            public DateTime Expires { get; }
            public bool Cancelled { get; }
            public string TokenType { get; }
            public string KeyType { get; }
            private readonly byte[] _digest;
            /// <summary>SHA-256 digest of the canonical issued token. Persist this with the entry in shared stores.</summary>
            public byte[] Digest => (byte[])_digest.Clone();
            /// <summary>Unique revision used for atomic store transitions across registry instances.</summary>
            public Guid Revision { get; }
            internal Entry Cancel() => new Entry(Owner, AppliesTo, Expires, TokenType, KeyType, _digest, true);
        }
    }
}
