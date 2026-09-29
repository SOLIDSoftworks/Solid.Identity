using System;
using System.Collections.Generic;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Bounded process-local issued-token store with expiration on access and admission.</summary>
    public class InMemoryIssuedTokenStore : IIssuedTokenStore
    {
        private const int MaxEntries = 1024;
        private readonly Dictionary<string, IssuedTokenRegistry.Entry> _tokens = new Dictionary<string, IssuedTokenRegistry.Entry>();
        private readonly object _gate = new object();
        private readonly TimeProvider _clock;

        public InMemoryIssuedTokenStore(TimeProvider clock) => _clock = clock;

        public void Register(string key, IssuedTokenRegistry.Entry entry)
        {
            lock (_gate)
            {
                Prune();
                if (entry.Expires <= _clock.GetUtcNow().UtcDateTime)
                    throw new InvalidRequestException("Issued token has already expired.");
                if (_tokens.Count >= MaxEntries && !_tokens.ContainsKey(key))
                    throw new InvalidRequestException("Issued-token registry capacity exceeded.");
                _tokens[key] = entry;
            }
        }

        public bool TryGet(string key, out IssuedTokenRegistry.Entry entry)
        {
            lock (_gate)
            {
                Prune();
                return _tokens.TryGetValue(key, out entry);
            }
        }

        public bool TryUpdate(string key, IssuedTokenRegistry.Entry expected, IssuedTokenRegistry.Entry updated)
        {
            lock (_gate)
            {
                Prune();
                if (!_tokens.TryGetValue(key, out var current) || current.Revision != expected.Revision)
                    return false;
                _tokens[key] = updated;
                return true;
            }
        }

        public bool TryRemove(string key, IssuedTokenRegistry.Entry expected)
        {
            lock (_gate)
            {
                Prune();
                return _tokens.TryGetValue(key, out var current) && current.Revision == expected.Revision && _tokens.Remove(key);
            }
        }

        private void Prune()
        {
            var now = _clock.GetUtcNow().UtcDateTime;
            var expired = new List<string>();
            foreach (var pair in _tokens)
                if (pair.Value.Expires <= now) expired.Add(pair.Key);
            foreach (var key in expired) _tokens.Remove(key);
        }
    }
}
