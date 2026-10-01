using System;
using System.Xml;
using Microsoft.IdentityModel.Xml;
using XmlException = System.Xml.XmlException;

namespace Solid.IdentityModel.Protocols.WsSecurity;

public class WsSecurityUtilitySerializer : ProtocolSerializer, IProtocolSerializer<Timestamp>
{
    protected override WsProtocolConstants GetProtocolConstants(WsSerializationContext context)
        => context.SecurityUtility;

    protected override string[] SupportedEntities => [WsSecurityUtilityElements.Timestamp];

    Timestamp IProtocolSerializer<Timestamp>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
        => ReadTimestamp(reader, serializer, CreateContext(reader));

    Timestamp IProtocolSerializer<Timestamp>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        => ReadTimestamp(reader, serializer, context);

    private Timestamp ReadTimestamp(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
    {
        AssertReader(reader, WsSecurityUtilityElements.Timestamp, context);
        _ = TryReadTimestamp(reader, serializer, context, out var timestamp);
        return timestamp;
    }

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out Timestamp entity)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        reader.MoveToContent();
        if (reader.LocalName != WsSecurityUtilityElements.Timestamp || !WsSecurityUtilityConstants.KnownNamespaces.ContainsKey(reader.NamespaceURI))
            return Out.False(out entity);
        return TryReadTimestamp(reader, serializer, CreateContext(reader), out entity);
    }

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
        out Timestamp entity)
        => TryReadTimestamp(reader, serializer, context, out entity);

    void IProtocolSerializer<Timestamp>.WriteEntity(XmlDictionaryWriter writer, Timestamp entity, WsSerializer serializer, WsSerializationContext context)
        => WriteTimestamp(writer, entity, serializer, context);

    protected virtual void WriteTimestamp(XmlDictionaryWriter writer, Timestamp entity, WsSerializer serializer, WsSerializationContext context)
    {
        WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));
        var utility = context.SecurityUtility ?? throw new ArgumentNullException(nameof(context.SecurityUtility));
        writer.WriteStartElement(utility.DefaultPrefix, WsSecurityUtilityElements.Timestamp, utility.Namespace);
        if (!string.IsNullOrEmpty(entity.Id))
            writer.WriteAttributeString(utility.DefaultPrefix, WsSecurityUtilityAttributes.Id, utility.Namespace, entity.Id);

        writer.WriteElementString(utility.DefaultPrefix, WsSecurityUtilityElements.Created, utility.Namespace,
            XmlConvert.ToString(entity.Created.ToUniversalTime(), XmlDateTimeSerializationMode.Utc));
        writer.WriteElementString(utility.DefaultPrefix, WsSecurityUtilityElements.Expires, utility.Namespace,
            XmlConvert.ToString(entity.Expires.ToUniversalTime(), XmlDateTimeSerializationMode.Utc));
        writer.WriteEndElement();
    }
    
    protected virtual bool TryReadTimestamp(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out Timestamp timestamp)
    {
        if(reader.LocalName != WsSecurityUtilityElements.Timestamp || reader.NamespaceURI != context.SecurityUtility.Namespace)
            return Out.False(out timestamp);

        var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
        var t = CreateTimestamp(context, attributes);
        ReadNode(reader, serializer, context, t, ReadTimestampChildNode);

        timestamp = t;
        return true;
    }

    protected virtual Timestamp CreateTimestamp(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
        => new Timestamp { Id = GetQualifiedAttribute(attributes, WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace) };

    private static string GetQualifiedAttribute(XmlAttributeDescriptor[] attributes, string name, string ns)
    {
        foreach (var attribute in attributes)
            if (attribute.LocalName == name && attribute.NamespaceUri == ns)
                return attribute.Value;
        return null;
    }

    protected virtual void ReadTimestampChildNode(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, Timestamp timestamp)
    {
        if (TryReadDateTime(WsSecurityUtilityElements.Created, reader, context, out var created))
            timestamp.Created = created;
        else if (TryReadDateTime(WsSecurityUtilityElements.Expires, reader, context, out var expires))
            timestamp.Expires = expires;
        else
            reader.Skip();
    }

    protected bool TryReadDateTime(string name, XmlDictionaryReader reader, WsSerializationContext context, out DateTime value)
    {
        if(reader.LocalName != name || reader.NamespaceURI != context.SecurityUtility.Namespace)
            return Out.False(out value);
        
        value = XmlConvert.ToDateTime(reader.ReadElementContentAsString(), XmlDateTimeSerializationMode.Utc);
        return true;
    }
    
    protected override WsSerializationContext CreateContext(string ns)
    {
        if(!WsSecurityUtilityConstants.KnownNamespaces.TryGetValue(ns, out var wsu))
            // Create IDX error
            throw new XmlException("Cannot create WS serialization context for " + ns);
            
        return new WsSerializationContext
        {
            SecurityUtility = wsu
        };
    }
}
