using Solid.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Solid.Http.Soap
{
    class SolidHttpTransportBindingElement : TransportBindingElement
    {
        private readonly ISolidHttpClient _client;
        private readonly XmlWriterSettings _settings;

        public SolidHttpTransportBindingElement(ISolidHttpClient server, XmlWriterSettings settings)
        {
            _client = server;
            _settings = settings;
        }

        public override string Scheme => "http";

        public ISolidHttpClient Client => _client;

        public override BindingElement Clone() => new SolidHttpTransportBindingElement(_client, _settings);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) => typeof(TChannel) == typeof(IRequestChannel);

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (!CanBuildChannelFactory<TChannel>(context))
            {
                throw new ArgumentException($"Unsupported channel type: {typeof(TChannel).Name}.");
            }
            return (IChannelFactory<TChannel>)(object)new SolidHttpRequestChannelFactory(this, context, _settings);
        }

    }
}
