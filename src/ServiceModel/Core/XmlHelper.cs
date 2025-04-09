using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Solid.ServiceModel
{
    /// <summary>
    /// Helper methods for dealing with XML.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Creates an <see cref="XmlElement"/> by using a delegate on an <see cref="XmlDictionaryWriter"/>.
        /// </summary>
        /// <param name="action">A delegate for writing XML.</param>
        /// <returns>An <see cref="XmlElement"/>.</returns>
        public static XmlElement CreateElement(Action<XmlDictionaryWriter> action)
        {
            using(var stream = new MemoryStream())
            {
                var settings = new XmlWriterSettings
                {
                    NewLineHandling = NewLineHandling.None,
                    Indent = false,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = true,
                    CloseOutput = false
                };
                using (var writer = XmlWriter.Create(stream, settings))
                using (var dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer))
                    action(dictionaryWriter);
                stream.Position = 0;
                var document = new XmlDocument();
                document.Load(stream);

                return document.DocumentElement;
            }
        }
    }
}
