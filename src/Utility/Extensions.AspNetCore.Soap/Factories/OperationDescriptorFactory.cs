using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Solid.Extensions.AspNetCore.Soap.Factories
{
    internal class OperationDescriptorFactory
    {
        public OperationDescriptor Create(MethodInfo method)
        {
            var operationContract = method.GetCustomAttribute<OperationContractAttribute>();
            var serviceContract = method.DeclaringType.GetCustomAttribute<ServiceContractAttribute>();
            var name = GetOperationName(operationContract, method);
            var ns = method.DeclaringType.GetServiceNamespace();
            var action = GetAction(method, operationContract);
            return new OperationDescriptor(action, name, ns, method, operationContract, serviceContract);
        }

        private string GetAction(MethodInfo method, OperationContractAttribute operationContract)
        {
            if (operationContract.Action != null) return operationContract.Action;

            var serviceName = method.DeclaringType.GetServiceFullName();
            var name = GetOperationName(operationContract, method);
            return $"{serviceName}/{name}";
        }
        
        private string GetOperationName(OperationContractAttribute operation, MethodInfo method)
        {
            if (operation.Name != null) return operation.Name;
            var regex = new Regex("Async$", RegexOptions.IgnoreCase);
            var match = regex.Match(method.Name);
            if (match?.Success == true)
                return method.Name.Remove(match.Index, 5);
            return method.Name;
        }
    }
}
