using System;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Immutable state of one pending WS-Trust binary exchange.</summary>
    public sealed class WsTrustPendingExchange
    {
        private readonly byte[] _state;

        public WsTrustPendingExchange(string owner, string valueType, WsTrustRequest request, byte[] state,
            DateTimeOffset expires, int round)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            _state = (byte[])(state ?? throw new ArgumentNullException(nameof(state))).Clone();
            Expires = expires;
            Round = round;
        }

        public string Owner { get; }
        public string ValueType { get; }
        /// <summary>A bounded, detached copy of the original issuance request; stores must not mutate it.</summary>
        public WsTrustRequest Request { get; }
        public byte[] State => (byte[])_state.Clone();
        public DateTimeOffset Expires { get; }
        public int Round { get; }
    }
}
