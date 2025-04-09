using Solid.Http;
using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Testing.Exceptions;
using System.Net.Http;
using System.Threading.Tasks;

namespace Solid.Http
{
    /// <summary>
    /// Extensions for ISolidHttpRequest
    /// </summary>
    public static class SolidHttpRequestExtensions
    {
        /// <summary>
        /// Performs an assertion against the http response message
        /// </summary>
        /// <param name="request">The Solid.Http request</param>
        /// <param name="assert">The assertion action</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion Should(this ISolidHttpRequest request, Action<HttpResponseMessage> assert)
            => request.Should(response =>
            {
                assert(response);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Performs an async assertion against the http response message
        /// </summary>
        /// <param name="request">The Solid.Http request</param>
        /// <param name="assert">The async assertion action</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion Should(this ISolidHttpRequest request, Func<HttpResponseMessage, Task> assert)
        {
            var assertion = new Assertion(request);
            assertion.Request.OnHttpResponse(async (_, response) => await assert(response));
            return assertion;
        }

        /// <summary>
        /// Asserts the status code of the response
        /// </summary>
        /// <param name="request">The Solid.Http request</param>
        /// <param name="statusCode">The expected status code</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion ShouldRespondWith(this ISolidHttpRequest request, HttpStatusCode statusCode)
        {
            return request.ShouldRespondWith((int)statusCode);
        }

        /// <summary>
        /// Asserts the status code of the response
        /// </summary>
        /// <param name="request">The Solid.Http request</param>
        /// <param name="statusCode">The expected status code</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion ShouldRespondWith(this ISolidHttpRequest request, int statusCode)
        {
            return request.Should(response => response.AssertStatusCode(statusCode));
        }

        /// <summary>
        /// Asserts that the status code of the response is successful (200-299)
        /// </summary>
        /// <param name="request">The Solid.Http request</param>
        /// <returns>The fluent assertion</returns>
        public static Assertion ShouldRespondSuccessfully(this ISolidHttpRequest request)
        {
            return request.Should(response => response.AssertSuccessful());
        }
    }
}
