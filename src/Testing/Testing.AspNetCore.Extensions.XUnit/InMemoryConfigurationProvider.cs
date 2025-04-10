using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    class InMemoryConfigurationProvider : IConfigurationProvider
    {
        private bool _changed;
        private InMemoryConfigurationChangeToken _changeToken = new InMemoryConfigurationChangeToken();
        private readonly IDictionary<string, string> _data = new Dictionary<string, string>();

        public InMemoryConfigurationProvider(IDictionary<string, string> initialData = null)
        {
            if (initialData != null)
                foreach (var pair in initialData)
                    _data.Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            _data.Clear();
            _changed = true;
        }

        public void Update(IDictionary<string, string> values)
        {
            foreach (var pair in values)
            {
                if(!TryGet(pair.Key, out var value) || value != pair.Value)
                    Set(pair.Key, pair.Value);
            }
        }

        public void Set(string key, string value)
        {
            _data[key] = value;
            _changed = true;
        }

        public void Load()
        {
            if (_changed)
            {
                var previousToken = Interlocked.Exchange(ref _changeToken, new InMemoryConfigurationChangeToken());
                previousToken.OnReload();
            }
            _changed = false;
        }

        public bool TryGet(string key, out string value) => _data.TryGetValue(key, out value);

        public IChangeToken GetReloadToken() => _changeToken;

        public virtual IEnumerable<string> GetChildKeys(
            IEnumerable<string> earlierKeys,
            string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            return _data
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(kv => Segment(kv.Key, prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        private string Segment(string key, int prefixLength)
        {
            var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
        }
    }
}
