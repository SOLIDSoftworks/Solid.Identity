using Solid.Testing.Abstractions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Solid.Testing.Exceptions;
using System.Net;

namespace Solid.Http
{
    /// <summary>
    /// Extension methods for <see cref="Assertion"/>.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Performs an assertion on an <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="assertion">The <see cref="Assertion"/>.</param>
        /// <param name="assert">An action to perform on an <see cref="HttpResponseMessage"/>.</param>
        /// <returns>The <see cref="Assertion"/> so that aditional calls can be chained.</returns>
        public static Assertion Should(this Assertion assertion, Action<HttpResponseMessage> assert)
            => assertion.Should(response =>
            {
                assert(response);
                return new ValueTask();
            });

        /// <summary>
        /// Performs an async assertion on an <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="assertion">The <see cref="Assertion"/>.</param>
        /// <param name="assert">An async func to perform on an <see cref="HttpResponseMessage"/>.</param>
        /// <returns>The <see cref="Assertion"/> so that aditional calls can be chained.</returns>
        public static Assertion Should(this Assertion assertion, Func<HttpResponseMessage, ValueTask> assert)
        {
            assertion.Request.OnHttpResponse(async (provider, response) => await assert(response));
            return assertion;
        }

        /// <summary>
        /// Asserts that a header with the specifed name is in the <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="assertion">The <see cref="Assertion"/>.</param>
        /// <param name="name">The name of the header.</param>
        /// <returns>A <see cref="HeaderAssertion"/> so that aditional calls can be chained.</returns>
        public static HeaderAssertion ShouldHaveResponseHeader(this Assertion assertion, string name)
        {
            assertion.Should(response =>
            {
                var contains = false;
                try
                {
                    contains = response.Headers.Contains(name);
                }
                catch (InvalidOperationException) // if checking on content header
                {
                    if (response.Content != null)
                        contains = response.Content.Headers.Contains(name);
                }
                if(!contains)
                    throw new SolidTestingAssertionException($"Expected '{name}' header");
            });
            return new HeaderAssertion(name, assertion);
        }

        /// <summary>
        /// Asserts the status code of the response.
        /// </summary>
        /// <param name="assertion">The <see cref="Assertion"/>.</param>
        /// <param name="statusCode">The expected status code.</param>
        /// <returns>The <see cref="Assertion"/> so that aditional calls can be chained.</returns>
        public static Assertion ShouldRespondWith(this Assertion assertion, HttpStatusCode statusCode)
        {
            return assertion.ShouldRespondWith((int)statusCode);
        }

        /// <summary>
        /// Asserts the status code of the response.
        /// </summary>
        /// <param name="assertion">The <see cref="Assertion"/>.</param>
        /// <param name="statusCode">The expected status code.</param>
        /// <returns>The <see cref="Assertion"/> so that aditional calls can be chained.</returns>
        public static Assertion ShouldRespondWith(this Assertion assertion, int statusCode)
        {
            return assertion.Should(response => response.AssertStatusCode(statusCode));
        }

        /// <summary>
        /// Asserts that the status code of the response is successful (200-299).
        /// </summary>
        /// <param name="assertion">The <see cref="Assertion"/>.</param>
        /// <returns>The <see cref="Assertion"/> so that aditional calls can be chained.</returns>
        public static Assertion ShouldRespondSuccessfully(this Assertion assertion)
        {
            return assertion.Should(response => response.AssertSuccessful());
        }
    }
}
