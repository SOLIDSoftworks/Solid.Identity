using System;
using System.Linq;
using System.Xml;

namespace Solid.IdentityModel.Protocols.WsSecurity;

public class WsSecurityUtilitySerializer : ProtocolSerializer, IProtocolSerializer<Timestamp>
{
    protected override WsProtocolConstants GetProtocolConstants(WsSerializationContext context)
        => context.SecurityUtility;

    Timestamp IProtocolSerializer<Timestamp>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
    {
        var context = CreateContext(reader);
        return TimestampSerializer.ReadEntity(reader, serializer, context);
    }

    Timestamp IProtocolSerializer<Timestamp>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
    {
        AssertReader(reader, WsSecurityUtilityElements.Timestamp, context);
        _ = TryReadTimestamp(reader, context, serializer, out var timestamp);
        return timestamp;
    }

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out Timestamp entity)
        => TryReadTimestamp(reader, CreateContext(reader), serializer, out entity);

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context,
        out Timestamp entity)
        => TryReadTimestamp(reader, context, serializer, out entity);

    void IProtocolSerializer<Timestamp>.WriteEntity(XmlDictionaryWriter writer, Timestamp entity, WsSerializer serializer, WsSerializationContext context)
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
    
    protected virtual bool TryReadTimestamp(XmlDictionaryReader reader, WsSerializationContext context, WsSerializer serializer, out Timestamp timestamp)
    {
        if(reader.LocalName != WsSecurityUtilityElements.Timestamp || reader.NamespaceURI != context.SecurityUtility.Namespace)
            return Out.False(out timestamp);

        var t = new Timestamp
        {
            Id = reader.GetAttribute(WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace)
        };
        var empty = reader.IsEmptyElement;
        reader.ReadStartElement();

        if (empty)
            return Out.True(t, out timestamp);

        reader.MoveToContent();
        while (reader.NodeType == XmlNodeType.Element)
        {
            ReadTimestampChildNode(reader, context, serializer, t);
            reader.MoveToContent();
        }
        reader.ReadEndElement();

        timestamp = t;
        return true;
    }

    protected virtual void ReadTimestampChildNode(XmlDictionaryReader reader, WsSerializationContext context, WsSerializer serializer, Timestamp timestamp)
    {
        if (TryReadDateTime(WsSecurityUtilityElements.Created, reader, context, out var created))
            timestamp.Created = created;
        else if (TryReadDateTime(WsSecurityUtilityElements.Expires, reader, context, out var expires))
            timestamp.Expires = expires;
        else
            reader.Skip();
    }

    protected IProtocolSerializer<Timestamp> TimestampSerializer => this;

    protected bool TryReadDateTime(string name, XmlDictionaryReader reader, WsSerializationContext context, out DateTime value)
    {
        if(reader.LocalName != name || reader.NamespaceURI != context.SecurityUtility.Namespace)
            return Out.False(out value);
        
        value = XmlConvert.ToDateTime(reader.ReadElementContentAsString(), XmlDateTimeSerializationMode.Utc);
        return true;
    }
    
    private WsSerializationContext CreateContext(XmlDictionaryReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException(nameof(reader));

        reader.MoveToContent();
        var name = reader.LocalName;
        if (!WsSecurityUtilityElements.All.Contains(name))
            throw new XmlException("Cannot create WS serialization context for " + name);

        var ns = reader.NamespaceURI;
        return CreateContext(ns);
    }

    private WsSerializationContext CreateContext(string ns)
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
