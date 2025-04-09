using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Solid.ServiceModel
{
    class LoggingBehavior : IContractBehavior
    {
        private ILoggerFactory _factory;

        public LoggingBehavior(ILoggerFactory factory)
        {
            _factory = factory;
        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            bindingParameters.Add(_factory);
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new LogScopeMessageInspector(_factory));
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }
    }
}
