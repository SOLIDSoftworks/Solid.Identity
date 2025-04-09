using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;

namespace Solid.Testing.Models
{
    /// <summary>
    /// A fluent <see cref="Assertion"/> object.
    /// <para>This object is awaitable.</para>
    /// </summary>
    public class Assertion
    {
        /// <summary>
        /// Create an <see cref="Assertion"/>.
        /// </summary>
        /// <param name="request">The <see cref="ISolidHttpRequest"/> to be performed.</param>
        public Assertion(ISolidHttpRequest request)
        {
            Request = request;
        }

        /// <summary>
        /// The <see cref="ISolidHttpRequest"/> to be performed.
        /// </summary>
        public ISolidHttpRequest Request { get; }

        /// <summary>
        /// Gets the <see cref="TaskAwaiter{TResult}" /> of <seealso cref="HttpResponseMessage"/>.
        /// <para>This enables await.</para>
        /// </summary>
        /// <returns>A <see cref="TaskAwaiter{TResult}" /> of <seealso cref="HttpResponseMessage"/>.</returns>
        public TaskAwaiter<HttpResponseMessage> GetAwaiter()
        {
            Func<Assertion, Task<HttpResponseMessage>> waiter = (async r =>
            {
                var response = await Request;
                return response;
            });
            return waiter(this).GetAwaiter();
        }
    }
}
