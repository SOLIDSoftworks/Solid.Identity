using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.ServiceModel
{
    class TraceMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private MessageEncodingBindingElement _inner;
        private ILoggerFactory _loggerFactory;

        public TraceMessageEncodingBindingElement(MessageEncodingBindingElement inner, ILoggerFactory loggerFactory)
            : base(inner)
        {
            _inner = inner;
            _loggerFactory = loggerFactory;
        }
        public override MessageVersion MessageVersion { get => _inner.MessageVersion; set => _inner.MessageVersion = value; }

        public override BindingElement Clone() => new TraceMessageEncodingBindingElement(_inner.Clone() as MessageEncodingBindingElement, _loggerFactory);

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            var factory = _inner.CreateMessageEncoderFactory();
            return new TraceMessageEncoderFactory(factory, _loggerFactory);
        }

        public override T GetProperty<T>(BindingContext context) => _inner.GetProperty<T>(context);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) => _inner.CanBuildChannelFactory<TChannel>(context);

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            var factory = context.BuildInnerChannelFactory<TChannel>();
            return factory;
        }
    }
}
