using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Solid.Extensions.AspNetCore.Soap;
using Solid.Extensions.AspNetCore.Soap.Middleware;
using Solid.Identity.Protocols.WsSecurity.Middleware;

namespace Solid.Identity.Protocols.WsTrust.Middleware;

public class WsTrustEndpointMiddleware : SoapMiddleware
{
    private readonly WsTrustContractOptions _options;
    private readonly RequestDelegate _next;

    public WsTrustEndpointMiddleware(WsTrustContractOptions options, RequestDelegate next, ILogger<WsTrustEndpointMiddleware> logger)
        : base(next, logger)
    {
        _options = options;
        _next = next;
    }

    protected override async ValueTask InvokeAsync(SoapContext context)
    {
        using var activity = Tracing.WsTrust.Base.StartActivity($"{nameof(WsTrustEndpointMiddleware)}.{nameof(InvokeAsync)}");
        WsTrustContractOptions.Current.Value = _options;
        await Next.Invoke(context);
        WsTrustContractOptions.Current.Value = null;
    }
}