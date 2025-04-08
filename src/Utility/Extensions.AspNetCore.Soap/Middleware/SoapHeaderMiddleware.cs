using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Middleware
{
    /// <summary>
    /// Middleware that is specifically designed to handle SOAP headers.
    /// </summary>
    public abstract class SoapHeaderMiddleware : SoapMiddleware
    {
        /// <summary>
        /// Creates an instance of <see cref="SoapHeaderMiddleware" />.
        /// </summary>
        /// <param name="next">The next <see cref="RequestDelegate" /> in the request pipeline.</param>
        /// <param name="logger">An <see cref="ILogger" /> instance.</param>
        protected SoapHeaderMiddleware(RequestDelegate next, ILogger logger)
            : base(next, logger)
        {
        }

        /// <summary>
        /// The namespace URI of the SOAP header.
        /// </summary>
        protected abstract string Namespace { get; }

        /// <summary>
        /// The local name of the SOAP header.
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        /// A method for handling the SOAP header described in <see cref="Namespace" /> and <seealso cref="Name" />.
        /// </summary>
        /// <param name="context">The <see cref="SoapContext" /> of the current SOAP request.</param>
        /// <param name="info">The <see cref="MessageHeaderInfo" /> of the header currently being handled.</param>
        /// <param name="index">The index of the header, currently being handled, in the Headers element of the current SOAP request.</param>
        /// <returns>A flag which represents whether the header was understood or not.</returns>
        protected abstract ValueTask<bool> HandleHeaderAsync(SoapContext context, MessageHeaderInfo info, int index);

        /// <summary>
        /// Attempts to handle the SOAP header described in <see cref="Namespace" /> and <seealso cref="Name" />.
        /// </summary>
        /// <param name="context">The <see cref="SoapContext" /> of the current SOAP request.</param>
        /// <returns>An awaitable <see cref="ValueTask" />.</returns>
        protected override async ValueTask InvokeAsync(SoapContext context)
        {
            var index = FindHeaderIndex(context, Name, Namespace);
            if(index >= 0)
            {
                using var activity = Tracing.Soap.StartActivity($"{nameof(SoapHeaderMiddleware)}.{nameof(InvokeAsync)}");
                Logger.LogDebug($"Attempting to handle header '{Name}'.");

                var header = context.Request.Headers[index];
                var handled = false;
                try
                {
                    handled = await HandleHeaderAsync(context, header, index);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, $"Could not understand header '{Name}'.");
                }
                finally
                {
                    if (handled)
                        context.Request.Headers.UnderstoodHeaders.Add(header);
                }
            }
            await Next(context);
        }

        /// <summary>
        /// Find the header element within the Headers element of the current SOAP request.
        /// </summary>
        /// <param name="context">The <see cref="SoapContext" /> of the current SOAP request.</param>
        /// <param name="name">The name of the SOAP header.</param>
        /// <param name="ns">The namespace URI of the SOAP header.</param>
        /// <returns>The index of the header in the Headers element of the current SOAP request.</returns>
        protected virtual int FindHeaderIndex(SoapContext context, string name, string ns)
            => context.Request.Headers.FindHeader(name, ns);
    }
}
