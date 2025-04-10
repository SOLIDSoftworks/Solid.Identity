using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Logging
{
    internal class ChannelLogger : ILogger
    {
        private ChannelLoggerOptions _options;
        private IServiceProvider _services;
        private LogMessageChannel _channel;
        private string _category;

        public ChannelLogger(ChannelLoggerOptions options, IServiceProvider services, LogMessageChannel channel, string category)
        {
            _options = options;
            _services = services;
            _channel = channel;
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state)
            => new DelegateScopeOutput<TState>(state, LogScopeOutput);

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) 
            => Log<TState>(logLevel.ToString().ToLower(), eventId, state, exception, formatter);
        private void Log<TState>(string logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = $"[{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}][{logLevel.PadRight(12)}] - {formatter(state, exception)} ({_category})";
            if (exception != null)
                message = message + Environment.NewLine + exception.ToString();
            var context = new LogMessageContext(message);
            _options.OnCreatingLogMessage(_services, context);
            _channel.Enqueue(context);
        }

        private void LogScopeOutput<TState>(string logLevel, TState state)
            => Log(logLevel, 0, state, null, (s, _) => s.ToString());

    }
}
