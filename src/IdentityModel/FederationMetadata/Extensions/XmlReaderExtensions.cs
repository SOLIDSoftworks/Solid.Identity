using Solid.IdentityModel.FederationMetadata;
using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xml
{
    internal static class XmlReaderExtensions
    {
        public static bool TryReadFederationEndpointType(this XmlDictionaryReader reader, out FederationEndpointType type)
        {
            if (!reader.TryReadAttribute(XsiConstants.Attributes.Type, XsiConstants.Namespace, out var value)) return Out.False(out type);
            if (value.IndexOf(':') < 0) return Out.False(out type);
            var split = value.Split(':');
            if (split.Length != 2) return Out.False(out type);
            var prefix = split[0];
            var ns = reader.LookupNamespace(prefix);
            if (string.IsNullOrEmpty(ns)) return Out.False(out type);
            if (ns != WsFederationConstants.Namespace) return Out.False(out type);

            var typeName = split[1];
            if (typeName.EndsWith("Type"))
                typeName = typeName.Remove(typeName.Length - 4);

            return Enum.TryParse(typeName, out type);
        }
    }
}
