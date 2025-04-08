using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Solid.Extensions.AspNetCore.Soap.Builder;
using Solid.Extensions.AspNetCore.Soap.Middleware;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding SOAP service middleware.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Maps a SOAP endpoint to a <paramref name="path" />.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IApplicationBuilder" /> instance so that additional calls can be chained.</returns>
        public static IApplicationBuilder MapSoapService<TService>(this IApplicationBuilder builder, PathString path)
            => builder.MapSoapService<TService>(path, MessageVersion.Default);

        /// <summary>
        /// Maps a SOAP endpoint to a <paramref name="path" />.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <param name="version">The <see cref="MessageVersion" /> of the SOAP endpoint being mapped.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IApplicationBuilder" /> instance so that additional calls can be chained.</returns>
        public static IApplicationBuilder MapSoapService<TService>(this IApplicationBuilder builder, PathString path, MessageVersion version)
            => builder.MapSoapService<TService>(path, version, _ => {});

        /// <summary>
        /// Maps a SOAP endpoint to a <paramref name="path" />.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <param name="configure">A delegate for adding middleware into the SOAP request pipeline.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IApplicationBuilder" /> instance so that additional calls can be chained.</returns>
        public static IApplicationBuilder MapSoapService<TService>(this IApplicationBuilder builder, PathString path, Action<ISoapApplicationBuilder> configure)
            => builder.MapSoapService<TService>(path, MessageVersion.Default, configure);

        /// <summary>
        /// Maps a SOAP endpoint to a <paramref name="path" />.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder" /> instance.</param>
        /// <param name="path">The path to map <typeparamref name="TService" /> to.</param>
        /// <param name="version">The <see cref="MessageVersion" /> of the SOAP endpoint being mapped.</param>
        /// <param name="configure">A delegate for adding middleware into the SOAP request pipeline.</param>
        /// <typeparam name="TService">The SOAP service contract.</typeparam>
        /// <returns>The <see cref="IApplicationBuilder" /> instance so that additional calls can be chained.</returns>
        public static IApplicationBuilder MapSoapService<TService>(this IApplicationBuilder builder, PathString path, MessageVersion version, Action<ISoapApplicationBuilder> configure)
        {
            builder.Map(path, b => b.UseSoapService<TService>(path, version, configure));
            return builder;
        }

        internal static IApplicationBuilder UseSoapService<TService>(this IApplicationBuilder builder, PathString path, MessageVersion version, Action<ISoapApplicationBuilder> configure)
        {
            if(version == MessageVersion.None)
                throw new InvalidOperationException("Solid.Extensions.AspNetCore.Soap doesn't support MessageVersion.None");
            builder.UseMiddleware<SoapRequestMiddleware<TService>>(version);
            var soap = new SoapApplicationBuilder<TService>(path, builder);
            configure(soap);
            builder.UseMiddleware<MustUnderstandMiddleware>();
            builder.UseMiddleware<SoapServiceInvokerMiddleware>();
            return builder;
        }
    }
}
