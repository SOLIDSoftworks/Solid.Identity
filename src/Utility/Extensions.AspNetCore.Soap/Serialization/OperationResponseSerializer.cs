using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Solid.Extensions.AspNetCore.Soap.Serialization
{
    internal class OperationResponseSerializer : XmlObjectSerializer
    {
        private IReadOnlyDictionary<string, object> _outParameters;
        private string _namespace;
        private string _name;

        public static readonly string XmlSchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

        public OperationResponseSerializer(IReadOnlyDictionary<string, object> outParameters, string serviceNamespace, string operationName)
        {
            _outParameters = outParameters;
            _namespace = serviceNamespace;
            _name = operationName;
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
            => throw new NotImplementedException();

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
            => throw new NotImplementedException();

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            writer.WriteEndElement();
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            writer.WriteStartElement(string.Empty, $"{_name}Result", _namespace);
            WriteObjectContentOrNil(writer, graph);
            writer.WriteEndElement();
            WriteOutParameters(writer);
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            writer.WriteStartElement(string.Empty, $"{_name}Response", _namespace);
        }

        private void WriteOutParameters(XmlDictionaryWriter writer)
        {
            foreach(var parameter in _outParameters)
            {
                writer.WriteStartElement(string.Empty, parameter.Key, _namespace);
                WriteObjectContentOrNil(writer, parameter.Value);
                writer.WriteEndElement();
            }
        }

        private void WriteObjectContentOrNil(XmlDictionaryWriter writer, object graph)
        {
            if (graph == null)
            {
                writer.WriteAttributeString("a", "nil", XmlSchemaInstance, "true");
            }
            else
            {
                var serializer = new DataContractSerializer(graph.GetType());
                serializer.WriteObjectContent(writer, graph);
            }
        }
    }
}
