using Solid.ServiceModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Extensions.ServiceModel
{
    /// <summary>
    /// A default implementation of <see cref="IProxyInitializer"/>.
    /// </summary>
    public class DefaultProxyInitializer : IProxyInitializer
    {
        private ILogger<DefaultProxyInitializer> _logger;
        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Creates an instance of <see cref="DefaultProxyInitializer"/>.
        /// </summary>
        /// <param name="loggerFactory">An optional instance of <see cref="ILoggerFactory"/> to create <see cref="ILogger"/> instances.</param>
        public DefaultProxyInitializer(ILoggerFactory loggerFactory = null)
        {
            _logger = loggerFactory?.CreateLogger<DefaultProxyInitializer>();
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public virtual ValueTask<TProxy> InitializeProxyAsync<TProxy>(ProxyOptions options, CancellationToken cancellationToken)
        {
            _logger?.LogDebug($"Initializing channel for endpoint: {options.Endpoint}");
            var binding = CreateBinding(options);
            var endpoint = new EndpointAddress(options.Endpoint);
            var factory = new IssuedTokenChannelFactory<TProxy>(binding, endpoint, loggerFactory: _loggerFactory);

            foreach (var behavior in options.ContractBehaviors)
                factory.Endpoint.Contract.ContractBehaviors.Add(behavior);

            var channel = factory.CreateChannel();
            return new ValueTask<TProxy>(channel);
        }

        /// <inheritdoc />
        public virtual ValueTask<TProxy> InitializeProxyAsync<TProxy>(ProxyOptions options, SecurityToken token, CancellationToken cancellationToken)
        {
            _logger?.LogDebug($"Initializing channel for endpoint: {options.Endpoint}");
            var binding = CreateBinding(options);
            var endpoint = new EndpointAddress(options.Endpoint);
            var factory = new IssuedTokenChannelFactory<TProxy>(binding, endpoint, loggerFactory: _loggerFactory);

            foreach (var behavior in options.ContractBehaviors)
                factory.Endpoint.Contract.ContractBehaviors.Add(behavior);

            factory.SecurityTokenHandlers.Clear();
            foreach (var handler in options.SecurityTokensHandlers)
                factory.SecurityTokenHandlers.Add(handler);

            var channel = factory.CreateChannelWithIssuedToken(token);
            return new ValueTask<TProxy>(channel);
        }

        /// <summary>
        /// Creates a <see cref="Binding"/> using the provided <paramref name="options"/>.
        /// </summary>
        /// <param name="options">An instance of <see cref="ProxyOptions"/>.</param>
        /// <returns>A <see cref="Binding"/> instance.</returns>
        protected virtual Binding CreateBinding(ProxyOptions options)
        {
            var binding = new IssuedTokenHttpBinding
            {
                KeyType = System.IdentityModel.Tokens.SecurityKeyType.BearerKey,
                MaxBufferPoolSize = options.MaxBufferPoolSize,
                MaxReceivedMessageSize = options.MaxReceivedMessageSize,

                SendTimeout = options.SendTimeout,
                ReceiveTimeout = options.ReceiveTimeout,
                CloseTimeout = options.CloseTimeout,
                OpenTimeout = options.OpenTimeout,

                ReaderQuotas =
                {
                    MaxArrayLength = options.ReaderQuotasMaxArrayLength,
                    MaxStringContentLength = options.ReaderQuotasMaxStringContentLength,
                    MaxDepth = options.ReaderQuotasMaxDepth
                }
            };
            return binding;
        }
    }
}
