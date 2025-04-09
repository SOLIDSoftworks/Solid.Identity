using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Logging
{
    internal class ChannelLoggerProvider : ILoggerProvider
    {
        private LogMessageChannel _channel;
        private IServiceProvider _services;
        private ChannelLoggerOptions _options;

        public ChannelLoggerProvider(LogMessageChannel channel, IServiceProvider services, IOptionsMonitor<ChannelLoggerOptions> monitor)
        {
            _channel = channel;
            _services = services;
            _options = monitor.CurrentValue;
        }
        public ILogger CreateLogger(string categoryName) => new ChannelLogger(_options, _services, _channel, categoryName);

        public void Dispose()
        {
        }
    }
}
