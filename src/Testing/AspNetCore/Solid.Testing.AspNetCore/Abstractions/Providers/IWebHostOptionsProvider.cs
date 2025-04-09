using Microsoft.AspNetCore.Hosting;
using Solid.Testing.AspNetCore.Abstractions.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Abstractions.Providers
{
    /// <summary>
    /// The IWebHostBuilder configuration delegate provider
    /// </summary>
    public interface IWebHostOptionsProvider
    {
        /// <summary>
        /// The configuration delegate
        /// </summary>
        Action<IWebHostBuilder> Configure { get; }
    }
}
