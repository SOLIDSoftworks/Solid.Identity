using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Abstractions
{
    /// <summary>
    /// An in memory host to be used in tests
    /// </summary>
    public interface IInMemoryHost : IDisposable
    {
        /// <summary>
        /// The base address of the in memory host
        /// </summary>
        Uri BaseAddress { get; }
    }
}
