using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust.Abstractions
{
    /// <summary>Maintains pending WS-Trust challenge exchanges across SOAP requests.</summary>
    public interface IWsTrustExchangeStore
    {
        WsTrustResponse Begin(ClaimsPrincipal principal, WsTrustRequest request);
        ValueTask<WsTrustResponse> CompleteAsync(ClaimsPrincipal principal, WsTrustResponse response, ISecurityTokenService sts, CancellationToken cancellationToken);
    }
}
