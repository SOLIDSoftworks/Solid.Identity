using System;
using System.Collections.Concurrent;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Bounded, single-use WS-Trust 1.3 issue challenge exchanges.</summary>
    public class WsTrustExchangeStore : IWsTrustExchangeStore
    {
        private readonly ConcurrentDictionary<string, Pending> _pending = new ConcurrentDictionary<string, Pending>();
        private readonly object _admissionLock = new object();
        private readonly TimeProvider _clock;

        public WsTrustExchangeStore(TimeProvider clock) => _clock = clock;

        public WsTrustResponse Begin(ClaimsPrincipal principal, WsTrustRequest request)
        {
            var owner = GetOwner(principal);
            if (request?.RequestType != WsTrustConstants.Trust13.Actions.Issue || request.AppliesTo == null ||
                string.IsNullOrEmpty(request.Context) || request.Context.Length > 128 ||
                request.BinaryExchange?.ValueType != WsTrustNegotiation.EchoValueType ||
                request.BinaryExchange.Data.Length == 0 || request.BinaryExchange.Data.Length > 4096)
                throw new InvalidRequestException("Invalid or unsupported binary challenge request.");

            // Keep only the bounded fields needed by IssueAsync; reject all other RST content
            // rather than retaining extension XML or a mutable caller-owned request graph.
            var audience = request.AppliesTo.EndpointReference?.Uri;
            if (string.IsNullOrEmpty(audience) || audience.Length > 2048 ||
                request.TokenType?.Length > 512 || request.KeyType?.Length > 512 ||
                request.AdditionalXmlElements.Count != 0 || request.AdditionalXmlAttributes.Count != 0 ||
                request.SecondaryParameters != null || request.Lifetime != null || request.Claims != null ||
                request.Entropy != null || request.OnBehalfOf != null || request.UseKey != null ||
                request.ProofEncryption != null || request.ActAs != null || request.DelegateTo != null ||
                request.Participants != null || request.PolicyReference != null || request.AdditionalContext != null ||
                request.Issuer != null || request.Encryption != null ||
                request.AuthenticationType != null || request.CanonicalizationAlgorithm != null ||
                request.ComputedKeyAlgorithm != null || request.EncryptionAlgorithm != null ||
                request.EncryptWith != null || request.SignWith != null || request.SignatureAlgorithm != null ||
                request.KeyWrapAlgorithm != null || request.Forwardable.HasValue || request.Delegatable.HasValue ||
                request.AllowPostdating.HasValue || request.Renewing != null ||
                request.RenewTarget != null || request.CancelTarget != null || request.ValidateTarget != null)
                throw new InvalidRequestException("Unsupported or oversized binary challenge request parameters.");
            var retained = new WsTrustRequest(WsTrustConstants.Trust13.Actions.Issue)
            {
                Context = request.Context,
                AppliesTo = new Solid.IdentityModel.Protocols.WsPolicy.AppliesTo(
                    new Solid.IdentityModel.Protocols.WsAddressing.EndpointReference(audience))
            };
            if (request.TokenType != null) retained.TokenType = request.TokenType;
            if (request.KeyType != null) retained.KeyType = request.KeyType;
            if (request.KeySizeInBits.HasValue) retained.KeySizeInBits = request.KeySizeInBits;

            var now = _clock.GetUtcNow();
            var challenge = new byte[32];
            RandomNumberGenerator.Fill(challenge);
            lock (_admissionLock)
            {
                foreach (var item in _pending)
                    if (item.Value.Expires <= now) _pending.TryRemove(item.Key, out _);
                if (_pending.Count >= 1024 || _pending.ContainsKey(request.Context))
                    throw new InvalidRequestException("Duplicate Context or too many pending exchanges.");
                if (!_pending.TryAdd(request.Context, new Pending(owner, retained, challenge, now.AddMinutes(2))))
                    throw new InvalidRequestException("Duplicate exchange Context.");
            }
            return new WsTrustResponse(new RequestSecurityTokenResponse
            {
                Context = request.Context,
                BinaryExchange = new BinaryExchange(challenge, WsTrustNegotiation.EchoValueType)
            });
        }

        public async ValueTask<WsTrustResponse> CompleteAsync(ClaimsPrincipal principal, WsTrustResponse response, ISecurityTokenService sts, CancellationToken cancellationToken)
        {
            var responses = response?.RequestSecurityTokenResponseCollection;
            if (responses?.Count != 1 || string.IsNullOrEmpty(responses[0].Context))
                throw new InvalidRequestException("Intermediate RSTR requires one response with Context.");
            var rstr = responses[0];
            if (rstr.RequestedSecurityToken != null || rstr.RequestedProofToken != null || rstr.Status != null ||
                rstr.RequestedTokenCancelled || rstr.AdditionalXmlElements.Count != 0 ||
                rstr.AdditionalXmlAttributes.Count != 0 || rstr.TokenType != null || rstr.KeyType != null ||
                rstr.Entropy != null || rstr.Lifetime != null || rstr.AppliesTo != null ||
                rstr.KeySizeInBits.HasValue || rstr.AttachedReference != null || rstr.UnattachedReference != null ||
                rstr.Authenticator != null || rstr.SignatureAlgorithm != null ||
                rstr.EncryptionAlgorithm != null || rstr.KeyWrapAlgorithm != null ||
                rstr.CanonicalizationAlgorithm != null || rstr.ComputedKeyAlgorithm != null ||
                rstr.AuthenticationType != null || rstr.EncryptWith != null || rstr.SignWith != null ||
                rstr.OnBehalfOf != null || rstr.UseKey != null || rstr.Claims != null ||
                rstr.ProofEncryption != null || rstr.PolicyReference != null ||
                rstr.AdditionalContext != null || rstr.Issuer != null || rstr.Participants != null ||
                rstr.Encryption != null || rstr.ActAs != null || rstr.DelegateTo != null ||
                rstr.SecondaryParameters != null || rstr.Forwardable.HasValue || rstr.Delegatable.HasValue)
                throw new InvalidRequestException("Unexpected result in intermediate RSTR.");
            Pending pending;
            lock (_admissionLock)
            {
                if (!_pending.TryGetValue(rstr.Context, out pending))
                    throw new InvalidRequestException("Unknown or replayed exchange Context.");
                if (pending.Expires <= _clock.GetUtcNow())
                {
                    _pending.TryRemove(rstr.Context, out _);
                    throw new InvalidRequestException("Expired binary challenge response.");
                }
                if (pending.Owner != GetOwner(principal))
                    throw new SecurityException("Exchange belongs to another requestor.");
                if (!_pending.TryRemove(rstr.Context, out pending))
                    throw new InvalidRequestException("Exchange was already completed.");
            }
            if (pending.Expires <= _clock.GetUtcNow() || rstr.BinaryExchange?.ValueType != WsTrustNegotiation.EchoValueType ||
                rstr.BinaryExchange.Data.Length != pending.Challenge.Length ||
                !CryptographicOperations.FixedTimeEquals(rstr.BinaryExchange.Data, pending.Challenge))
                throw new InvalidRequestException("Expired or incorrect binary challenge response.");

            // A challenge response must not be interpreted as a second RST.
            return await sts.IssueAsync(principal, pending.Request, cancellationToken);
        }

        private static string GetOwner(ClaimsPrincipal principal)
        {
            var identity = principal?.Identity as ClaimsIdentity;
            var subject = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var method = identity?.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            if (identity?.IsAuthenticated != true || string.IsNullOrEmpty(subject) ||
                string.IsNullOrEmpty(identity.AuthenticationType) || string.IsNullOrEmpty(method))
                throw new SecurityException("Negotiation requires a stable authenticated credential identity.");
            // Length-prefixed values avoid collisions when claims contain separators.
            var issuer = identity.FindFirst(ClaimTypes.NameIdentifier)?.Issuer ?? string.Empty;
            if (subject.Length > 512 || issuer.Length > 512 || method.Length > 512 || identity.AuthenticationType.Length > 512)
                throw new SecurityException("Credential identity exceeds the exchange limit.");
            return $"{subject.Length}:{subject}{issuer.Length}:{issuer}{method.Length}:{method}{identity.AuthenticationType.Length}:{identity.AuthenticationType}";
        }

        private sealed class Pending
        {
            public Pending(string owner, WsTrustRequest request, byte[] challenge, DateTimeOffset expires)
            {
                Owner = owner;
                Request = request;
                Challenge = challenge;
                Expires = expires;
            }

            public string Owner { get; }
            public WsTrustRequest Request { get; }
            public byte[] Challenge { get; }
            public DateTimeOffset Expires { get; }
        }
    }
}
