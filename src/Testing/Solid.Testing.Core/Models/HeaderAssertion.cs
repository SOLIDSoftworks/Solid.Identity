using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Http;
using System.Runtime.CompilerServices;

namespace Solid.Testing.Models
{
    /// <summary>
    /// A fluent header assertion.
    /// </summary>
    public class HeaderAssertion : Assertion
    {
        /// <summary>
        /// Create a <see cref="HeaderAssertion"/>.
        /// </summary>
        /// <param name="name">The name of the header</param>
        /// <param name="assertion">The base <see cref="Assertion"/>.</param>
        public HeaderAssertion(string name, Assertion assertion)
            : base(assertion.Request)
        {
            Name = name;
            Assertion = assertion;
        }

        internal string Name { get; }
        /// <summary>
        /// The base <see cref="Assertion"/>.
        /// </summary>
        public Assertion Assertion { get; }
    }
}
