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

            protected override WsSerializationContext CreateSerializationContext(WsTrustVersion version)
            {
                if(_options == null)
                    return base.CreateSerializationContext(version);
                
                return new WsSerializationContext
                {
                    TrustVersion = version,
                    TrustActions = _options.TrustActions,
                    TrustConstants = _options.TrustConstants,
                    TrustKeyTypes = _options.TrustKeyTypes,
                    AddressingConstants = _options.AddressingConstants,
                    FedConstants = _options.FedConstants,
                    SecurityConstants = _options.SecurityConstants,
                    PolicyConstants = _options.PolicyConstants
                };
            }
        }
    }
}
