using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Builder
{
    /// <summary>
    /// An interface that enables extending a SOAP endpoint.
    /// <para>
    /// This is an extension of <see cref="IApplicationBuilder" />.
    /// </para>
    /// </summary>
    public interface ISoapApplicationBuilder : IApplicationBuilder
    {
        /// <summary>
        /// The contract type mapped to the endpoint.
        /// </summary>
        Type Contract { get; }

        /// <summary>
        /// The <see cref="SoapServiceOptions"/> for the service mapped to the endpoint.
        /// </summary>
        SoapServiceOptions Options { get; }

        /// <summary>
        /// The path of the endpoint being mapped.
        /// </summary>
        PathString Path { get; }
    }
}
