using System;
using System.Xml;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Protocols.WsSecurity;
using XmlException = Microsoft.IdentityModel.Xml.XmlException;

namespace Solid.IdentityModel.Protocols.XmlEnc;

public class WsSecurityExampleSerializer : ProtocolSerializer,
    IProtocolSerializer<ExampleValueEntity>,
    IProtocolSerializer<ExampleXmlOpenEntity>,
    IProtocolSerializer<ExampleKnownChildEntity>
{
    // Always returns the corresponding constants so that the serializer can read and write xml entities in the correct namespace
    protected override WsProtocolConstants GetProtocolConstants(WsSerializationContext context)
        => context.Security;

    protected override WsSerializationContext CreateContext(string ns)
    {
        if(!WsSecurityConstants.KnownNamespaces.TryGetValue(ns, out var ws))
            // Create IDX error
            throw new XmlException("Cannot create WS serialization context for " + ns);

        return new WsSerializationContext
        {
            Security = ws,
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10,
        };
    }

    protected override string[] SupportedEntities => [nameof(ExampleValueEntity), nameof(ExampleXmlOpenEntity), nameof(ExampleKnownChildEntity)];

    #region ExampleValueEntity

    ExampleValueEntity IProtocolSerializer<ExampleValueEntity>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
        => ReadExampleValueEntity(reader, serializer, CreateContext(reader));

    ExampleValueEntity IProtocolSerializer<ExampleValueEntity>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        => ReadExampleValueEntity(reader, serializer, context);

    bool IProtocolSerializer<ExampleValueEntity>.TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out ExampleValueEntity entity)
        => TryReadExampleValueEntity(reader, serializer, CreateContext(reader), out entity);

    bool IProtocolSerializer<ExampleValueEntity>.TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleValueEntity entity)
        => TryReadExampleValueEntity(reader, serializer, context, out entity);

    void IProtocolSerializer<ExampleValueEntity>.WriteEntity(XmlDictionaryWriter writer, ExampleValueEntity entity, WsSerializer serializer, WsSerializationContext context)
        => WriteExampleValueEntity(writer, entity, serializer, context);

    // Extension point for reading entity. Naming convention: TryRead<EntityName>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleEntity exampleEntity)
    protected virtual bool TryReadExampleValueEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleValueEntity entity)
    {
        // Get the expected namespace from the context
        var expectedNamespace = context.Security.Namespace;

        // If the reader doesn't match the expected location, return false
        if(reader.LocalName != nameof(ExampleValueEntity) || reader.NamespaceURI != expectedNamespace)
            return Out.False(out entity);

        // Read attributes
        var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
        var result = CreateExampleValueEntity(context, attributes);
        // If the entity is a value entity (only has inner-text), then just read the value directly and set the property of the result.
        ReadNode(reader, result, ReadValue);
        entity = result;
        return true;
    }

    // Extension point for writing entity. Naming convention: Write<EntityName>(XmlDictionaryWriter writer, ExampleEntity entity, WsSerializer serializer, WsSerializationContext context)
    protected virtual void WriteExampleValueEntity(XmlDictionaryWriter writer, ExampleValueEntity entity, WsSerializer serializer, WsSerializationContext context)
    {
        WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));

        // Write entity. Perform null checks if necessary
        writer.WriteStartElement(context.Security.DefaultPrefix, nameof(ExampleValueEntity), context.Security.Namespace);
        writer.WriteAttributeString("Id", context.SecurityUtility.Namespace, entity.Id);
        writer.WriteValue(entity.Value);
        writer.WriteEndElement();
    }

    // Extension point for creating the entity. This is used if there exists an entity that uses our entity class as it's base class
    protected virtual ExampleValueEntity CreateExampleValueEntity(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
    {
        var entity = new ExampleValueEntity
        {
            // Use correct attribute name and namespace
            Id = XmlAttributeDescriptor.GetAttribute(attributes, "Id", context.SecurityUtility.Namespace),
        };
        return entity;
    }

    private void ReadValue(XmlDictionaryReader reader, ExampleValueEntity entity)
    {
        // Reads the entity value using the correct Read method for the value type.
        entity.Value = reader.ReadString();
    }

    private ExampleValueEntity ReadExampleValueEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
    {
        // Asserts reader location. Use constant from WsStar project for element name
        AssertReader(reader, nameof(ExampleValueEntity), context);
        _ = TryReadExampleValueEntity(reader, serializer, context, out var exampleEntity);
        return exampleEntity;
    }

    #endregion ExampleValueEntity

    #region ExampleXmlOpenEntity

    ExampleXmlOpenEntity IProtocolSerializer<ExampleXmlOpenEntity>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        => ReadExampleXmlOpenEntity(reader, serializer, context);

    bool IProtocolSerializer<ExampleXmlOpenEntity>.TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out ExampleXmlOpenEntity entity)
        => TryReadExampleXmlOpenEntity(reader, serializer, CreateContext(reader), out entity);

    bool IProtocolSerializer<ExampleXmlOpenEntity>.TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleXmlOpenEntity entity)
        => TryReadExampleXmlOpenEntity(reader, serializer, context, out entity);

    void IProtocolSerializer<ExampleXmlOpenEntity>.WriteEntity(XmlDictionaryWriter writer, ExampleXmlOpenEntity entity, WsSerializer serializer, WsSerializationContext context)
        => WriteExampleXmlOpenEntity(writer, entity, serializer, context);

    ExampleXmlOpenEntity IProtocolSerializer<ExampleXmlOpenEntity>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
        => ReadExampleXmlOpenEntity(reader, serializer, CreateContext(reader));

    // Extension point for reading entity. Naming convention: TryRead<EntityName>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleEntity exampleEntity)
    protected virtual bool TryReadExampleXmlOpenEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleXmlOpenEntity entity)
    {
        // Get the expected namespace from the context
        var expectedNamespace = context.Security.Namespace;

        // If the reader doesn't match the expected location, return false
        if(reader.LocalName != nameof(ExampleXmlOpenEntity) || reader.NamespaceURI != expectedNamespace)
            return Out.False(out entity);

        // Read attributes
        var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
        var result = CreateExampleXmlOpenEntity(context, attributes);
        // If the entity can have child nodes, then we iterate through the child nodes using ReadNode
        ReadNode(reader, serializer, context, result, ReadExampleXmlOpenEntityChildNode);
        entity = result;
        return true;
    }

    // Extension point for writing entity. Naming convention: Write<EntityName>(XmlDictionaryWriter writer, ExampleEntity entity, WsSerializer serializer, WsSerializationContext context)
    protected virtual void WriteExampleXmlOpenEntity(XmlDictionaryWriter writer, ExampleXmlOpenEntity entity, WsSerializer serializer, WsSerializationContext context)
    {
        WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));

        // Write entity. Perform null checks if necessary
        writer.WriteStartElement(context.Security.DefaultPrefix, nameof(ExampleXmlOpenEntity), context.Security.Namespace);
        writer.WriteAttributeString("Id", context.SecurityUtility.Namespace);
        // Since this is an XmlOpenItem, write attributes that aren't strictly in the contract
        WriteXmlOpenItemAttributes(writer, context, entity);
        // Directly call known entity writer
        WriteExampleKnownChildEntity(writer, entity.Known, serializer, context);
        // Use the WsSerializer for unknown entities to offload the writing to a separate protocol serializer
        serializer.WriteEntity(writer, entity.Unknown, context);
        // Write the child nodes that aren't strictly part of the contract
        WriteXmlOpenItemElements(writer, context, entity);
        writer.WriteEndElement();
    }

    // Extension point for creating the entity. This is used if there exists an entity that uses our entity class as it's base class
    protected virtual ExampleXmlOpenEntity CreateExampleXmlOpenEntity(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
    {
        var entity = new ExampleXmlOpenEntity();

        foreach(var attribute in attributes)
        {
            if (attribute.LocalName == "Id" && attribute.NamespaceUri == context.SecurityUtility.Namespace)
                entity.Id = attribute.Value;
            else
                entity.AdditionalXmlAttributes.Add(attribute);
        };
        return entity;
    }

    protected virtual void ReadExampleXmlOpenEntityChildNode(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, ExampleXmlOpenEntity entity)
    {
        // Create an if statement that trys to read each expected child
        // If the child element type is known to the current serializer, use the Try method directly
        if (TryReadExampleKnownChildEntity(reader, serializer, context, out var known))
            entity.Known = known;
        // Since ExampleXmlOpenEntity is an XmlOpenItem, then we default to reading the raw xml elements
        else
            ReadAdditionalXmlElement(reader, entity);
    }

    private ExampleXmlOpenEntity ReadExampleXmlOpenEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
    {
        // Asserts reader location. Use constant from WsStar project for element name
        AssertReader(reader, nameof(ExampleValueEntity), context);
        _ = TryReadExampleXmlOpenEntity(reader, serializer, context, out var exampleKnownChildEntity);
        return exampleKnownChildEntity;
    }


    #endregion ExampleXmlOpenEntity

    #region ExampleKnownChildEntity

    ExampleKnownChildEntity IProtocolSerializer<ExampleKnownChildEntity>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
        => ReadExampleKnownChildEntity(reader, serializer, context);

    bool IProtocolSerializer<ExampleKnownChildEntity>.TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out ExampleKnownChildEntity entity)
        => TryReadExampleKnownChildEntity(reader, serializer, CreateContext(reader), out entity);

    bool IProtocolSerializer<ExampleKnownChildEntity>.TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleKnownChildEntity entity)
        => TryReadExampleKnownChildEntity(reader, serializer, context, out entity);

    void IProtocolSerializer<ExampleKnownChildEntity>.WriteEntity(XmlDictionaryWriter writer, ExampleKnownChildEntity entity, WsSerializer serializer, WsSerializationContext context)
        => WriteExampleKnownChildEntity(writer, entity, serializer, context);

    ExampleKnownChildEntity IProtocolSerializer<ExampleKnownChildEntity>.ReadEntity(XmlDictionaryReader reader, WsSerializer serializer)
        => ReadExampleKnownChildEntity(reader, serializer, CreateContext(reader));

    // Extension point for reading entity. Naming convention: TryRead<EntityName>(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleEntity exampleEntity)
    protected virtual bool TryReadExampleKnownChildEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out ExampleKnownChildEntity entity)
    {
        // Get the expected namespace from the context
        var expectedNamespace = context.Security.Namespace;

        // If the reader doesn't match the expected location, return false
        if(reader.LocalName != nameof(ExampleKnownChildEntity) || reader.NamespaceURI != expectedNamespace)
            return Out.False(out entity);

        // Read attributes
        var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
        var result = CreateExampleKnownChildEntity(context, attributes);
        // If the entity can have child nodes, then we iterate through the child nodes using ReadNode
        ReadNode(reader, serializer, context, result, ReadExampleKnownChildEntityChildNode);
        entity = result;
        return true;
    }

    // Extension point for writing entity. Naming convention: Write<EntityName>(XmlDictionaryWriter writer, ExampleEntity entity, WsSerializer serializer, WsSerializationContext context)
    protected virtual void WriteExampleKnownChildEntity(XmlDictionaryWriter writer, ExampleKnownChildEntity entity, WsSerializer serializer, WsSerializationContext context)
    {
        WsUtils.ValidateParamsForWriting(writer, context, entity, nameof(entity));

        // Write entity. Perform null checks if necessary
        writer.WriteStartElement(context.Security.DefaultPrefix, nameof(ExampleKnownChildEntity), context.Security.Namespace);
        // Directly call known entity writer
        WriteExampleValueEntity(writer, entity.Value, serializer, context);
        writer.WriteEndElement();
    }

    // Extension point for creating the entity. This is used if there exists an entity that uses our entity class as it's base class
    protected virtual ExampleKnownChildEntity CreateExampleKnownChildEntity(WsSerializationContext context, XmlAttributeDescriptor[] attributes)
    {
        var entity = new ExampleKnownChildEntity();
        return entity;
    }

    protected virtual void ReadExampleKnownChildEntityChildNode(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, ExampleKnownChildEntity entity)
    {
        // Create an if statement that trys to read each expected child
        // If the child element type is known to the current serializer, use the Try method directly
        if (TryReadExampleValueEntity(reader, serializer, context, out var value))
            entity.Value = value;
        // Since ExampleKnownChildEntity is not an XmlOpenItem, then we skip unknown child nodes
        else
            reader.Skip();
    }

    private ExampleKnownChildEntity ReadExampleKnownChildEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context)
    {
        // Asserts reader location. Use constant from WsStar project for element name
        AssertReader(reader, nameof(ExampleValueEntity), context);
        _ = TryReadExampleKnownChildEntity(reader, serializer, context, out var exampleKnownChildEntity);
        return exampleKnownChildEntity;
    }

    #endregion ExampleKnownChildEntity
    // Add more supported entities
}

public class ExampleValueEntity
{
    public string Id { get; set; }
    public string Value { get; set; }
}

public class ExampleXmlOpenEntity : XmlOpenItem
{
    public string Id { get; set; }
    public ExampleKnownChildEntity Known { get; set; }
    public ExampleUnknownChildEntity Unknown { get; set; }
}

public class ExampleKnownChildEntity
{
    public ExampleValueEntity Value { get; set; }
}

public class ExampleUnknownChildEntity
{

}
