using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Extensions.ServiceModel
{
    internal class ProxyFactory : IProxyFactory, IDisposable
    {
        private ConcurrentDictionary<string, IDisposable> _disposables = new ConcurrentDictionary<string, IDisposable>();
        private IServiceProvider _services;
        private IOptionsSnapshot<ProxyOptions> _optionsSnapshop;
        private ISoapSecurityTokenProvider _securityTokenProvider;

        public ProxyFactory(
            IServiceProvider services, 
            IOptionsSnapshot<ProxyOptions> optionsSnapshot,
            ISoapSecurityTokenProvider securityTokenProvider = null
        )
        {
            _services = services;
            _optionsSnapshop = optionsSnapshot;
            _securityTokenProvider = securityTokenProvider;
        }

        public async ValueTask<TProxy> CreateProxyAsync<TProxy>(string token = null, CancellationToken cancellationToken = default)
        {
            if (token == null)
                token = await GetSecurtityTokenAsync();
            var key = KeyFactory.CreateKey<TProxy>();
            var options = _optionsSnapshop.Get(key);

            if (token == null)
                return await CreateProxyAsync<TProxy>(key, options, null, cancellationToken);

            var securityToken = ReadSecurityToken(token, options);
            return await CreateProxyAsync<TProxy>(key, options, securityToken, cancellationToken);
        }

        public async ValueTask<TProxy> CreateProxyAsync<TProxy>(SecurityToken token, CancellationToken cancellationToken = default)
        {
            var key = KeyFactory.CreateKey<TProxy>();
            var options = _optionsSnapshop.Get(key);
            if (token == null)
                token = ReadSecurityToken(await GetSecurtityTokenAsync(), options);
            return await CreateProxyAsync<TProxy>(key, options, token, cancellationToken);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables.Select(p => p.Value))
                disposable.Dispose();
        }

        private SecurityToken ReadSecurityToken(string token, ProxyOptions options)
        {
            foreach (var handler in options.SecurityTokensHandlers)
            {
                if (handler.CanReadToken(token))
                    return handler.ReadToken(token);
            }
            throw new ArgumentException("Cannot read token", nameof(token));
        }

        private async ValueTask<TProxy> CreateProxyAsync<TProxy>(string key, ProxyOptions options, SecurityToken token, CancellationToken cancellationToken)
        {
            var initializer = _services.GetService(options.ProxyInitializerType) as IProxyInitializer;
            var proxy = default(TProxy);
            if (token == null)
                proxy = await initializer.InitializeProxyAsync<TProxy>(options, cancellationToken);
            else 
                proxy = await initializer.InitializeProxyAsync<TProxy>(options, token, cancellationToken);
            RegisterForDispose(key, proxy);
            return proxy;
        }

        private void RegisterForDispose(string key, object proxy)
        {
            if (proxy is IChannel channel)
            {
                channel.Closed += (sender, args) => RemoveDisposable(key);
                channel.Faulted += (sender, args) => RemoveDisposable(key);
            }
            if (proxy is IDisposable disposable)
                _disposables.TryAdd(key, disposable);
        }

        private void RemoveDisposable(string key) => _disposables.TryRemove(key, out _);

        private async ValueTask<string> GetSecurtityTokenAsync()
        {
            if (_securityTokenProvider == null)
                throw new InvalidOperationException($"No implementation of {nameof(ISoapSecurityTokenProvider)} found.");
            var token = await _securityTokenProvider.GetSecurityTokenAsync();
            if (token == null)
                throw new SecurityException("Could not get a security token.");
            return token;
        }
    }
}
