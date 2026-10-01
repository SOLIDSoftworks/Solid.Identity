using System;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Protocols.WsSecurity;

namespace Solid.IdentityModel.Protocols.WsSecureConversation;

/// <summary>
/// Reads and writes WS-SecureConversation security context tokens.
/// </summary>
public class WsSecureConversationSerializer : ProtocolSerializer, IProtocolSerializer<SecurityContextToken>
{
    protected override WsProtocolConstants GetProtocolConstants(WsSerializationContext context)
        => context.SecureConversation ?? throw new ArgumentNullException(nameof(context.SecureConversation));

    protected override string[] SupportedEntities => [WsSecureConversationElements.SecurityContextToken];

    protected override WsSerializationContext CreateContext(string ns)
    {
        if (!WsSecureConversationConstants.KnownNamespaces.TryGetValue(ns, out var constants))
            throw XmlUtil.LogReadException(LogMessages.IDX15011,
                WsSecureConversationConstants.SecureConversation13.Namespace,
                WsSecureConversationElements.SecurityContextToken, ns, WsSecureConversationElements.SecurityContextToken);

        return new WsSerializationContext
        {
            SecureConversation = constants,
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10
        };
    }

    public SecurityContextToken ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
        => ReadSecurityContextToken(reader, serializer, CreateContext(reader));

    public SecurityContextToken ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        => ReadSecurityContextToken(reader, serializer, context);

    private SecurityContextToken ReadSecurityContextToken(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
    {
        AssertReader(reader, WsSecureConversationElements.SecurityContextToken, context);
        _ = TryReadSecurityContextToken(reader, serializer, context, out var token);
        return token;
    }

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out SecurityContextToken entity)
    {
        if (reader == null)
            throw LogHelper.LogArgumentNullException(nameof(reader));
        reader.MoveToContent();
        if (reader.LocalName != WsSecureConversationElements.SecurityContextToken || !WsSecureConversationConstants.KnownNamespaces.ContainsKey(reader.NamespaceURI))
            return Out.False(out entity);
        return TryReadSecurityContextToken(reader, serializer, CreateContext(reader), out entity);
    }

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out SecurityContextToken entity)
        => TryReadSecurityContextToken(reader, serializer, context, out entity);

    protected virtual bool TryReadSecurityContextToken(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out SecurityContextToken entity)
    {
        if (reader == null)
            throw LogHelper.LogArgumentNullException(nameof(reader));
        if (context == null)
            throw LogHelper.LogArgumentNullException(nameof(context));
        var constants = GetProtocolConstants(context);
        reader.MoveToContent();
        if (reader.LocalName != WsSecureConversationElements.SecurityContextToken || reader.NamespaceURI != constants.Namespace)
            return Out.False(out entity);

        var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
        var token = CreateSecurityContextToken(context, attributes);
        ReadNode(reader, serializer, context, token, ReadSecurityContextTokenChildNode);

        if (!Uri.TryCreate(token.Identifier?.Value, UriKind.Absolute, out _))
            throw XmlUtil.LogReadException(LogMessages.IDX15011,
                constants.Namespace, WsSecureConversationElements.Identifier, constants.Namespace, token.Identifier?.Value ?? string.Empty);

        entity = token;
        return true;
    }

    protected virtual SecurityContextToken CreateSecurityContextToken(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
    {
        var token = new SecurityContextToken();
        var utilityNamespace = context.SecurityUtility?.Namespace ?? WsSecurityUtilityConstants.SecurityUtility10.Namespace;
        foreach (var attribute in attributes)
        {
            if (attribute.LocalName == WsSecurityUtilityAttributes.Id && attribute.NamespaceUri == utilityNamespace)
                token.Id = attribute.Value;
            else
                token.AdditionalXmlAttributes.Add(attribute);
        }
        return token;
    }

    protected virtual void ReadSecurityContextTokenChildNode(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, SecurityContextToken token)
    {
        var constants = GetProtocolConstants(context);
        if (reader.LocalName == WsSecureConversationElements.Identifier && reader.NamespaceURI == constants.Namespace)
            token.Identifier = new Identifier { Value = reader.ReadElementContentAsString() };
        else if (reader.LocalName == WsSecureConversationElements.Instance && reader.NamespaceURI == constants.Namespace)
            token.Instance = reader.ReadElementContentAsString();
        else
            ReadAdditionalXmlElement(reader, token);
    }

    public void WriteEntity(XmlDictionaryWriter writer, SecurityContextToken entity, WsSerializer serializer, WsSerializationContext context)
        => WriteSecurityContextToken(writer, entity, serializer, context);

    protected virtual void WriteSecurityContextToken(XmlDictionaryWriter writer, SecurityContextToken entity, WsSerializer serializer, WsSerializationContext context)
    {
        WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));
        var constants = GetProtocolConstants(context);
        if (!Uri.TryCreate(entity.Identifier?.Value, UriKind.Absolute, out _))
            throw new ArgumentException("Security context identifier must be an absolute URI.", nameof(entity));

        writer.WriteStartElement(constants.DefaultPrefix, WsSecureConversationElements.SecurityContextToken, constants.Namespace);
        if (!string.IsNullOrEmpty(entity.Id))
        {
            var utility = context.SecurityUtility ?? WsSecurityUtilityConstants.SecurityUtility10;
            writer.WriteAttributeString(utility.DefaultPrefix, WsSecurityUtilityAttributes.Id, utility.Namespace, entity.Id);
        }

        WriteXmlOpenItemAttributes(writer, context, entity);
        writer.WriteElementString(constants.DefaultPrefix, WsSecureConversationElements.Identifier, constants.Namespace, entity.Identifier.Value);
        if (entity.Instance != null)
            writer.WriteElementString(constants.DefaultPrefix, WsSecureConversationElements.Instance, constants.Namespace, entity.Instance);
        WriteXmlOpenItemElements(writer, context, entity);
        writer.WriteEndElement();
    }
}
