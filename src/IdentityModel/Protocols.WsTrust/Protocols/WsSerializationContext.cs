#pragma warning disable 1591

using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsFed;
using Solid.IdentityModel.Protocols.WsPolicy;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.IdentityModel.Protocols
{
    /// <summary>
    /// Used to remember the prefix, namespace to use / expect when reading and writing WsTrust Requests and Responses.
    /// </summary>
    public class WsSerializationContext
    {
        public WsSerializationContext()
        {
        }
        
        public WsSerializationContext(WsTrustVersion wsTrustVersion)
        {
            TrustVersion = wsTrustVersion;

            if (wsTrustVersion is WsTrustFeb2005Version)
            {
                AddressingConstants = WsAddressingConstants.Addressing10;
                FedConstants = WsFedConstants.Fed12;
                PolicyConstants = WsPolicyConstants.Policy12;
                SecurityConstants = WsSecurityConstants.WsSecurity10;
                TrustActions = WsTrustActions.TrustFeb2005;
                TrustConstants = WsTrustConstants.TrustFeb2005;
                TrustKeyTypes = WsTrustKeyTypes.TrustFeb2005;
            }
            else if (wsTrustVersion is WsTrust13Version)
            {
                AddressingConstants = WsAddressingConstants.Addressing10;
                FedConstants = WsFedConstants.Fed12;
                PolicyConstants = WsPolicyConstants.Policy12;
                SecurityConstants = WsSecurityConstants.WsSecurity11;
                TrustActions = WsTrustActions.Trust13;
                TrustConstants = WsTrustConstants.Trust13;
                TrustKeyTypes = WsTrustKeyTypes.Trust13;
            }
            else if (wsTrustVersion is WsTrust14Version)
            {
                AddressingConstants = WsAddressingConstants.Addressing10;
                FedConstants = WsFedConstants.Fed12;
                PolicyConstants = WsPolicyConstants.Policy12;
                SecurityConstants = WsSecurityConstants.WsSecurity11;
                TrustActions = WsTrustActions.Trust14;
                TrustConstants = WsTrustConstants.Trust14;
                TrustKeyTypes = WsTrustKeyTypes.Trust14;
            }
        }
        
        public WsAddressingConstants AddressingConstants { get; init; }

        public WsFedConstants FedConstants { get; init; }
       
        public WsPolicyConstants PolicyConstants { get; init; }

        public WsSecurityConstants SecurityConstants { get; init; }

        public WsTrustActions TrustActions { get; init; }

        public WsTrustConstants TrustConstants { get; init; }

        public WsTrustKeyTypes TrustKeyTypes { get; init; }

        public WsTrustVersion TrustVersion { get; init; }
    }
}
