using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Xml
{
    public static class XmlReaderExtensions
    {
        public static void ForEachChild<TReader>(this TReader reader, Func<TReader, bool> tryRead)
            where TReader : XmlReader
        {
            if (!reader.IsEmptyElement && reader.Read())
            {
                while (reader.IsStartElement())
                {
                    if (!tryRead(reader))
                        reader.ReadOuterXml();
                }
            }
            reader.Read();
        }

        public static void ForEachChild(this XmlDictionaryReader reader, Func<XmlDictionaryReader, bool> tryRead, out Signature signature)
        {
            using(var signatureReader = new EnvelopedSignatureReader(reader))
            {
                if (!signatureReader.IsEmptyElement && reader.Read())
                {
                    while (signatureReader.IsStartElement())
                    {
                        if (!tryRead(signatureReader))
                            signatureReader.ReadOuterXml();
                    }
                }
                signatureReader.Read();
                signature = signatureReader.Signature;
            }
        }

        public static bool TryReadAttribute(this XmlReader reader, string name, out string value)
        {
            var str = reader.GetAttribute(name);
            value = str;
            return value != null;
        }
        public static bool TryReadAttribute(this XmlReader reader, string localName, string ns, out string value)
        {
            var str = reader.GetAttribute(localName, ns);
            value = str;
            return value != null;
        }

        public static bool TryReadAttributeAsDateTime(this XmlReader reader, string name, out DateTime? value)
            => reader.TryReadAttributeAs(name, str =>
            {
                try
                {
                    return XmlConvert.ToDateTime(str, XmlDateTimeSerializationMode.Utc);
                }
                catch
                {
                    return null;
                }
            }, out value);

        public static bool TryReadAttributeAsUri(this XmlReader reader, string name, out Uri value)
            => reader.TryReadAttributeAs(name, str =>
            {
                if (!Uri.IsWellFormedUriString(str, UriKind.Absolute)) return null;
                return new Uri(str);
            }, out value);

        public static bool TryReadAttributeAsTimeSpan(this XmlReader reader, string name, out TimeSpan? value)
            => reader.TryReadAttributeAs(name, str =>
            {
                try
                {
                    return XmlConvert.ToTimeSpan(str);
                }
                catch
                {
                    return null;
                }
            }, out value);

        public static bool TryReadAttributeAsBoolean(this XmlReader reader, string name, out bool? value)
            => reader.TryReadAttributeAs(name, str =>
            {
                if (bool.TryParse(str, out var b)) return b;
                return null;
            }, out value);

        public static bool TryReadElementContent(this XmlReader reader, string name, string ns, out string content)
        {
            if (!reader.IsStartElement(name, ns)) return Out.False(out content);
            var value = reader.ReadElementContentAsString();
            content = value;
            return true;
        }

        public static Uri ReadElementContentAsUri(this XmlReader reader, UriKind kind = UriKind.Absolute)
        {
            var content = reader.ReadElementContentAsString();
            if (!Uri.IsWellFormedUriString(content, kind)) 
                throw new InvalidOperationException($"Unable to read content '{content}' as {kind} uri.");
            return new Uri(content, kind);
        }

        private static bool TryReadAttributeAs<T>(this XmlReader reader, string name, Func<string, T> convert, out T value)
        {
            if (!reader.TryReadAttribute(name, out var str)) return Out.False(out value);

            value = convert(str);
            return value != null;
        }
    }
}
