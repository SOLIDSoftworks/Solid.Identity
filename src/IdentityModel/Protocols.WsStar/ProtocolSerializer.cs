using System;
using System.Linq;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Xml;
using XmlException = System.Xml.XmlException;

namespace Solid.IdentityModel.Protocols;

public abstract class ProtocolSerializer
{
    protected abstract WsProtocolConstants GetProtocolConstants(WsSerializationContext context);
    protected abstract WsSerializationContext CreateContext(string ns);
    protected abstract string[] SupportedEntities { get; }
    
    protected void AssertReader(XmlReader reader, string element, WsSerializationContext context)
    {
        if (context == null)
            throw LogHelper.LogArgumentNullException(nameof(context));
        
        var constants = GetProtocolConstants(context);
        AssertReader(reader, element, constants.Namespace);
    }
    
    protected void AssertReader(XmlReader reader, string element, string ns)
    {
        if (reader == null)
            throw LogHelper.LogArgumentNullException(nameof(reader));

        // IsStartElement calls reader.MoveToContent().
        if (!reader.IsStartElement())
            throw XmlUtil.LogReadException(LogMessages.IDX15022, reader.NodeType);

        if (!reader.IsStartElement(element, ns))
            throw XmlUtil.LogReadException(LogMessages.IDX15011, ns, element, reader.NamespaceURI, reader.LocalName);
    }
    
    protected void ReadAdditionalXmlElement(XmlDictionaryReader reader, XmlOpenItem entity)
    {
        var doc = new XmlDocument();
        doc.LoadXml(reader.ReadOuterXml());
        entity.AdditionalXmlElements.Add(doc.DocumentElement!);
    }

