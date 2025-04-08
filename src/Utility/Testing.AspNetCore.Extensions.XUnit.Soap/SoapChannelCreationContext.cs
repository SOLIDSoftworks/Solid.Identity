using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.Text;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Solid.Testing.AspNetCore.Extensions.XUnit.Soap
{
    internal class SoapChannelCreationContext<TChannel> : SoapChannelCreationContext
    {
        public SoapChannelCreationContext(string path, MessageVersion messageVersion, bool reusable, IReadOnlyDictionary<string, object> properties) 
            : base(path, messageVersion, reusable, properties)
        {
        }

        protected override string GenerateId(string path, MessageVersion messageVersion, bool reusable, IReadOnlyDictionary<string, object> properties)
        {
            var parts = new List<string>
            {
                typeof(TChannel).FullName,
                messageVersion.ToString(),
                path ?? "/"
            };

            foreach (var pair in properties)
                parts.Add($"{pair.Key}:{pair.Value}");

            if (!reusable)
                parts.Add(Guid.NewGuid().ToString());

            return string.Join("__", parts);
        }
    }

    public abstract class SoapChannelCreationContext
    {
        protected SoapChannelCreationContext(string path, MessageVersion messageVersion, bool reusable, IReadOnlyDictionary<string, object> properties)
        {
            Path = path;
            MessageVersion = messageVersion;
            Reusable = reusable;
            Properties = properties;

            Id = GenerateId(path, messageVersion, reusable, properties);
        }

        public bool Reusable { get; }
        public string Path { get; }
        public string Id { get; } 
        public MessageVersion MessageVersion { get;  }
        public IReadOnlyDictionary<string, object> Properties { get; }

        public static SoapChannelCreationContext Create<TChannel>(string path = "", MessageVersion messageVersion = null, bool reusable = true, IDictionary<string, object> properties = null)
        {
            if (messageVersion == null)
                messageVersion = MessageVersion.Default;
            return new SoapChannelCreationContext<TChannel>(path, messageVersion, reusable, new ReadOnlyDictionary<string, object>(properties ?? new Dictionary<string, object>()));
        }

        protected abstract string GenerateId(string path, MessageVersion messageVersion, bool reusable, IReadOnlyDictionary<string, object> properties);
    }
}
