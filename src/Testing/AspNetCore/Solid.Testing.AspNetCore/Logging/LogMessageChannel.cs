using Microsoft.Extensions.Options;
using Solid.Testing.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Logging
{
    public class LogMessageChannel : IDisposable
    {
        private Channel<LogMessageContext> _channel;
        private int _messages = 0;

        public LogMessageChannel()
        {
            var options = new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            };
            _channel = Channel.CreateUnbounded<LogMessageContext>(options);
            _channel.Reader.Completion.ContinueWith(_ => Completed = true);
        }

        public bool Completed { get; private set; }

        public void Enqueue(LogMessageContext message)
        {
            if (!Completed && _channel.Writer.TryWrite(message))
                _messages++;
        }

        public async ValueTask<LogMessageContext> ReadAsync()
        {
            if (!await _channel.Reader.WaitToReadAsync()) 
                return null;
            var context = await _channel.Reader.ReadAsync();
            _messages--;
            return context;
        }

        public void Dispose()
        {
            if (!_channel.Writer.TryComplete())
                Completed = true;
        }

        public bool MessagesWaiting => _messages > 0;
    }
}
