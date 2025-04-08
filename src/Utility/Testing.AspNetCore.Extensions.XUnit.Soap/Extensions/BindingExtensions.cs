using Solid.Http;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using Solid.Testing.AspNetCore.Extensions.XUnit.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace System.ServiceModel.Channels
{
    public static class BindingExtensions
    {
        public static Binding WithSolidHttpTransport(this Binding binding, TestingServer server, XmlWriterSettings writerSettings = null)
        {
            var settings = writerSettings?.Clone() ?? new XmlWriterSettings();
            settings.CloseOutput = false;

            var custom = binding as CustomBinding;
            if (custom == null)
                custom = new CustomBinding(binding);

            var http = custom
                .Elements
                .OfType<HttpTransportBindingElement>()
                .Where(e => e.Scheme == "http" || e.Scheme == "https")
                .FirstOrDefault()
            ;
            if (http != null)
            {
                var solid = new SolidHttpTransportBindingElement(server, settings);
                custom.Elements.Remove(http);
                custom.Elements.Add(solid);
            }
            return custom;
        }
    }
}
