using Microsoft.Extensions.Options;
using Solid.IdentityModel.Protocols.WsTrust;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.IdentityModel.Protocols;

namespace Solid.Identity.Protocols.WsTrust
{
    public class WsTrustSerializerFactory
    {
        public WsTrustSerializerFactory(SecurityTokenHandlerProvider provider)
        {
            SecurityTokenHandlerProvider = provider;
        }

        protected SecurityTokenHandlerProvider SecurityTokenHandlerProvider { get; }

        public WsTrustSerializer Create()
        {
            var options = WsTrustContractOptions.Current.Value;
            var serializer = new CustomWsTrustSerializer(options);
            serializer.SecurityTokenHandlers.Clear();
            var handlers = SecurityTokenHandlerProvider.GetAllSecurityTokenHandlers();
            foreach (var handler in handlers)
                serializer.SecurityTokenHandlers.Add(handler);
            return serializer;
        }

        class CustomWsTrustSerializer : WsTrustSerializer
        {
            private readonly WsTrustContractOptions _options;

            public CustomWsTrustSerializer(WsTrustContractOptions options)
            {
                _options = options;
            }

            protected override WsSerializationContext CreateSerializationContext(WsTrustConstants version)
            {
                if (_options == null)
                    return base.CreateSerializationContext(version);

                var defaults = base.CreateSerializationContext(version);
                return new WsSerializationContext(version)
                {
                    Trust = _options.TrustConstants ?? defaults.Trust,
                    Addressing = _options.AddressingConstants ?? defaults.Addressing,
                    Federation = _options.FedConstants ?? defaults.Federation,
                    Security = _options.SecurityConstants ?? defaults.Security,
                    SecurityPolicy = _options.PolicyConstants ?? defaults.SecurityPolicy
                };
            }
        }
    }
}
