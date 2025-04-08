using Solid.Testing.AspNetCore.Extensions.XUnit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    public class SoapTestingServerFixture<TStartup> : TestingServerFixture<TStartup>
    {
        private ConcurrentDictionary<string, ICommunicationObject> _channels = new ConcurrentDictionary<string, ICommunicationObject>();

        public virtual TChannel CreateChannel<TChannel>(MessageVersion version = null, string path = null, bool reusable = true)
            where TChannel : class
        {
            var context = SoapChannelCreationContext.Create<TChannel>(path, version, reusable);
            return CreateChannel<TChannel>(context);
        }

        protected virtual Binding CreateBinding<TContract>(SoapChannelCreationContext context)
        {
            var binding = new CustomBinding(new BasicHttpBinding())
            {
            };
            var encoding = binding.Elements.Find<TextMessageEncodingBindingElement>();
            encoding.MessageVersion = context.MessageVersion;
            return binding;
        }

        protected virtual EndpointAddress CreateEndpointAddress<TChannel>(Uri url, SoapChannelCreationContext context)
            => new EndpointAddress(url);

        protected virtual ChannelFactory<TChannel> CreateChannelFactory<TChannel>(Binding binding, EndpointAddress endpointAddress, SoapChannelCreationContext context)
            => new ChannelFactory<TChannel>(binding, endpointAddress);
        
        protected virtual ICommunicationObject CreateChannel<TChannel>(ChannelFactory<TChannel> factory, SoapChannelCreationContext context)
            => factory.CreateChannel() as ICommunicationObject;

        protected virtual TChannel CreateChannel<TChannel>(SoapChannelCreationContext context)
            where TChannel : class
        {
            return _channels.GetOrAdd(context.Id, k =>
            {
                var url = TestingServer.BaseAddress;
                if (context.Path != null)
                    url = new Uri(url, context.Path);
                
                var binding = CreateBinding<TChannel>(context).WithSolidHttpTransport(TestingServer);
                var endpointAddress = CreateEndpointAddress<TChannel>(url, context);
                var factory = CreateChannelFactory<TChannel>(binding, endpointAddress, context);
                var channel = CreateChannel(factory, context);
                channel.Faulted += (sender, args) => _channels.TryRemove(k, out _);
                channel.Closing += (sender, args) => _channels.TryRemove(k, out _);
                return channel;
            }) as TChannel;
        }

        protected virtual string GenerateKey<TChannel>(MessageVersion version, string path, bool unique)
        {
            if (unique) return $"{typeof(TChannel).FullName}__{version}__{path ?? "/"}__{Guid.NewGuid()}";
            return $"{typeof(TChannel).FullName}__{version}__{path ?? "/"}";
        }

        protected override void Disposing()
        {
            var channels = _channels.Values.OfType<ICommunicationObject>().ToArray();
            foreach (var channel in channels)
                channel.Close();
        }
    }
}
