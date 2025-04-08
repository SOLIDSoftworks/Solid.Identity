using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Models
{
    internal class MethodInvocationResult
    {
        public MethodInvocationResult(bool isVoid, bool isAsync, object result, IDictionary<string, object> outParameters = null)
        {
            IsVoid = isVoid;
            IsAsync = isAsync;
            Result = result;
            OutParameters = new ReadOnlyDictionary<string, object>(outParameters ?? new Dictionary<string, object>());
        }
        public bool IsAsync { get; }
        public bool IsVoid { get; }
        public object Result { get; }
        public IReadOnlyDictionary<string, object> OutParameters { get; }
    }
}
