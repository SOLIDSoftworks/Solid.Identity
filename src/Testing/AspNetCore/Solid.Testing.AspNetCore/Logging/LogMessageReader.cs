using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Logging
{
    public class LogMessageReader
    {
        private AspNetCoreHostOptions _options;
        private IServiceProvider _services;
        private LogMessageChannel _channel;

        public LogMessageReader(IServiceProvider services, LogMessageChannel channel, IOptionsMonitor<AspNetCoreHostOptions> monitor)
        {
            _options = monitor.CurrentValue;
            _services = services;
            _channel = channel;
        }

        public void Start()
        {
            _ = Task.Factory.StartNew(async () =>
            {
                await ReadMessagesAsync();
            }, TaskCreationOptions.LongRunning);
        }

        public bool MessagesAvailable => _channel.MessagesWaiting;

        private async Task ReadMessagesAsync()
        {
            while (!_channel.Completed)
            {
                var message = await _channel.ReadAsync();
                if (message == null) break; // can't read
                try
                {
                    _options.OnLogMessage?.Invoke(message);
                }
                catch { }
            }
        }
    }
}
