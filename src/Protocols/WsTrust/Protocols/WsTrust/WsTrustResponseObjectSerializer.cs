using Solid.IdentityModel.Protocols.WsTrust;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Solid.Identity.Protocols.WsTrust
{
    internal class WsTrustResponseObjectSerializer : XmlObjectSerializer
    {
        private WsTrustConstants _version;
        private WsTrustSerializer _inner;
        private readonly bool _intermediate;

        public WsTrustResponseObjectSerializer(WsTrustConstants version, WsTrustSerializer inner, bool intermediate = false)
        {
            _version = version;
            _inner = inner;
            _intermediate = intermediate;
        }
        public override bool IsStartObject(XmlDictionaryReader reader) => throw new NotSupportedException();

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName) => throw new NotSupportedException();

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            var response = graph as WsTrustResponse;
            if (graph == null)
                throw new ArgumentException($"Cannot serialize {graph.GetType().Name} using WsTrustResponseSerializer.");

            if (_intermediate)
            {
                if (response.RequestSecurityTokenResponseCollection.Count != 1)
                    throw new InvalidOperationException("An intermediate response must contain exactly one RSTR.");
                _inner.WriteRequestSecurityTokenResponse(writer, _version, response.RequestSecurityTokenResponseCollection[0]);
            }
            else
                _inner.WriteResponse(writer, _version, response);
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
        }
    }
}
