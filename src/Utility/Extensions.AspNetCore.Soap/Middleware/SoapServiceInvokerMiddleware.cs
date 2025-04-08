using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Solid.Extensions.AspNetCore.Soap.Models;
using Solid.Extensions.AspNetCore.Soap.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap.Middleware
{
    internal class SoapServiceInvokerMiddleware : SoapMiddleware
    {
        private MethodLocator _locator;
        private MethodParameterReader _reader;
        private MethodInvoker _invoker;

        public SoapServiceInvokerMiddleware(
            MethodLocator locator,
            MethodParameterReader reader,
            MethodInvoker invoker,
            RequestDelegate next, 
            ILogger<SoapServiceInvokerMiddleware> logger
        ) 
            : base(next, logger)
        {
            _locator = locator;
            _reader = reader;
            _invoker = invoker;
        }

        protected override async ValueTask InvokeAsync(SoapContext context)
        {
            using var activity = Tracing.Soap.StartActivity($"{nameof(SoapServiceInvokerMiddleware)}.{nameof(InvokeAsync)}");
            var action = context.Request.Headers.Action;
            var instance = context.RequestServices.GetService(context.Contract);

            var methodInfo = _locator.GetMethod(context.Contract, action);
            var name = _locator.GetOperationName(context.Contract, action);
            var ns = _locator.GetNamespace(context.Contract, action);

            var parameters = _reader.ReadParameters(methodInfo, context.Request, ns);
            var result = await _invoker.InvokeMethodAsync(instance, methodInfo, parameters, context.CancellationToken);
            var response = result.Result as Message;
            if(response == null)
            {
                var serializer = new OperationResponseSerializer(result.OutParameters, ns, name);
                response = Message.CreateMessage(context.MessageVersion, null, result.Result, serializer);
            }
            context.Response = response;
        }
    }
}
