using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;

namespace Solid.Extensions.AspNetCore.Soap
{
    internal class SoapContext<TContract> : SoapContext
    {
        public SoapContext(HttpContext http, Message request, MessageVersion version, SoapServiceOptions options) 
            : base(http, request, version, options)
        {
        }

        public override Type Contract => typeof(TContract);
    }

    /// <summary>
    /// Encapsulates all SOAP-specific information about an individual SOAP request.
    /// </summary>
    public abstract class SoapContext
    {
        internal SoapContext(HttpContext http, Message request, MessageVersion version, SoapServiceOptions options)
        {
            HttpContext = http;
            Request = request;
            MessageVersion = version;
            Options = options;
        }

        /// <summary>
        /// The <see cref="HttpContext" /> of the incoming AspNetCore http request.
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// The SOAP service contract type.
        /// </summary>
        public abstract Type Contract { get; }

        /// <summary>
        /// The value of the content-type http header.
        /// </summary>
        public string ContentType => HttpContext.Request.ContentType;

        /// <summary>
        /// The <see cref="SoapServiceOptions" /> instance for the SOAP service contract being invoked.
        /// </summary>
        public SoapServiceOptions Options { get; }

        /// <summary>
        /// The SOAP <see cref="MessageVersion" /> of the endpoint being invoked.
        /// </summary>
        public MessageVersion MessageVersion { get; }

        /// <summary>
        /// The <see cref="IServiceProvider" /> scoped for the current http request.
        /// </summary>
        public IServiceProvider RequestServices => HttpContext.RequestServices;

        /// <summary>
        /// The request SOAP <see cref="Message" />.
        /// </summary>
        public Message Request { get; set; }

        /// <summary>
        /// The response SOAP <see cref="Message" />.
        /// </summary>
        public Message Response { get; set; }

        /// <summary>
        /// The current user on the request.
        /// </summary>
        public ClaimsPrincipal User { get => HttpContext.User; set => HttpContext.User = value; }

        /// <summary>
        /// The <see cref="CancellationToken" /> for the http request.
        /// </summary>
        public CancellationToken CancellationToken => HttpContext.RequestAborted;
    }
}
