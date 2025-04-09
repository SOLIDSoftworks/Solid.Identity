using Solid.Testing.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Solid.Http
{
    internal static class HttpResponseMessageExtensions
    {
        public static IEnumerable<string> GetHeaderValues(this HttpResponseMessage response, string name)
        {
            var values = null as IEnumerable<string>;
            try
            {
                values = response.Headers.GetValues(name);
            }
            catch (InvalidOperationException)
            {
                if (response.Content == null)
                    values = Enumerable.Empty<string>();
                else
                    values = response.Content.Headers.GetValues(name);
            }
            return values;
        }

        public static void AssertSuccessful(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new SolidTestingAssertionException($"Expected successful status code. Got {response.StatusCode} instead.");
        }

        public static void AssertStatusCode(this HttpResponseMessage response, int statusCode)
        {
            if (statusCode != (int)response.StatusCode)
                throw new SolidTestingAssertionException($"Expected {statusCode} status code. Got {response.StatusCode} instead.");
        }
    }
}
