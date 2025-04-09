using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.ServiceModel
{
    class TraceMessageEncoder : MessageEncoder
    {
        private MessageEncoder _inner;
        private ILogger<TraceMessageEncoder> _logger;

        public TraceMessageEncoder(MessageEncoder inner, ILoggerFactory loggerFactory)
        {
            _inner = inner;
            _logger = loggerFactory.CreateLogger<TraceMessageEncoder>();
        }
        public override string ContentType => _inner.ContentType;

        public override string MediaType => _inner.MediaType;

        public override MessageVersion MessageVersion => _inner.MessageVersion;

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            var message = _inner.ReadMessage(buffer, bufferManager, contentType);
            LogMessages.SoapResponse(_logger, ref message);
            return message;
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            var message = _inner.ReadMessage(stream, maxSizeOfHeaders, contentType);
            LogMessages.SoapResponse(_logger, ref message);
            return message;
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            LogMessages.SoapRequest(_logger, ref message);
            return _inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            LogMessages.SoapRequest(_logger, ref message);
            _inner.WriteMessage(message, stream);
        }
    }
}
