using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Testing.AspNetCore.Options;

namespace Solid.Testing.AspNetCore.Abstractions.Factories
{
    /// <summary>
    /// The asp net core web host factory
    /// </summary>
    public interface IWebHostFactory
    {
        /// <summary>
        /// Create an asp net core web host using a startup class and a hostname
        /// </summary>
        /// <param name="startup">The startup class type</param>
        /// <param name="options">The AspNetCore host options</param>
        /// <returns>An asp net core web host</returns>
        IWebHost CreateWebHost(Type startup, AspNetCoreHostOptions options);
    }
}
