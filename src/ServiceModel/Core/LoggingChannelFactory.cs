using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Solid.ServiceModel
{
    /// <summary>
    /// A SOAP channel factory that enables logging.
    /// </summary>
    /// <typeparam name="T">The type of channel to be created.</typeparam>
    public class LoggingChannelFactory<T> : ChannelFactory<T>
    {
        /// <summary>
        /// A logger for this <see cref="LoggingChannelFactory{T}"/>.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Creates an <see cref="LoggingChannelFactory{T}"/> instance.
        /// </summary>
        /// <param name="binding">The <see cref="Binding"/> to use for the channel.</param>
        /// <param name="remoteAddress">The <see cref="EndpointAddress"/> for the channel.</param>
        /// <param name="loggerFactory">An optional <see cref="ILoggerFactory"/> for creating <see cref="ILogger"/>.</param>
        public LoggingChannelFactory(Binding binding, EndpointAddress remoteAddress, ILoggerFactory loggerFactory) 
        : base(EnableMessageTracing(binding, loggerFactory), remoteAddress)
        {
            Logger = loggerFactory?.CreateLogger(GetType().FullName);
            if (loggerFactory != null)
                Endpoint.Contract.ContractBehaviors.Add(new LoggingBehavior(loggerFactory));
        }

        private static Binding EnableMessageTracing(Binding binding, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) return binding;

            if (binding is CustomBinding custom)
                return AlterMessageEncoding(custom, loggerFactory);
            else
                return AlterMessageEncoding(new CustomBinding(binding), loggerFactory);
        }

        private static Binding AlterMessageEncoding(CustomBinding binding, ILoggerFactory loggerFactory)
        {
            var encoding = binding.Elements.Find<MessageEncodingBindingElement>();
            var trace = new TraceMessageEncodingBindingElement(encoding, loggerFactory);
            var index = binding.Elements.IndexOf(encoding);
            binding.Elements.RemoveAt(index);
            binding.Elements.Insert(index, trace);
            return binding;
        }
    }
}
