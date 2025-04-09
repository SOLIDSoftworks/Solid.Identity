using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Solid.ServiceModel
{
    class LogScopeMessageInspector : IClientMessageInspector
    {
        private ILogger<LogScopeMessageInspector> _logger;
        private ConcurrentDictionary<object, IDisposable> _scopes = new ConcurrentDictionary<object, IDisposable>();

        public LogScopeMessageInspector(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<LogScopeMessageInspector>();
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (_scopes.TryRemove(correlationState, out var scope))
                scope?.Dispose();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var scope = LogMessages.BeginSoapRequestScope(_logger, request);
            var guid = Guid.NewGuid();
            _scopes.TryAdd(guid, scope);
            return guid;
        }
    }
}
