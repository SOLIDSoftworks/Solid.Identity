using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Extensions.ServiceModel
{
    /// <summary>
    /// An interface that describes a class that initializes a proxy.
    /// </summary>
    public interface IProxyInitializer
    {
        /// <summary>
        /// Initilizes a proxy.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy to initialize.</typeparam>
        /// <param name="options">A <see cref="ProxyOptions"/> instance.</param>
        /// <param name="token">The <see cref="SecurityToken"/> to use to initialize the proxy.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> instance.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> of type <typeparamref name="TProxy"/>.</returns>
        ValueTask<TProxy> InitializeProxyAsync<TProxy>(ProxyOptions options, SecurityToken token, CancellationToken cancellationToken);

        /// <summary>
        /// Initilizes a proxy.
        /// </summary>
        /// <typeparam name="TProxy">The type of proxy to initialize.</typeparam>
        /// <param name="options">A <see cref="ProxyOptions"/> instance.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> instance.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> of type <typeparamref name="TProxy"/>.</returns>
        ValueTask<TProxy> InitializeProxyAsync<TProxy>(ProxyOptions options, CancellationToken cancellationToken);
    }
}
