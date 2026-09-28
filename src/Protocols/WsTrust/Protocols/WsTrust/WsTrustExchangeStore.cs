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
    public class WsTrustExchangeStore
    {
        private readonly ConcurrentDictionary<string, Pending> _pending = new ConcurrentDictionary<string, Pending>();
        private readonly TimeProvider _clock;

        public WsTrustExchangeStore(TimeProvider clock) => _clock = clock;

        public WsTrustResponse Begin(ClaimsPrincipal principal, WsTrustRequest request)
        {
            if (principal?.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(principal.Identity.Name))
                throw new SecurityException("Negotiation requires an authenticated requestor.");
            if (request?.RequestType != WsTrustConstants.Trust13.Actions.Issue || request.AppliesTo == null ||
                string.IsNullOrEmpty(request.Context) || request.BinaryExchange?.ValueType != WsTrustNegotiation.EchoValueType ||
                request.BinaryExchange.Data.Length == 0 || request.BinaryExchange.Data.Length > 4096)
                throw new InvalidRequestException("Invalid or unsupported binary challenge request.");

            var now = _clock.GetUtcNow();
            foreach (var item in _pending)
                if (item.Value.Expires <= now) _pending.TryRemove(item.Key, out _);
            if (_pending.Count >= 1024 || _pending.ContainsKey(request.Context))
                throw new InvalidRequestException("Duplicate Context or too many pending exchanges.");

            var challenge = new byte[32];
            RandomNumberGenerator.Fill(challenge);
            var pending = new Pending(principal.Identity.Name, request, challenge, now.AddMinutes(2));
            if (!_pending.TryAdd(request.Context, pending))
                throw new InvalidRequestException("Duplicate exchange Context.");
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
                rstr.TokenType != null || rstr.KeyType != null || rstr.Entropy != null)
                throw new InvalidRequestException("Unexpected result in intermediate RSTR.");
            if (!_pending.TryGetValue(rstr.Context, out var pending))
                throw new InvalidRequestException("Unknown or replayed exchange Context.");
            if (pending.Expires <= _clock.GetUtcNow())
            {
                _pending.TryRemove(rstr.Context, out _);
                throw new InvalidRequestException("Expired binary challenge response.");
            }
            if (principal?.Identity?.IsAuthenticated != true || pending.Owner != principal.Identity.Name)
                throw new SecurityException("Exchange belongs to another requestor.");
            if (!_pending.TryRemove(rstr.Context, out pending))
                throw new InvalidRequestException("Exchange was already completed.");
            if (pending.Expires <= _clock.GetUtcNow() || rstr.BinaryExchange?.ValueType != WsTrustNegotiation.EchoValueType ||
                rstr.BinaryExchange.Data.Length != pending.Challenge.Length ||
                !CryptographicOperations.FixedTimeEquals(rstr.BinaryExchange.Data, pending.Challenge))
                throw new InvalidRequestException("Expired or incorrect binary challenge response.");

            // A challenge response must not be interpreted as a second RST.
            return await sts.IssueAsync(principal, pending.Request, cancellationToken);
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
