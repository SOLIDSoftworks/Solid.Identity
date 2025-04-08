using Solid.Extensions.AspNetCore.Soap.Factories;
using Solid.Extensions.AspNetCore.Soap.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Solid.Extensions.AspNetCore.Soap
{
    internal class MethodLocator
    {
        private ConcurrentDictionary<Type, IReadOnlyDictionary<string, OperationDescriptor>> _methods
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<string, OperationDescriptor>>();
        private OperationDescriptorFactory _factory;

        public MethodLocator(OperationDescriptorFactory factory)
        {
            _factory = factory;
        }

        public MethodInfo GetMethod(Type contract, string action)
        {
            using var activity = Tracing.Soap.StartActivity($"{nameof(MethodLocator)}.{nameof(GetMethod)}");
            return FindOperation(contract, action).Method;
        }

        public string GetOperationName(Type contract, string action)
        {
            using var activity = Tracing.Soap.StartActivity($"{nameof(MethodLocator)}.{nameof(GetOperationName)}");
            return FindOperation(contract, action).Name;
        }

        public string GetNamespace(Type contract, string action)
        {
            using var activity = Tracing.Soap.StartActivity($"{nameof(MethodLocator)}.{nameof(GetNamespace)}");
            return FindOperation(contract, action).Namespace;
        }

        private OperationDescriptor FindOperation(Type contract, string action)
        {
            var dictionary = _methods.GetOrAdd(contract, type => InitializeContract(type));
            // TODO: Change to ActionNotFoundException
            if (!dictionary.TryGetValue(action, out var descriptor)) throw new ArgumentException($"Action '{action}' not found on {contract.FullName}.", nameof(action));
            return descriptor;
        }

        private IReadOnlyDictionary<string, OperationDescriptor> InitializeContract(Type contract)
        {
            var descriptors = contract
                .GetMethods()
                .Select(method => _factory.Create(method))
                .Where(t => t.Contract != null)
                .ToDictionary(t => t.Action, t => t)
            ;
            return new ReadOnlyDictionary<string, OperationDescriptor>(descriptors);
        }
    }
}
