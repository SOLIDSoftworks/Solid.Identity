using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Testing.AspNetCore.Extensions.XUnit
{
    class InMemoryConfigurationChangeToken : IChangeToken
    {
        private ConcurrentDictionary<Guid, InMemoryConfigurationChangeCallback> Callbacks = new ConcurrentDictionary<Guid, InMemoryConfigurationChangeCallback>();
        public bool HasChanged { get; private set; }

        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            var c = new InMemoryConfigurationChangeCallback(callback, state, this);
            Callbacks.AddOrUpdate(c.Id, c, (_, __) => c);
            return c;
        }

        /// <summary>
        /// Used to trigger the change token when a reload occurs.
        /// </summary>
        public void OnReload()
        {
            HasChanged = true;
            foreach (var callback in Callbacks.Values)
                callback.Action(callback.State);
        }

        class InMemoryConfigurationChangeCallback : IDisposable
        {
            public InMemoryConfigurationChangeCallback(Action<object> action, object state, InMemoryConfigurationChangeToken changeToken)
            {
                Action = action;
                State = state;
                ChangeToken = changeToken;
            }

            public Guid Id { get; } = Guid.NewGuid();
            public Action<object> Action { get; }
            public object State { get; }
            public InMemoryConfigurationChangeToken ChangeToken { get; }

            public void Dispose() => ChangeToken.Callbacks.TryRemove(Id, out _);
        }
    }
}
