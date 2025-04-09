using Solid.ServiceModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Solid.Extensions.ServiceModel
{
    /// <summary>
    /// Options for a service proxy.
    /// </summary>
    public class ProxyOptions
    {
        /// <summary>
        /// The endpoint of the service proxy.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The max received message size. 
        /// <para>Used for <see cref="WSHttpBindingBase.MaxReceivedMessageSize"/>.</para>
        /// </summary>
        public long MaxReceivedMessageSize { get; set; } = ProxyDefaults.MaxReceivedMessageSize;

        /// <summary>
        /// The max buffer pool size. 
        /// <para>Used for <see cref="WSHttpBindingBase.MaxBufferPoolSize"/>.</para>
        /// </summary>
        public long MaxBufferPoolSize { get; set; } = ProxyDefaults.MaxBufferPoolSize;

        /// <summary>
        /// The reader quota max depth. 
        /// <para>Used for <see cref="XmlDictionaryReaderQuotas.MaxDepth"/> property of <see cref="WSHttpBindingBase.ReaderQuotas"/>.</para>
        /// </summary>
        public int ReaderQuotasMaxDepth { get; set; } = ProxyDefaults.ReaderQuotasMaxDepth;

        /// <summary>
        /// The reader quota max array length. 
        /// <para>Used for <see cref="XmlDictionaryReaderQuotas.MaxArrayLength"/> property of <see cref="WSHttpBindingBase.ReaderQuotas"/>.</para>
        /// </summary>
        public int ReaderQuotasMaxArrayLength { get; set; } = ProxyDefaults.ReaderQuotasMaxArrayLength;

        /// <summary>
        /// The reader quota max string content length. 
        /// <para>Used for <see cref="XmlDictionaryReaderQuotas.MaxStringContentLength"/> property of <see cref="WSHttpBindingBase.ReaderQuotas"/>.</para>
        /// </summary>
        public int ReaderQuotasMaxStringContentLength { get; set; } = ProxyDefaults.ReaderQuotasMaxStringContentLength;

        /// <summary>
        /// The timeout for opening a connection.
        /// <para>Used for <see cref="Binding.OpenTimeout"/>.</para>
        /// </summary>
        public TimeSpan OpenTimeout { get; set; } = ProxyDefaults.OpenTimeout;

        /// <summary>
        /// The timeout for closing a connection.
        /// <para>Used for <see cref="Binding.CloseTimeout"/>.</para>
        /// </summary>
        public TimeSpan CloseTimeout { get; set; } = ProxyDefaults.CloseTimeout;

        /// <summary>
        /// The timeout for receiving data from a connection.
        /// <para>Used for <see cref="Binding.ReceiveTimeout"/>.</para>
        /// </summary>
        public TimeSpan ReceiveTimeout { get; set; } = ProxyDefaults.ReceiveTimeout;

        /// <summary>
        /// The timeout for sending data over a connection.
        /// <para>Used for <see cref="Binding.SendTimeout"/>.</para>
        /// </summary>
        public TimeSpan SendTimeout { get; set; } = ProxyDefaults.SendTimeout;

        /// <summary>
        /// The <see cref="SecurityTokenHandler"/>s supported by this proxy.
        /// </summary>
        public ICollection<SecurityTokenHandler> SecurityTokensHandlers { get; } = ProxyDefaults.SecurityTokensHandlers;

        /// <summary>
        /// The <see cref="IContractBehavior"/>s used by this proxy.
        /// </summary>
        public ICollection<IContractBehavior> ContractBehaviors { get; } = ProxyDefaults.ContractBehaviors;

        /// <summary>
        /// The type used to initialize this proxy.
        /// </summary>
        public Type ProxyInitializerType { get; internal set; }
    }
}
