using System.Security.Cryptography;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Default echo challenge processor.</summary>
    public sealed class EchoBinaryExchangeProcessor : IBinaryExchangeProcessor
    {
        public string ValueType => WsTrustNegotiation.EchoValueType;

        public BinaryExchangeStep Begin(BinaryExchange exchange)
        {
            var challenge = new byte[32];
            RandomNumberGenerator.Fill(challenge);
            return BinaryExchangeStep.Challenge(challenge, challenge);
        }

        public BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange)
        {
            if (state == null || exchange?.Data == null || state.Length != exchange.Data.Length ||
                !CryptographicOperations.FixedTimeEquals(state, exchange.Data))
                throw new InvalidRequestException("Incorrect binary challenge response.");
            return BinaryExchangeStep.Completed();
        }
    }
}
