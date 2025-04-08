using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace System
{
    internal static class TypeExtensions
    {
        public static ServiceLifetime? GetSoapServiceLifetime(this Type type)
        {
            return ServiceLifetime.Singleton;
        }
        public static bool IsMessage(this Type type) => typeof(Message).IsAssignableFrom(type);

        public static string GetServiceName(this Type type)
            => type.GetCustomAttribute<ServiceContractAttribute>()?.Name ?? type.Name;

        public static string GetServiceNamespace(this Type type)
            => type.GetCustomAttribute<ServiceContractAttribute>()?.Namespace ?? "http://tempuri.org/";
        public static string GetServiceFullName(this Type type)
        {
            var ns = type.GetServiceNamespace();
            var name = type.GetServiceName();
            if (!ns.EndsWith("/")) ns = ns + "/";
            return ns + name;
        }
    }
}
