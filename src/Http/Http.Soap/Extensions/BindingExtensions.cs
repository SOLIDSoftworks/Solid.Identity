using Solid.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Solid.Http.Soap;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace System.ServiceModel.Channels
{
    public static class BindingExtensions
    {
        public static CustomBinding WithSolidHttpTransport(this Binding binding, ISolidHttpClient client, XmlWriterSettings writerSettings = null)
        {
            var settings = writerSettings?.Clone() ?? new XmlWriterSettings();
            settings.CloseOutput = false;

            var custom = binding as CustomBinding;
            if (custom == null)
                custom = new CustomBinding(binding);

            var http = custom
                .Elements
                .OfType<HttpTransportBindingElement>()
                .FirstOrDefault(e => e.Scheme == "http" || e.Scheme == "https")
            ;
            if (http != null)
            {
                var solid = new SolidHttpTransportBindingElement(client, settings);
                custom.Elements.Remove(http);
                custom.Elements.Add(solid);
            }
            return custom;
        }
    }
}
