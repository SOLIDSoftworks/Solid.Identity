using Solid.Testing.AspNetCore.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Solid.Testing.AspNetCore.Options
{
    public class AspNetCoreHostOptions
    {
        public string Environment { get; set; } = Environments.Development;
        public string HostName { get; set; }
        public Action<LogMessageContext> OnLogMessage { get; set; } = _ => { };
    }
}
