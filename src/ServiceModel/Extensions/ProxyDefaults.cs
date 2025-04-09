using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace Solid.Extensions.ServiceModel
{
    /// <summary>
    /// Default values for <see cref="ProxyOptions"/>.
    /// </summary>
    public static class ProxyDefaults
    {
        /// <summary>
        /// The default max received message size. 
        /// <para>Used for <see cref="WSHttpBindingBase.MaxReceivedMessageSize"/>.</para>
        /// </summary>
        public const long MaxReceivedMessageSize = 10_000_000;

        /// <summary>
        /// The default max buffer pool size. 
        /// <para>Used for <see cref="WSHttpBindingBase.MaxBufferPoolSize"/>.</para>
        /// </summary>
        public const long MaxBufferPoolSize = 20_000_000;

        /// <summary>
        /// The default reader quota max depth. 
        /// <para>Used for <see cref="XmlDictionaryReaderQuotas.MaxDepth"/> property of <see cref="WSHttpBindingBase.ReaderQuotas"/>.</para>
        /// </summary>
        public const int ReaderQuotasMaxDepth = 32;

        /// <summary>
        /// The default reader quota max array length. 
        /// <para>Used for <see cref="XmlDictionaryReaderQuotas.MaxArrayLength"/> property of <see cref="WSHttpBindingBase.ReaderQuotas"/>.</para>
        /// </summary>
        public const int ReaderQuotasMaxArrayLength = 400_000;

        /// <summary>
        /// The default reader quota max string content length. 
        /// <para>Used for <see cref="XmlDictionaryReaderQuotas.MaxStringContentLength"/> property of <see cref="WSHttpBindingBase.ReaderQuotas"/>.</para>
        /// </summary>
        public const int ReaderQuotasMaxStringContentLength = 200_000;
        
        /// <summary>
        /// The default timeout for opening a connection.
        /// <para>Used for <see cref="Binding.OpenTimeout"/>.</para>
        /// </summary>
        public static readonly TimeSpan OpenTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default timeout for closing a connection.
        /// <para>Used for <see cref="Binding.CloseTimeout"/>.</para>
        /// </summary>
        public static readonly TimeSpan CloseTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default timeout for receiving data from a connection.
        /// <para>Used for <see cref="Binding.ReceiveTimeout"/>.</para>
        /// </summary>
        public static readonly TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default timeout for sending data over a connection.
        /// <para>Used for <see cref="Binding.SendTimeout"/>.</para>
        /// </summary>
        public static readonly TimeSpan SendTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default <see cref="SecurityTokenHandler"/>s supported.
        /// </summary>
        public static ICollection<SecurityTokenHandler> SecurityTokensHandlers => new List<SecurityTokenHandler>
        {
            new SamlSecurityTokenHandler(),
            new Saml2SecurityTokenHandler()
        };

        /// <summary>
        /// The default <see cref="IContractBehavior"/>s.
        /// </summary>
        public static ICollection<IContractBehavior> ContractBehaviors => new List<IContractBehavior>();
    }
}
