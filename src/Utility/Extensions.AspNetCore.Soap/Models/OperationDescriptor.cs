using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Models
{
    internal class OperationDescriptor
    {
        public OperationDescriptor(string action, string name, string ns, MethodInfo method, OperationContractAttribute contract, ServiceContractAttribute serviceContract)
        {
            Action = action;
            Name = name;
            Namespace = ns;
            Method = method;
            Contract = contract;
            ServiceContract = serviceContract;
        }
        public string Action { get; }
        public string Name { get; }
        public string Namespace { get; }
        public MethodInfo Method { get; }
        public OperationContractAttribute Contract { get; }
        public ServiceContractAttribute ServiceContract { get; }
    }
}
