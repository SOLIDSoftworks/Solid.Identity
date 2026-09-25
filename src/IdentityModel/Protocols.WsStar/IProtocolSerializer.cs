using System.Xml;

namespace Solid.IdentityModel.Protocols;

public interface IProtocolSerializer<T>
    where T : class
{
    T ReadEntity(XmlDictionaryReader reader, WsSerializer serializer);
    T ReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context);
    bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, out T entity);
    bool TryReadEntity(XmlDictionaryReader reader, WsSerializer serializer, WsSerializationContext context, out T entity);
    void WriteEntity(XmlDictionaryWriter writer, T entity, WsSerializer serializer, WsSerializationContext context);
}