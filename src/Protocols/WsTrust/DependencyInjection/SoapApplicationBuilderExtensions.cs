using Microsoft.AspNetCore.Builder;
using Solid.Extensions.AspNetCore.Soap.Builder;
using Solid.Identity.Protocols.WsSecurity.Middleware;
using Solid.Identity.Protocols.WsTrust;
using Solid.Identity.Protocols.WsTrust.Middleware;

namespace Microsoft.Extensions.DependencyInjection;

internal static class SoapApplicationBuilderExtensions
{
    public static void UseTrust13(this ISoapApplicationBuilder soap, WsTrustContractOptions options)
    {
        soap.UseMiddleware<WsTrustEndpointMiddleware>(options);
        soap.UseMiddleware<WsSecurityMiddleware>();
    }
}