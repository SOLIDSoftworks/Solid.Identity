using Microsoft.Extensions.Logging;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Extensions.AspNetCore.Soap
{
    internal class MethodInvoker
    {
        private ILogger<MethodInvoker> _logger;

        public MethodInvoker(ILogger<MethodInvoker> logger)
        {
            _logger = logger;
        }

        public ValueTask<MethodInvocationResult> InvokeMethodAsync(object instance, MethodInfo method, IEnumerable<object> args, CancellationToken cancellationToken = default)
            => InvokeMethodAsync(instance, method, args.Select(a => new MethodParameter { Value = a }), cancellationToken);

        public async ValueTask<MethodInvocationResult> InvokeMethodAsync(object instance, MethodInfo method, IEnumerable<MethodParameter> args, CancellationToken cancellationToken = default)
        {            
            using var activity = Tracing.Soap.StartActivity($"{nameof(MethodInvoker)}.{nameof(InvokeMethodAsync)}");
            var isVoid = IsVoid(method);
            var isTask = IsTask(method);
            //var isValueTask = IsValueTask(method);
            var result = null as object;

            _logger.LogInformation($"Invoking {method.Name} on {method.DeclaringType.Name}");

            var array = args.Select(p => p.Value).ToArray();
            var returned = method.Invoke(instance, array);

            if (isTask)
                await (returned as Task);

            if (!isVoid)
            {
                if (isTask)
                {
                    var d = returned as dynamic;
                    result = d.Result as object;
                }
                else
                {
                    result = returned;
                }
            }
            var outParameters = new Dictionary<string, object>();
            for(var i = 0; i < args.Count(); i++)
            {
                var arg = args.ElementAt(i);
                if (!arg.Out) continue;
                outParameters.Add(arg.Name, array[i]);
            }

            return new MethodInvocationResult(isVoid, isTask, result, outParameters);
        }

        private bool IsVoid(MethodInfo method)
        {
            return method.ReturnType.FullName == "System.Void" || method.ReturnType == typeof(Task);
        }

        private bool IsTask(MethodInfo method)
        {
            return method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
        }

        //private bool IsValueTask(MethodInfo method)
        //{
        //    return method.ReturnType == typeof(ValueTask) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>));
        //}
    }
}
