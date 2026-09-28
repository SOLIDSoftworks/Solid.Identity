using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust.Abstractions
{
    /// <summary>Handles one WS-Trust BinaryExchange ValueType; state must be bounded and serializable.</summary>
    public interface IBinaryExchangeProcessor
    {
        string ValueType { get; }
        BinaryExchangeStep Begin(BinaryExchange exchange);
        BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange);
    }
}
