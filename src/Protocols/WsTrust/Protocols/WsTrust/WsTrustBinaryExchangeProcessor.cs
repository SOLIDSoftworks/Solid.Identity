using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;
using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsPolicy;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Coordinates bounded WS-Trust 1.3 challenge exchanges by BinaryExchange ValueType.</summary>
    public sealed class WsTrustBinaryExchangeProcessor
    {
        private const int MaxRounds = 8;
        private const int MaxBytes = 4096;
        private readonly IWsTrustExchangeStore _store;
        private readonly IReadOnlyDictionary<string, IBinaryExchangeProcessor> _processors;
        private readonly TimeProvider _clock;

        public WsTrustBinaryExchangeProcessor(IWsTrustExchangeStore store, IEnumerable<IBinaryExchangeProcessor> processors, TimeProvider clock)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            if (processors == null) throw new ArgumentNullException(nameof(processors));
            var registered = new Dictionary<string, IBinaryExchangeProcessor>(StringComparer.Ordinal);
            foreach (var processor in processors)
            {
                if (processor == null || string.IsNullOrEmpty(processor.ValueType) || !registered.TryAdd(processor.ValueType, processor))
                    throw new InvalidOperationException("Binary exchange processors must have unique nonempty ValueTypes.");
            }
            _processors = registered;
        }

        public WsTrustResponse Begin(ClaimsPrincipal principal, WsTrustRequest request)
        {
            var owner = GetOwner(principal);
            var exchange = request?.BinaryExchange;
            if (request?.RequestType != WsTrustConstants.Trust13.Actions.Issue || request.AppliesTo == null ||
                string.IsNullOrEmpty(request.Context) || request.Context.Length > 128 ||
                exchange == null || exchange.Data.Length == 0 || exchange.Data.Length > MaxBytes)
                throw new InvalidRequestException("Invalid or oversized binary challenge request.");
            if (!_processors.TryGetValue(exchange.ValueType, out var processor))
                throw new InvalidRequestException("Unsupported BinaryExchange ValueType.");

            // Retain only bounded issuance inputs; never retain caller-owned extension XML or key material.
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
                AppliesTo = new AppliesTo(new EndpointReference(audience))
            };
            if (request.TokenType != null) retained.TokenType = request.TokenType;
            if (request.KeyType != null) retained.KeyType = request.KeyType;
            if (request.KeySizeInBits.HasValue) retained.KeySizeInBits = request.KeySizeInBits;

            var step = processor.Begin(exchange);
            if (step == null || step.IsComplete)
                throw new InvalidRequestException("The initial binary exchange must produce a challenge.");
            ValidateChallenge(step);
            var pending = new WsTrustPendingExchange(owner, processor.ValueType, retained, step.State,
                _clock.GetUtcNow().AddMinutes(2), 1);
            if (!_store.TryAdd(request.Context, pending))
                throw new InvalidRequestException("Duplicate Context or too many pending exchanges.");
            return Challenge(request.Context, processor.ValueType, step.ChallengeData);
        }

        public async ValueTask<WsTrustResponse> CompleteAsync(ClaimsPrincipal principal, WsTrustResponse response,
            ISecurityTokenService sts, CancellationToken cancellationToken)
        {
            var responses = response?.RequestSecurityTokenResponseCollection;
            if (responses?.Count != 1 || string.IsNullOrEmpty(responses[0].Context))
                throw new InvalidRequestException("Intermediate RSTR requires one response with Context.");
            var rstr = responses[0];
            if (rstr.Context.Length > 128 || rstr.RequestedSecurityToken != null || rstr.RequestedProofToken != null ||
                rstr.Status != null || rstr.RequestedTokenCancelled || rstr.AdditionalXmlElements.Count != 0 ||
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
            if (!_store.TryGet(rstr.Context, out var pending))
                throw new InvalidRequestException("Unknown or replayed exchange Context.");
            if (pending.Owner != GetOwner(principal))
                throw new SecurityException("Exchange belongs to another requestor.");
            if (pending.Expires <= _clock.GetUtcNow())
            {
                _store.TryRemove(rstr.Context, pending);
                throw new InvalidRequestException("Expired binary challenge response.");
            }
            var exchange = rstr.BinaryExchange;
            if (exchange == null || exchange.ValueType != pending.ValueType ||
                exchange.Data.Length == 0 || exchange.Data.Length > MaxBytes ||
                !_processors.TryGetValue(pending.ValueType, out var processor))
                throw new InvalidRequestException("Unexpected BinaryExchange ValueType or data.");

            var step = processor.Continue(pending.State, exchange);
            if (pending.Expires <= _clock.GetUtcNow())
            {
                _store.TryRemove(rstr.Context, pending);
                throw new InvalidRequestException("Expired binary challenge response.");
            }
            if (step == null) throw new InvalidRequestException("Binary exchange processor returned no result.");
            if (step.IsComplete)
            {
                if (step.ChallengeData != null || step.State != null)
                    throw new InvalidRequestException("Completed binary exchange cannot contain a challenge or state.");
                if (!_store.TryRemove(rstr.Context, pending))
                    throw new InvalidRequestException("Exchange was already completed.");
                return await sts.IssueAsync(principal, pending.Request, cancellationToken);
            }
            if (pending.Round >= MaxRounds)
            {
                _store.TryRemove(rstr.Context, pending);
                throw new InvalidRequestException("Binary exchange round limit exceeded.");
            }
            ValidateChallenge(step);
            var next = new WsTrustPendingExchange(pending.Owner, pending.ValueType, pending.Request,
                step.State, pending.Expires, pending.Round + 1);
            if (!_store.TryUpdate(rstr.Context, pending, next))
                throw new InvalidRequestException("Exchange was already advanced.");
            return Challenge(rstr.Context, pending.ValueType, step.ChallengeData);
        }

        private static void ValidateChallenge(BinaryExchangeStep step)
        {
            if (step.ChallengeData == null || step.ChallengeData.Length == 0 || step.ChallengeData.Length > MaxBytes ||
                step.State == null || step.State.Length > MaxBytes)
                throw new InvalidRequestException("Binary exchange challenge or state exceeds the limit.");
        }

        private static WsTrustResponse Challenge(string context, string valueType, byte[] data)
            => new WsTrustResponse(new RequestSecurityTokenResponse
            {
                Context = context,
                BinaryExchange = new BinaryExchange(data, valueType)
            });

        private static string GetOwner(ClaimsPrincipal principal)
        {
            var identity = principal?.Identity as ClaimsIdentity;
            var subject = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var method = identity?.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            if (identity?.IsAuthenticated != true || string.IsNullOrEmpty(subject) ||
                string.IsNullOrEmpty(identity.AuthenticationType) || string.IsNullOrEmpty(method))
                throw new SecurityException("Negotiation requires a stable authenticated credential identity.");
            var issuer = identity.FindFirst(ClaimTypes.NameIdentifier)?.Issuer ?? string.Empty;
            if (subject.Length > 512 || issuer.Length > 512 || method.Length > 512 || identity.AuthenticationType.Length > 512)
                throw new SecurityException("Credential identity exceeds the exchange limit.");
            return $"{subject.Length}:{subject}{issuer.Length}:{issuer}{method.Length}:{method}{identity.AuthenticationType.Length}:{identity.AuthenticationType}";
        }
    }
}
