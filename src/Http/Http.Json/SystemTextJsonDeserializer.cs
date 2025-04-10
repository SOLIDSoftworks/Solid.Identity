using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Solid.Http.Json
{
    internal class SystemTextJsonDeserializer : IDeserializer, IDisposable
    {
        private SolidHttpJsonOptions _options;
        private readonly IDisposable _optionsChangeToken;

        public SystemTextJsonDeserializer(IOptionsMonitor<SolidHttpJsonOptions> monitor)
        {
            _options = monitor.CurrentValue;
            _optionsChangeToken = monitor.OnChange(o => _options = o);
        }

        public async ValueTask<T> DeserializeAsync<T>(HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, _options.SerializerOptions);
            }
        }

        /// <summary>
        /// Checks whether this deserializer can deserialize the <paramref name="mediaType" /> into a the <paramref name="typeToReturn" />.
        /// </summary>
        /// <param name="mediaType">A mime type.</param>
        /// <param name="typeToReturn">A <see cref="Type" /> to deserialize to.</param>
        /// <returns>true or false</returns>
        public bool CanDeserialize(string mediaType, Type typeToReturn) 
            => _options.SupportedMediaTypes.Any(m => m.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase));


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
            => _optionsChangeToken?.Dispose();
    }
}
