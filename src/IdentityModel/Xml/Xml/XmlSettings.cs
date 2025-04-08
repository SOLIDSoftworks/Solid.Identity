using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Solid.IdentityModel.Xml
{
    public static class XmlSettings
    {
        public static XmlWriterSettings DefaultWriterSettings => new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = false,
            CloseOutput = false,
            Encoding = new UTF8Encoding(false)
        };

        public static XmlWriterSettings CreateWriterSettings(Action<XmlWriterSettings> configure)
        {
            var settings = DefaultWriterSettings;
            configure(settings);
            return settings;
        }
    }
}
