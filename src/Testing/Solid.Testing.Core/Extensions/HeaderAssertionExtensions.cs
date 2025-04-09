using Solid.Testing.Abstractions;
using Solid.Testing.Exceptions;
using Solid.Testing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solid.Http
{
    /// <summary>
    /// Header value assertion extensions
    /// </summary>
    public static class HeaderAssertionExtensions
    {
        /// <summary>
        /// Asserts whether the header has a specified value
        /// </summary>
        /// <param name="assertion">The header assertion</param>
        /// <param name="value">The header value</param>
        /// <returns>A header assertion</returns>
        public static HeaderAssertion WithValue(this HeaderAssertion assertion, string value)
        {
            return assertion.WithValue(value, StringComparer.Ordinal);
        }

        /// <summary>
        /// Asserts whether the header has a specified value
        /// </summary>
        /// <param name="assertion">The header assertion</param>
        /// <param name="value">The header value</param>
        /// <param name="comparer">An equality comparer to use for the header value</param>
        /// <returns>A header assertion</returns>
        public static HeaderAssertion WithValue(this HeaderAssertion assertion, string value, IEqualityComparer<string> comparer)
        {
            return assertion.WithValueComparer(values => values.Contains(value, comparer), $"Expected value '{value}' not found.");
        }

        /// <summary>
        /// Asserts whether the header starts with a specified value
        /// </summary>
        /// <param name="assertion">The header assertion</param>
        /// <param name="value">The value the header should start with</param>
        /// <returns>A header assertion</returns>
        public static HeaderAssertion WithValueStartingWith(this HeaderAssertion assertion, string value)
        {
            return assertion.WithValueStartingWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Asserts whether the header starts with a specified value
        /// </summary>
        /// <param name="assertion">The header assertion</param>
        /// <param name="value">The value the header should start with</param>
        /// <param name="comparison">A comparison to use for the header value</param>
        /// <returns>A header assertion</returns>
        public static HeaderAssertion WithValueStartingWith(this HeaderAssertion assertion, string value, StringComparison comparison)
        {
            return assertion.WithValueComparer(values => values.Any(v => v.StartsWith(value, comparison)), $"Expected value starting with '{value}' not found.");
        }

        /// <summary>
        /// Asserts whether the header ends with a specified value
        /// </summary>
        /// <param name="assertion">The header assertion</param>
        /// <param name="value">The value the header should end with</param>
        /// <returns>A header assertion</returns>
        public static HeaderAssertion WithValueEndingWith(this HeaderAssertion assertion, string value)
        {
            return assertion.WithValueEndingWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Asserts whether the header ends with a specified value
        /// </summary>
        /// <param name="assertion">The header assertion</param>
        /// <param name="value">The value the header should end with</param>
        /// <param name="comparison">A comparison to use for the header value</param>
        /// <returns>A header assertion</returns>
        public static HeaderAssertion WithValueEndingWith(this HeaderAssertion assertion, string value, StringComparison comparison)
        {
            return assertion.WithValueComparer(values => values.Any(v => v.EndsWith(value, comparison)), $"Expected value starting with '{value}' not found.");
        }

        private static HeaderAssertion WithValueComparer(this HeaderAssertion assertion, Func<IEnumerable<string>, bool> compare, string message)
        {
            assertion.Assertion.Should(response =>
            {
                var header = response.GetHeaderValues(assertion.Name);
                if (!compare(header))
                    throw new SolidTestingAssertionException(message);
            });
            return assertion;
        }
    }
}
