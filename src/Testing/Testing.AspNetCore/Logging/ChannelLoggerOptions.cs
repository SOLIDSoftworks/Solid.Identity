using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Logging
{
    public class ChannelLoggerOptions
    {
        public Action<IServiceProvider, LogMessageContext> OnCreatingLogMessage { get; set; } = (_, __) => { };
    }
}
