using Microsoft.AspNetCore.Hosting;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Providers
{
    internal class WebHostOptionsProvider : IWebHostOptionsProvider
    {
        public Action<IWebHostBuilder> Configure { get; set; }
    }
}
