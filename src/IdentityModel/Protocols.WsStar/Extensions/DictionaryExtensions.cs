using System.Xml;

namespace System.Collections.Generic;

internal static class DictionaryExtensions
{
    public static bool TryGetValue<T>(this IDictionary<string, T> dictionary, XmlDictionaryString key, out T value)
    {
        return dictionary.TryGetValue(key.Value, out value);
    }
}