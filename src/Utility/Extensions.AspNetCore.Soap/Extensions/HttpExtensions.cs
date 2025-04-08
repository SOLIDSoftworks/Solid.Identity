using Solid.Extensions.AspNetCore.Soap;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    internal static class HttpExtensions
    {
        private static readonly string _contextKey = typeof(SoapContext).FullName;

        public static void SetSoapContext(this HttpContext http, SoapContext context)
            => http.Items[_contextKey] = context;

        public static SoapContext GetSoapContext(this HttpContext http)
        {
            if (!http.Items.TryGetValue(_contextKey, out var context)) return null;
            return context as SoapContext;
        }

        public static async Task EnableRewindAsync(this HttpRequest request)
        {
            var stream = new MemoryStream();
            await request.Body.CopyToAsync(stream);
            stream.Position = 0;
            request.Body = stream;
            request.HttpContext.Response.RegisterForDispose(stream);
        }

        public static IAsyncDisposable Buffer(this HttpResponse response) => new HttpResponseBodyBuffer(response);

        class HttpResponseBodyBuffer : IAsyncDisposable
        {
            private Stream _body;
            private HttpResponse _response;

            public HttpResponseBodyBuffer(HttpResponse response)
            {
                _response = response;
                _body = _response.Body;
                _response.Body = new MemoryStream();
                
            }

            public async ValueTask DisposeAsync()
            {
                _response.Body.Position = 0;
                await _response.Body.CopyToAsync(_body);
                _response.Body = _body;
            }
        }
    }
}