    protected void ReadNode<TEntity, TEntityReadState>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, TEntity entity, ReadChildNode<TEntity, TEntityReadState> readChildren)
        where TEntity : class
        where TEntityReadState: class, new()
    {
        var state = new TEntityReadState();
        ReadNode(reader, serializer, context, entity, state, readChildren);
    }

    protected void ReadNode<TEntity, TEntityReadState>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, TEntity entity, TEntityReadState state, ReadChildNode<TEntity, TEntityReadState> readChildren)
        where TEntity : class
        where TEntityReadState: class
    {
        var empty = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (empty)
            return;

        reader.MoveToContent();
        while (reader.NodeType == XmlNodeType.Element)
        {
            readChildren(reader, serializer, context, entity, state);
            reader.MoveToContent();
        }
        reader.ReadEndElement();
    }

    protected void ReadNode<TEntity>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, TEntity entity, ReadChildNode<TEntity> readChildren)
        where TEntity : class
        => ReadNode<TEntity, object>(reader, serializer, context, entity, (r, s, c, e, _) => readChildren(r, s, c, e));

    protected void ReadNode<T>(XmlDictionaryReader reader, T entity, ReadNodeValue<T> readValue)
        where T : class
    {
        var empty = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (empty)
            return;

        reader.MoveToContent();
        readValue(reader, entity);
        reader.ReadEndElement();
    }

    protected void WriteXmlOpenItemAttributes(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlOpenItem item)
    {
        foreach (var attribute in item.AdditionalXmlAttributes)
        {
            WriteXmlAttribute(writer, serializationContext, attribute);
        }
    }
    
    protected void WriteXmlOpenItemElements(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlOpenItem item)
    {
        foreach (var element in item.AdditionalXmlElements)
        {
            WriteXmlElement(writer, serializationContext, element);
        }
    }
    
    protected void WriteXmlElement(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlElement xmlElement)
    {
        WsUtils.ValidateParamsForWriting(writer, serializationContext, xmlElement, nameof(xmlElement));
        try
        {
            var prefix = NormalizePrefix(writer, serializationContext, xmlElement);
            if (!string.IsNullOrEmpty(prefix))
                writer.WriteStartElement(prefix, xmlElement.LocalName, xmlElement.NamespaceURI);
            else 
                writer.WriteStartElement(xmlElement.LocalName, xmlElement.NamespaceURI);

            foreach (var attribute in xmlElement.Attributes.Cast<XmlAttribute>())
                WriteXmlAttribute(writer, serializationContext, attribute);

            foreach (XmlNode child in xmlElement.ChildNodes)
                child.WriteTo(writer);

            writer.WriteEndElement();
        }
        catch (Exception ex)
        {
            if (ex is XmlWriteException)
                throw;

            throw XmlUtil.LogWriteException(LogMessages.IDX15407, ex, xmlElement.LocalName, ex);
        }
    }
    
    protected void WriteXmlAttribute(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlAttributeDescriptor attribute)
    {
        WsUtils.ValidateParamsForWriting(writer, serializationContext, attribute, nameof(attribute));
        try
        {
            var prefix = NormalizePrefix(writer, serializationContext, attribute);
            if (!string.IsNullOrEmpty(prefix))
                writer.WriteAttributeString(prefix, attribute.LocalName, attribute.NamespaceUri, attribute.Value);
            else 
                writer.WriteAttributeString(attribute.LocalName, attribute.NamespaceUri, attribute.Value);
        }
        catch (Exception ex)
        {
            if (ex is XmlWriteException)
                throw;

            throw XmlUtil.LogWriteException(LogMessages.IDX15407, ex, attribute.LocalName, ex);
        }
    }


    protected WsSerializationContext CreateContext(XmlDictionaryReader reader)
    {
        if (reader == null)
            throw LogHelper.LogArgumentNullException(nameof(reader));

        reader.MoveToContent();
        var name = reader.LocalName;
        if (!SupportedEntities.Contains(name))
            throw new XmlException("Cannot create WS serialization context for " + name);

        var ns = reader.NamespaceURI;
        return CreateContext(ns);
    }


    private void WriteXmlAttribute(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlAttribute attribute)
    {
        WsUtils.ValidateParamsForWriting(writer, serializationContext, attribute, nameof(attribute));

        try
        {
            var prefix = NormalizePrefix(writer, serializationContext, attribute);
            if (!string.IsNullOrEmpty(prefix))
                writer.WriteAttributeString(prefix, attribute.LocalName, attribute.NamespaceURI, attribute.Value);
            else 
                writer.WriteAttributeString(attribute.LocalName, attribute.NamespaceURI, attribute.Value);
        }
        catch (Exception ex)
        {
            if (ex is XmlWriteException)
                throw;

            throw XmlUtil.LogWriteException(LogMessages.IDX15407, ex, attribute.LocalName, ex);
        }
    }

    private string NormalizePrefix(XmlDictionaryWriter writer, WsSerializationContext context, string prefix, string namespaceUri)
    {
        if (string.IsNullOrEmpty(prefix))
            return null;
        
        var p = writer.LookupPrefix(namespaceUri);
        if (p != null)
            return p;
        
        var constants = GetProtocolConstants(context);
        if (namespaceUri == constants.Namespace)
            return GetProtocolConstants(context).DefaultPrefix;

        return prefix;
    }

    private string NormalizePrefix(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlElement element)
        => NormalizePrefix(writer, serializationContext, element.Prefix, element.NamespaceURI);

    private string NormalizePrefix(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlAttributeDescriptor attribute)
        =>  NormalizePrefix(writer, serializationContext, attribute.Prefix, attribute.NamespaceUri);
    private string NormalizePrefix(XmlDictionaryWriter writer, WsSerializationContext serializationContext, XmlAttribute attribute)
        =>  NormalizePrefix(writer, serializationContext, attribute.Prefix, attribute.NamespaceURI);
}

public delegate void ReadChildNode<TEntity, TEntityReadState>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, TEntity entity, TEntityReadState state)
    where TEntity : class
    where TEntityReadState : class;

public delegate void ReadChildNode<TEntity>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, TEntity entity)
    where TEntity : class;

public delegate void ReadNodeValue<TEntity>(XmlDictionaryReader reader, TEntity entity)
    where TEntity : class;
