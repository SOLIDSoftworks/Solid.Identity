using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    internal class MethodParameterReader
    {
        private ConcurrentDictionary<(Type ParameterType, string ParameterName, string ServiceNamespace), DataContractSerializer> _serializers =
            new ConcurrentDictionary<(Type ParameterType, string ParameterName, string ServiceNamespace), DataContractSerializer>();
        public IEnumerable<MethodParameter> ReadParameters(MethodInfo method, Message message, string serviceNamespace)
        {
            using var activity = Tracing.Soap.StartActivity($"{nameof(MethodParameterReader)}.{nameof(ReadParameters)}");
            var parameters = method.GetParameters();
            if (parameters.Length == 0) return Enumerable.Empty<MethodParameter>();
            if (parameters.All(p => p.IsOut)) return parameters.Select(p => new MethodParameter { Name = p.Name, Out = true });
            if (parameters.Length == 1 && parameters[0].ParameterType.IsMessage()) return new[] { new MethodParameter { Name = parameters[0].Name, Value = message } };

            var p = new List<MethodParameter>();
            var reader = message.GetReaderAtBodyContents();
            foreach (var parameter in parameters)
            {
                if(parameter.IsOut)
                {
                    p.Add(new MethodParameter { Name = parameter.Name, Out = true });
                    continue;
                }
                if (reader.ReadToFollowing(parameter.Name))
                {
                    var key = (parameter.ParameterType, parameter.Name, serviceNamespace);
                    var serializer = _serializers.GetOrAdd(key, InitializerSerializer);
                    p.Add(new MethodParameter { Name = parameter.Name, Out = false, Value = serializer.ReadObject(reader, verifyObjectName: true) });
                }
            }

            return p.ToArray();
        }

        private DataContractSerializer InitializerSerializer((Type ParameterType, string ParameterName, string ServiceNamespace) tuple) 
            => new DataContractSerializer(tuple.ParameterType, tuple.ParameterName, tuple.ServiceNamespace);
        
    }
}
