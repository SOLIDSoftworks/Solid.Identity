using Solid.Http;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    class SolidHttpTransportBindingElement : TransportBindingElement
    {
        private TestingServer _server;
        private XmlWriterSettings _settings;

        public SolidHttpTransportBindingElement(TestingServer server, XmlWriterSettings settings)
        {
            _server = server;
            _settings = settings;
        }

        public override string Scheme => "http";

        public ISolidHttpClient Client => _server;

        public override BindingElement Clone() => new SolidHttpTransportBindingElement(_server, _settings);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context) => typeof(TChannel) == typeof(IRequestChannel);

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!CanBuildChannelFactory<TChannel>(context))
            {
                throw new ArgumentException(String.Format("Unsupported channel type: {0}.", typeof(TChannel).Name));
            }
            return (IChannelFactory<TChannel>)(object)new SolidHttpRequestChannelFactory(this, context, _settings);
        }

    }
}
