using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Logging
{
    public class LogMessageContext
    {
        public LogMessageContext(string message) => Message = message;
        public string Message { get; }
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    }
}
