using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.WsSecurity.Middleware;
using Solid.Identity.Protocols.WsTrust.WsTrust13;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using Solid.Extensions.AspNetCore.Soap;
using Solid.Identity.Protocols.WsTrust;
using Solid.Identity.Protocols.WsTrust.Middleware;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EndpointBuilderExtensions
    {
        public static IEndpointRouteBuilder MapWsTrust13AsyncService(this IEndpointRouteBuilder builder)
            => builder.MapWsTrust13AsyncService(WsTrustDefaults.DefaultTrust13Path);
        public static IEndpointRouteBuilder MapWsTrust13AsyncService(this IEndpointRouteBuilder builder, WsTrustContractOptions options)
            => builder.MapWsTrust13AsyncService(WsTrustDefaults.DefaultTrust13Path, options);
        public static IEndpointRouteBuilder MapWsTrust13AsyncService(this IEndpointRouteBuilder builder, PathString path)
            => builder.MapWsTrust13AsyncService(path, WsTrustContractOptions.DefaultTrust13Contract);
        public static IEndpointRouteBuilder MapWsTrust13AsyncService(this IEndpointRouteBuilder builder, PathString path, WsTrustContractOptions options)
        {
            builder.ServiceProvider.InitializeCustomCryptoProvider();
            builder.MapSoapService<IWsTrust13AsyncContract>(path, MessageVersion.Soap12WSAddressing10, soap =>
            {
                soap.UseTrust13(options);
            });
            return builder;
        }
    }
}