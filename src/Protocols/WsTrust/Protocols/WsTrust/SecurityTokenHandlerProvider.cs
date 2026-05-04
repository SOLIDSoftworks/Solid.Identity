using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.WsSecurity.Abstractions;
using Solid.Identity.Protocols.WsTrust;
using Solid.Identity.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Solid.Identity.Protocols.WsTrust
{
    public class SecurityTokenHandlerProvider : IDisposable
    {
        private readonly IDisposable _optionsChangeToken;
        private IReadOnlyDictionary<Type, SecurityTokenHandlerDescriptor> _descriptorsByType;
        private IReadOnlyDictionary<string, SecurityTokenHandlerDescriptor> _descriptorsByTokenTypeIdentifier;
        private readonly IServiceProvider _services;

        public SecurityTokenHandlerProvider(IServiceProvider services, IOptionsMonitor<WsTrustOptions> monitor)
        {
            _services = services;
            _optionsChangeToken = monitor.OnChange((options, _) => UpdateSecurityTokenHandlerDescriptors(options));
            UpdateSecurityTokenHandlerDescriptors(monitor.CurrentValue);
        }

        public IEnumerable<SecurityTokenHandler> GetAllSecurityTokenHandlers()
            => _descriptorsByType.Values.Select(d => d.Factory(_services));

        public SecurityTokenHandler GetSecurityTokenHandler(SecurityToken token)
            => GetSecurityTokenHandler(token?.GetType());

        public SecurityTokenHandler GetSecurityTokenHandler(Type tokenType)
            => _descriptorsByType.TryGetValue(tokenType, out var descriptor) ? descriptor.Factory(_services) : null;

        public SecurityTokenHandler GetSecurityTokenHandler(string tokenTypeIdentifier)
            => _descriptorsByTokenTypeIdentifier.TryGetValue(tokenTypeIdentifier, out var descriptor) ? descriptor.Factory(_services) : null;

        private void UpdateSecurityTokenHandlerDescriptors(WsTrustOptions options)
        {
            var handlersByTokenTypeIdentifier = new Dictionary<string, SecurityTokenHandlerDescriptor>();
            var handlersByType = new Dictionary<Type, SecurityTokenHandlerDescriptor>();
            foreach(var descriptor in options.SecurityTokenHandlers)
            {
                var handler = descriptor.Factory(_services);
                if (handler is not AsyncSecurityTokenHandler)
                {
                    // TODO: Remove wrapper if/when our PR for CanWriteSecurityToken default implementation gets accepted and released.
                    // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/pull/1438
                    handler = new SecurityTokenHandlerWrapper(handler);
                }
                handlersByType.Add(handler.TokenType, descriptor);
                foreach (var identifier in descriptor.TokenTypeIdentifiers)
                    handlersByTokenTypeIdentifier.Add(identifier, descriptor);
            }

            _descriptorsByType = new ReadOnlyDictionary<Type, SecurityTokenHandlerDescriptor>(handlersByType);
            _descriptorsByTokenTypeIdentifier = new ReadOnlyDictionary<string, SecurityTokenHandlerDescriptor>(handlersByTokenTypeIdentifier);
        }

        public void Dispose() => _optionsChangeToken.Dispose();
    }
}
