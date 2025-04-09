using Solid.Testing.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Models
{
    /// <summary>
    /// An in-memory hosted service wrapper.
    /// </summary>
    public class InMemoryHost : IInMemoryHost, IDisposable
    {
        private IDisposable _host;

        /// <summary>
        /// Create an in-memory host wrapper.
        /// </summary>
        /// <param name="host">The in-memory host.</param>
        /// <param name="baseAddress">The base address of the in-memory host.</param>
        public InMemoryHost(IDisposable host, Uri baseAddress)
        {
            _host = host;
            BaseAddress = baseAddress;
        }

        /// <summary>
        /// The base address of the in-memory host
        /// </summary>
        public Uri BaseAddress { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
