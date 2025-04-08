using Solid.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    class SolidHttpRequestChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        private MessageEncoderFactory _encoderFactory;
        private SolidHttpTransportBindingElement _transportElement;
        private XmlWriterSettings _settings;

        public SolidHttpRequestChannelFactory(SolidHttpTransportBindingElement element, BindingContext context, XmlWriterSettings settings)
            : base(context.Binding)
        {
            var messageElement = context.BindingParameters.OfType<MessageEncodingBindingElement>().Single();
            context.BindingParameters.Remove(messageElement);
            _encoderFactory = messageElement.CreateMessageEncoderFactory();
            _transportElement = element;
            _settings = settings;
        }

        private Task OnOpenAsync(TimeSpan timeout)
        {
            OnOpen(timeout);
            return Task.CompletedTask;
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            => OnOpenAsync(timeout).ConvertToAsyncResult(callback, state);

        protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            return new SolidHttpRequestChannel(address, via, _transportElement.Client, _encoderFactory.Encoder, _encoderFactory.MessageVersion, _settings);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            if (!(result is Task task)) return;

            if (task.IsFaulted) throw task.Exception;
            if (task.IsCanceled) throw new TaskCanceledException();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }
    }
}
