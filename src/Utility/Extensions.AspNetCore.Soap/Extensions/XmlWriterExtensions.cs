using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xml
{
    internal static class XmlWriterExtensions
    {
        public static void WriteElement(this XmlWriter writer, XmlReader reader)
        {
            using (var inner = reader.ReadSubtree())
            {
                while (inner.Read())
                {
                    if (inner.NodeType == XmlNodeType.Element)
                    {
                        if (string.IsNullOrEmpty(inner.NamespaceURI))
                            writer.WriteStartElement(inner.LocalName);
                        else
                            writer.WriteStartElement(inner.LocalName, inner.NamespaceURI);

                        writer.WriteAttributes(inner, false);
                        continue;
                    }

                    if (inner.NodeType == XmlNodeType.Text)
                    {
                        writer.WriteValue(inner.Value);
                        continue;
                    }

                    if (inner.NodeType == XmlNodeType.EndElement)
                    {
                        writer.WriteEndElement();
                        continue;
                    }
                }
            }
        }
    }
}
