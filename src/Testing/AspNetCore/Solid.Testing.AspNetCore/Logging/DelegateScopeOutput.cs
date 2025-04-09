using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.AspNetCore.Logging
{
    class DelegateScopeOutput<TState> : IDisposable
    {
        private TState _state;
        private Action<string, TState> _output;

        public DelegateScopeOutput(TState state, Action<string, TState> output)
        {
            output("scope_start", state);
            _state = state;
            _output = output;
        }

        public void Dispose()
        {
            _output("scope_end", _state);
        }
    }
}
