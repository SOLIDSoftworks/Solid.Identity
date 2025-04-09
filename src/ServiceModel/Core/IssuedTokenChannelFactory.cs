using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.IdentityModel.Tokens.Xml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace Solid.ServiceModel
{
    /// <summary>
    /// A SOAP channel factory for creating channels using an issued <see cref="SecurityToken"/>.
    /// </summary>
    /// <typeparam name="T">The type of channel to be created.</typeparam>
    public class IssuedTokenChannelFactory<T> : LoggingChannelFactory<T>
    {
        /// <summary>
        /// Creates an <see cref="IssuedTokenChannelFactory{T}"/> instance.
        /// </summary>
        /// <param name="binding">The <see cref="Binding"/> to use for the channel.</param>
        /// <param name="remoteAddress">The <see cref="EndpointAddress"/> for the channel.</param>
        /// <param name="loggerFactory">An optional <see cref="ILoggerFactory"/> for creating <see cref="ILogger"/>.</param>
        public IssuedTokenChannelFactory(Binding binding, EndpointAddress remoteAddress, ILoggerFactory loggerFactory = null) 
            : base(binding, remoteAddress, loggerFactory)
        {
        }

        /// <summary>
        /// A list of <see cref="SecurityTokenHandler"/> that can be used to write a token to a SOAP request.
        /// <para>Attention: Careful with some <see cref="SecurityTokenHandler"/>s. They might not write the signature if the <see cref="SecurityToken"/> doesn't include the <see cref="SecurityToken.SigningKey"/>.</para>
        /// </summary>
        public ICollection<SecurityTokenHandler> SecurityTokenHandlers { get; } = new List<SecurityTokenHandler>
        {
        };

        /// <summary>
        /// Creates a channel using the provided <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The <see cref="SecurityToken"/> that is used in the WS-Security header of each SOAP request.</param>
        /// <param name="address">The <see cref="EndpointAddress"/> that provides the location of the service.</param>
        /// <param name="via">The <see cref="Uri"/> that contains the transport address to which the channel sends messages.</param>
        /// <returns>A channel of type <typeparamref name="T"/>.</returns>
        public T CreateChannelWithIssuedToken(SecurityToken token, EndpointAddress address, Uri via)
        {
            LogMessages.CreatingChannelWithIssuedToken(Logger, typeof(T));
            var contract = CreateChannel(address, via);
            AddSecurityToken(contract, token);
            return contract;
        }


        /// <summary>
        /// Creates a channel using the provided <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The <see cref="SecurityToken"/> that is used in the WS-Security header of each SOAP request.</param>
        /// <param name="address">The <see cref="EndpointAddress"/> that provides the location of the service.</param>
        /// <returns>A channel of type <typeparamref name="T"/>.</returns>
        public T CreateChannelWithIssuedToken(SecurityToken token, EndpointAddress address)
        {
            LogMessages.CreatingChannelWithIssuedToken(Logger, typeof(T));
            var contract = CreateChannel(address);
            AddSecurityToken(contract, token);
            return contract;
        }


        /// <summary>
        /// Creates a channel using the provided <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The <see cref="SecurityToken"/> that is used in the WS-Security header of each SOAP request.</param>
        /// <returns>A channel of type <typeparamref name="T"/>.</returns>
        public T CreateChannelWithIssuedToken(SecurityToken token)
        {
            LogMessages.CreatingChannelWithIssuedToken(Logger, typeof(T));
            var contract = CreateChannel();
            AddSecurityToken(contract, token);
            return contract;
        }

        /// <summary>
        /// Gets an array of supported <see cref="SecurityTokenHandler"/>s with the addition of a generic xml security token handler.
        /// </summary>
        /// <returns>An array of supported <see cref="SecurityTokenHandler"/>s.</returns>
        protected SecurityTokenHandler[] GetSupportedSecurityTokenHandlers() => new[] { new GenericXmlSecurityTokenHandler() }.Concat(SecurityTokenHandlers).ToArray();
        
        private void AddSecurityToken(T contract, SecurityToken token)
        {
            var channel = contract as IChannel;
            var channelParameters = channel.GetProperty<ChannelParameterCollection>();
            var credentials = new ExtendedClientCredentials(null);
            credentials.IssuedToken.Token = token;
            credentials.IssuedToken.Handlers = GetSupportedSecurityTokenHandlers();
            channelParameters.Add(credentials);
        }
    }
}
