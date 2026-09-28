using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Solid.Identity.Protocols.WsTrust.Abstractions;

namespace Solid.Identity.Protocols.WsTrust
{
    /// <summary>Default in-memory, bounded storage for pending WS-Trust exchanges.</summary>
    public class WsTrustExchangeStore : IWsTrustExchangeStore
    {
        private readonly ConcurrentDictionary<string, WsTrustPendingExchange> _pending = new ConcurrentDictionary<string, WsTrustPendingExchange>();
        private readonly object _admissionLock = new object();
        private readonly TimeProvider _clock;

        public WsTrustExchangeStore(TimeProvider clock) => _clock = clock ?? throw new ArgumentNullException(nameof(clock));

        public bool TryAdd(string context, WsTrustPendingExchange exchange)
        {
            lock (_admissionLock)
            {
                foreach (var item in _pending)
                    if (item.Value.Expires <= _clock.GetUtcNow())
                        TryRemove(item.Key, item.Value);
                return _pending.Count < 1024 && _pending.TryAdd(context, exchange);
            }
        }

        public bool TryGet(string context, out WsTrustPendingExchange exchange)
            => _pending.TryGetValue(context, out exchange);

        public bool TryUpdate(string context, WsTrustPendingExchange current, WsTrustPendingExchange next)
            => _pending.TryUpdate(context, next, current);

        public bool TryRemove(string context, WsTrustPendingExchange current)
            => ((ICollection<KeyValuePair<string, WsTrustPendingExchange>>)_pending)
                .Remove(new KeyValuePair<string, WsTrustPendingExchange>(context, current));
    }
}
