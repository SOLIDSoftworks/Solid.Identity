using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using XmlException = System.Xml.XmlException;

namespace Solid.IdentityModel.Protocols;

public class WsSerializer
{
    private readonly IEnumerable<ProtocolSerializer> _serializers;
    private readonly ConcurrentDictionary<Type, ProtocolSerializer?> _cache;
    public WsSerializer(IEnumerable<ProtocolSerializer> serializers)
    {
        _serializers = serializers;
        _cache = new ConcurrentDictionary<Type, ProtocolSerializer?>();
    }

    public T? ReadEntity<T>(XmlDictionaryReader reader)
        where T : class
    {
        var serializer = GetSerializerFor<T>();
        return serializer?.ReadEntity(reader, this);
    }

    public T? ReadEntity<T>(XmlDictionaryReader reader, WsSerializationContext context)
        where T : class
    {
        var serializer = GetSerializerFor<T>();
        return serializer?.ReadEntity(reader, this, context);
    }

    public bool TryReadEntity<T>(XmlDictionaryReader reader, [MaybeNullWhen(false)] out T entity)
        where T : class
    {
        var serializer = GetSerializerFor<T>();
        if (serializer == null)
            return Out.False(out entity);
        return serializer.TryReadEntity(reader, this, out entity);
    }

    public bool TryReadEntity<T>(XmlDictionaryReader reader, WsSerializationContext context, [MaybeNullWhen(false)] out T entity)
        where T : class
    {
        var serializer = GetSerializerFor<T>();
        if (serializer == null)
            return Out.False(out entity);
        return serializer.TryReadEntity(reader, this, context, out entity);
    }

    public void WriteEntity<T>(XmlDictionaryWriter writer, T entity, WsSerializationContext context)
        where T : class
    {
        var serializer = GetSerializerFor<T>();
        if (serializer == null)
            // TODO: create idx error 
            throw new XmlException();
        serializer.WriteEntity(writer, entity, this, context);
    }

    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    private IProtocolSerializer<T>? GetSerializerFor<T>()
        where T : class
    {
        var type = typeof(IProtocolSerializer<T>);
        return _cache.GetOrAdd(type, _ => FindSerializerFor<T>()) as IProtocolSerializer<T>;
    }
    

    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    private ProtocolSerializer? FindSerializerFor<T>()
        where T : class
    {
        var serializer = _serializers.FirstOrDefault(s => s is IProtocolSerializer<T>);
        return serializer;
    }
}