using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.IdentityModel.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust.Tests.Host;

public sealed class FinalBinaryExchangeSecurityTokenService : SecurityTokenService
{
    public FinalBinaryExchangeSecurityTokenService(
        IdentityProviderProvider identityProviders, RelyingPartyProvider relyingParties,
        IncomingClaimsMapper mapper, OutgoingSubjectFactory subjectFactory,
        SecurityTokenHandlerProvider securityTokenHandlerProvider, IServiceProvider services,
        ILoggerFactory loggerFactory, IOptions<WsTrustOptions> options, TimeProvider clock)
        : base(identityProviders, relyingParties, mapper, subjectFactory, securityTokenHandlerProvider,
            services, loggerFactory, options, clock) { }

    public override async ValueTask<WsTrustResponse> IssueAsync(ClaimsPrincipal principal, WsTrustRequest request,
        CancellationToken cancellationToken)
    {
        var result = await base.IssueAsync(principal, request, cancellationToken);
        if (request.Context?.StartsWith("final-binary-", StringComparison.Ordinal) == true)
            result.RequestSecurityTokenResponseCollection[0].BinaryExchange =
                new BinaryExchange(new byte[] { 3 }, WsTrustNegotiation.EchoValueType);
        return result;
    }
}
