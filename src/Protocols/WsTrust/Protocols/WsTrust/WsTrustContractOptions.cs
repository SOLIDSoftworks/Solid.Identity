using System.Threading;
using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsFederation;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust;

public class WsTrustContractOptions
{
    internal static readonly AsyncLocal<WsTrustContractOptions> Current = new ();
    
    public WsAddressingConstants AddressingConstants { get; set; }

    public WsFederationConstants FedConstants { get; set; }
       
    public WsSecurityPolicyConstants PolicyConstants { get; set; }

    public WsSecurityConstants SecurityConstants { get; set; }

    public WsTrustActions TrustActions { get; set; }

    public WsTrustConstants TrustConstants { get; set; }

    public WsTrustKeyTypes TrustKeyTypes { get; set; }

    public WsTrustConstants TrustVersion { get; internal set; }

    public static WsTrustContractOptions DefaultTrust13Contract => new ()
    {
        TrustVersion = WsTrustConstants.Trust13,
        AddressingConstants = WsAddressingConstants.Addressing10,
        FedConstants = WsFederationConstants.Federation12,
        PolicyConstants = WsSecurityPolicyConstants.SecurityPolicy12,
        SecurityConstants = WsSecurityConstants.WsSecurity11,
        TrustActions = WsTrustActions.Trust13,
        TrustConstants = WsTrustConstants.Trust13,
        TrustKeyTypes = WsTrustKeyTypes.Trust13
    };
    public static WsTrustContractOptions DefaultTrust14Contract => new()
    {
        TrustVersion = WsTrustConstants.Trust14,
        AddressingConstants = WsAddressingConstants.Addressing10,
        FedConstants = WsFederationConstants.Federation12,
        PolicyConstants = WsSecurityPolicyConstants.SecurityPolicy12,
        SecurityConstants = WsSecurityConstants.WsSecurity11,
        TrustActions = WsTrustActions.Trust14,
        TrustConstants = WsTrustConstants.Trust14,
        TrustKeyTypes = WsTrustKeyTypes.Trust14
    };
    public static WsTrustContractOptions DefaultTrustFeb2005Contract => new ()
    {
        TrustVersion = WsTrustConstants.TrustFeb2005,
        AddressingConstants = WsAddressingConstants.Addressing10,
        FedConstants = WsFederationConstants.Federation12,
        PolicyConstants = WsSecurityPolicyConstants.SecurityPolicy12,
        SecurityConstants = WsSecurityConstants.WsSecurity10,
        TrustActions = WsTrustActions.TrustFeb2005,
        TrustConstants = WsTrustConstants.TrustFeb2005,
        TrustKeyTypes = WsTrustKeyTypes.TrustFeb2005,
    };
    
}
