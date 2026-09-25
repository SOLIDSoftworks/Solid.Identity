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

    public SecurityContextToken ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
    {
        if (reader == null)
            throw LogHelper.LogArgumentNullException(nameof(reader));

        reader.MoveToContent();
        if (!WsSecureConversationConstants.KnownNamespaces.TryGetValue(reader.NamespaceURI, out var constants))
            throw XmlUtil.LogReadException(LogMessages.IDX15011,
                WsSecureConversationConstants.SecureConversation13.Namespace,
                WsSecureConversationElements.SecurityContextToken, reader.NamespaceURI, reader.LocalName);

        return ReadEntity(reader, serializer, new WsSerializationContext
        {
            SecureConversation = constants,
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10
        });
    }

    public SecurityContextToken ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
    {
        AssertReader(reader, WsSecureConversationElements.SecurityContextToken, context);
        _ = TryReadEntity(reader, serializer, context, out var token);
        return token;
    }

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out SecurityContextToken entity)
    {
        if (reader == null)
            throw LogHelper.LogArgumentNullException(nameof(reader));

        reader.MoveToContent();
        if (!WsSecureConversationConstants.KnownNamespaces.TryGetValue(reader.NamespaceURI, out var constants))
            return Out.False(out entity);

        return TryReadEntity(reader, serializer, new WsSerializationContext
        {
            SecureConversation = constants,
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10
        }, out entity);
    }

    public bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out SecurityContextToken entity)
    {
        if (reader == null)
            throw LogHelper.LogArgumentNullException(nameof(reader));
        if (context == null)
            throw LogHelper.LogArgumentNullException(nameof(context));
        var constants = GetProtocolConstants(context);
        reader.MoveToContent();
        if (!reader.IsStartElement(WsSecureConversationElements.SecurityContextToken, constants.Namespace))
            return Out.False(out entity);

        var token = new SecurityContextToken();
        var utilityNamespace = context.SecurityUtility?.Namespace ?? WsSecurityUtilityConstants.SecurityUtility10.Namespace;
        token.Id = reader.GetAttribute(WsSecurityUtilityAttributes.Id, utilityNamespace);
        foreach (var attribute in XmlAttributeDescriptor.ReadAttributes(reader))
        {
            if (attribute.LocalName != WsSecurityUtilityAttributes.Id || attribute.NamespaceUri != utilityNamespace)
                token.AdditionalXmlAttributes.Add(attribute);
        }

        bool empty = reader.IsEmptyElement;
        reader.ReadStartElement();
        if (!empty)
        {
            reader.MoveToContent();
            while (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.IsStartElement(WsSecureConversationElements.Identifier, constants.Namespace))
                    token.Identifier = new Identifier { Value = reader.ReadElementContentAsString() };
                else if (reader.IsStartElement(WsSecureConversationElements.Instance, constants.Namespace))
                    token.Instance = reader.ReadElementContentAsString();
                else
                    ReadAdditionalXmlElement(reader, token);

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        if (!Uri.TryCreate(token.Identifier?.Value, UriKind.Absolute, out _))
            throw XmlUtil.LogReadException(LogMessages.IDX15011,
                constants.Namespace, WsSecureConversationElements.Identifier, constants.Namespace, token.Identifier?.Value ?? string.Empty);

        entity = token;
        return true;
    }

    public void WriteEntity(XmlDictionaryWriter writer, SecurityContextToken entity, WsSerializer serializer, WsSerializationContext context)
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
