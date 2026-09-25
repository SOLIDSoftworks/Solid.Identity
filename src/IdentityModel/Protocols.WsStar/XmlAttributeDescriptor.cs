using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.IdentityModel.Xml
{
    public struct XmlAttributeDescriptor
    {
        public static XmlAttributeDescriptor[] EmptyArray = Array.Empty<XmlAttributeDescriptor>();

        public XmlAttributeDescriptor(string prefix, string localName, string ns, string value)
        {
            Prefix = prefix;
            LocalName = localName;
            NamespaceUri = ns;
            Value = value;
        }

        public string Prefix { get; }

        public string NamespaceUri { get; }

        public string LocalName { get; }

        public string Value { get; }

        public static XmlAttributeDescriptor[] ReadAttributes(XmlDictionaryReader reader)
        {
            if (reader.AttributeCount == 0)
                return EmptyArray;

            var count = reader.AttributeCount;
            var attributes = new List<XmlAttributeDescriptor>(count);
            reader.MoveToFirstAttribute();
            for (int i = 0; i < count; i++)
            {
                var prefix = reader.Prefix;
                if (prefix == "xmlns")
                {
                    reader.MoveToNextAttribute();
                    continue;
                }

                var ns = reader.NamespaceURI;
                var localName = reader.LocalName;
                var value = string.Empty;
                while (reader.ReadAttributeValue())
                {
                    if (value.Length == 0)
                        value = reader.Value;
                    else
                        value += reader.Value;
                }

                attributes.Add(new XmlAttributeDescriptor(prefix, localName, ns, value));
                reader.MoveToNextAttribute();
            }

            reader.MoveToElement();
            return attributes.ToArray();
        }

        public static string? GetAttribute(IEnumerable<XmlAttributeDescriptor> attributes, string localName, string ns)
        {
            foreach (var attribute in attributes)
            {
                // if a prefix exist, then the namespace comes into play
                if (!string.IsNullOrEmpty(attribute.Prefix))
                {
                    if (attribute.LocalName == localName && attribute.NamespaceUri == ns)
                        return attribute.Value;
                }
                else
                {
                    if (attribute.LocalName == localName)
                        return attribute.Value;
                }
            }

            return null;
        }
    }
}
