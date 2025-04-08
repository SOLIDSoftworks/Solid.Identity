using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Solid.Extensions.AspNetCore.Soap.Logging;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Middleware
{
    /// <summary>
    /// A function that can process a SOAP request.
    /// </summary>
    /// <param name="context">The <see cref="SoapContext" /> for the request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public delegate Task SoapRequestDelegate(SoapContext context);

    /// <summary>
    /// Middleware that processes a SOAP request.
    /// </summary>
    public abstract class SoapMiddleware
    {
        private RequestDelegate _next;

        /// <summary>
        /// Creates an instance of <see cref="SoapMiddleware" />.
        /// </summary>
        /// <param name="next">The next <see cref="RequestDelegate" /> in the request pipeline.</param>
        /// <param name="logger">An <see cref="ILogger" /> instance.</param>
        public SoapMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            Logger = logger;
            Next = soap => _next(soap.HttpContext);
        }

        /// <summary>
        /// The next middleware in the request pipeline.
        /// </summary>
        protected SoapRequestDelegate Next { get; }

        /// <summary>
        /// An <see cref="ILogger" /> instance.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Processes part of the SOAP request pipeline.
        /// </summary>
        /// <param name="context">The <see cref="SoapContext" /> of the current SOAP request.</param>
        /// <returns>An awaitable <see cref="ValueTask" />.</returns>
        protected abstract ValueTask InvokeAsync(SoapContext context);

        /// <summary>
        /// Processes part of the HTTP request pipeline.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /> of the current HTTP request.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        public Task InvokeAsync(HttpContext context)
        {
            var soap = context.GetSoapContext();
            if (soap == null) _next(context); // TODO: error?

            if(soap?.Response != null)
            {
                LoggerMessages.LogShortCircuitingPipeline(Logger);
                return Task.CompletedTask;
            }
            LoggerMessages.LogInvokingMiddleware(Logger, this.GetType());
            return InvokeAsync(soap).AsTask();
        }
    }
}
