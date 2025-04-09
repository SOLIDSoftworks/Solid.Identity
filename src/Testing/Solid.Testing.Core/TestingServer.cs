using Solid.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Testing.Models;
using Solid.Testing.Abstractions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace Solid.Http
{
    /// <summary>
    /// A wrapper around an in-memory hosted service and a client to perform request on the in-memory hosted service.
    /// </summary>
    public class TestingServer : ISolidHttpClient, IDisposable
    {
        private IInMemoryHost _host;
        private ServiceProvider _root;
        private List<IServiceScope> _scopes;
        private ISolidHttpClient _client;

        internal TestingServer(IInMemoryHost host, ServiceProvider provider)
        {
            _scopes = new List<IServiceScope>();
            _host = host;
            _root = provider;
        }

        /// <summary>
        /// The base address of the testing server.
        /// </summary>
        public Uri BaseAddress => _host.BaseAddress;

        /// <summary>
        /// The <see cref="IServiceProvider"/> for the testing server.
        /// <para>This is NOT the <see cref="IServiceProvider"/> that the in-memory host uses internally.</para>
        /// </summary>
        public IServiceProvider Provider => CreateScope().ServiceProvider;

        /// <summary>
        /// The <see cref="ISolidHttpClient"/> that is paired with the <seealso cref="TestingServer"/>.
        /// </summary>
        [Obsolete("Will be removed in next major release. The TestingServer is now it's own client.")]
        public ISolidHttpClient Client => GetInnerClient();

        private ISolidHttpClient GetInnerClient()
        {
            if (_client == null)
            {
                _client = Provider.GetService<ISolidHttpClientFactory>().CreateWithBaseAddress(BaseAddress);
                _client.OnRequestCreated(request => request.OnHttpResponse(_ => _client = null));
            }
            return _client;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var scope in _scopes)
                scope.Dispose();
        }

        private IServiceScope CreateScope()
        {
            var scope = _root.CreateScope();
            _scopes.Add(scope);
            return scope;
        }

        Uri ISolidHttpClient.BaseAddress { get => this.BaseAddress; }

        ISolidHttpClient ISolidHttpClient.OnRequestCreated(Action<IServiceProvider, ISolidHttpRequest> handler)
            => GetInnerClient().OnRequestCreated(handler);

        ISolidHttpRequest ISolidHttpClient.PerformRequestAsync(HttpMethod method, Uri url, CancellationToken cancellationToken)
            => GetInnerClient()
                .PerformRequestAsync(method, url, cancellationToken);
    }
}
