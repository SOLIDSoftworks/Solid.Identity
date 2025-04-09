using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.ServiceModel
{
    class TraceMessageEncoderFactory : MessageEncoderFactory
    {
        public TraceMessageEncoderFactory(MessageEncoderFactory inner, ILoggerFactory loggerFactory)
        {
            Encoder = new TraceMessageEncoder(inner.Encoder, loggerFactory);
            MessageVersion = inner.Encoder.MessageVersion;
        }

        public override MessageEncoder Encoder { get; }

        public override MessageVersion MessageVersion { get; }
    }
}
