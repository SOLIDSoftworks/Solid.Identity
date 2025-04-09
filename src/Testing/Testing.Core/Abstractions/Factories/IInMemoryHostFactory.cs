using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Abstractions.Factories
{
    /// <summary>
    /// A factory to create in memory hosts
    /// </summary>
    public interface IInMemoryHostFactory
    {
        /// <summary>
        /// Create a host with a startup type
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        IInMemoryHost CreateHost(Type startup);
    }
}
