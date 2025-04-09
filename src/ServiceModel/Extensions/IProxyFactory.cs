using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Extensions.ServiceModel
{
    /// <summary>
    /// An insterface that describes a factory to create proxies.
    /// </summary>
    public interface IProxyFactory
    {
        /// <summary>
        /// Creates a proxy of type <typeparamref name="TProxy"/>.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy to create.</typeparam>
        /// <param name="token">
        /// A security token for this proxy. 
        /// <para>If a security token is not provided, <see cref="ISoapSecurityTokenProvider.GetSecurityTokenAsync"/> will be called to attempt to get a security token.</para>
        /// </param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance.</param>
        /// <returns>An awaitable <see cref="ValueTask{TResult}"/> og type <typeparamref name="TProxy"/>.</returns>
        ValueTask<TProxy> CreateProxyAsync<TProxy>(string token = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a proxy of type <typeparamref name="TProxy"/>.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy to create.</typeparam>
        /// <param name="token">
        /// A <see cref="SecurityToken"/> for this proxy. 
        /// <para>If a <see cref="SecurityToken"/> is not provided, <see cref="ISoapSecurityTokenProvider.GetSecurityTokenAsync"/> will be called to attempt to get a <see cref="SecurityToken"/>.</para>
        /// </param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance.</param>
        /// <returns>An awaitable <see cref="ValueTask{TResult}"/> og type <typeparamref name="TProxy"/>.</returns>
        ValueTask<TProxy> CreateProxyAsync<TProxy>(SecurityToken token, CancellationToken cancellationToken = default);
    }
}
