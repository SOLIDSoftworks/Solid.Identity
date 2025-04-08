using Solid.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    class SolidHttpRequestChannel : AsyncRequestChannel
    {
        private ISolidHttpClient _client;
        private MessageEncoder _encoder;
        private MessageVersion _messageVersion;
        private XmlWriterSettings _settings;

        public SolidHttpRequestChannel(EndpointAddress address, Uri via, ISolidHttpClient client, MessageEncoder encoder, MessageVersion messageVersion, XmlWriterSettings settings)
        {
            RemoteAddress = address;
            Via = via;
            _client = client;
            _encoder = encoder;
            _messageVersion = messageVersion;
            _settings = settings;
        }

        public override EndpointAddress RemoteAddress { get; }
        public override Uri Via { get; }

        protected override async Task<Message> RequestAsync(Message request, TimeSpan timeout)
        {
            request.Headers.To = RemoteAddress.Uri;

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, _settings))
                    request.WriteMessage(writer);

                stream.Position = 0;
                var content = new StreamContent(stream);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(_encoder.ContentType);

                var response = await _client
                    .PostAsync(Via.AbsolutePath)
                    .WithContent(content)
                ;

                using(var responseContent = await response.Content.ReadAsStreamAsync())
                using(var reader = XmlReader.Create(responseContent))
                {
                    var message = Message.CreateMessage(reader, int.MaxValue, _messageVersion);
                    var buffered = message.CreateBufferedCopy(int.MaxValue).CreateMessage();
                    return buffered;
                }
            }
        }
    }
}
