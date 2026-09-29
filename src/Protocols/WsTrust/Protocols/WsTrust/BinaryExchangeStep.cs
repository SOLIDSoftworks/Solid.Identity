using System;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Next challenge and state, or successful completion of a binary exchange.</summary>
    public sealed class BinaryExchangeStep
    {
        private readonly byte[] _challenge;
        private readonly byte[] _state;

        private BinaryExchangeStep(bool complete, byte[] challenge, byte[] state)
        {
            IsComplete = complete;
            _challenge = challenge == null ? null : (byte[])challenge.Clone();
            _state = state == null ? null : (byte[])state.Clone();
        }

        public bool IsComplete { get; }
        public byte[] ChallengeData => _challenge == null ? null : (byte[])_challenge.Clone();
        public byte[] State => _state == null ? null : (byte[])_state.Clone();

        public static BinaryExchangeStep Challenge(byte[] challenge, byte[] state)
            => new BinaryExchangeStep(false, challenge ?? throw new ArgumentNullException(nameof(challenge)),
                state ?? throw new ArgumentNullException(nameof(state)));

        public static BinaryExchangeStep Completed() => new BinaryExchangeStep(true, null, null);
    }
}
