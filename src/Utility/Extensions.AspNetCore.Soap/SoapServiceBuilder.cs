using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    /// <summary>
    /// A builder used for adding extensions to a SOAP service.
    /// </summary>
    public class SoapServiceBuilder
    {
        internal SoapServiceBuilder(Type contract, IServiceCollection services)
        {
            Contract = contract;
            Services = services;            
        }

        /// <summary>
        /// The contract of the SOAP service being built.
        /// </summary>
        public Type Contract { get; }

        /// <summary>
        /// The <see cref="IServiceCollection" /> of the application.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Configures options for the SOAP service.
        /// </summary>
        /// <param name="configureOptions">A delegate used to configure options for <see cref="Contract" />.</param>
        /// <returns>The <see cref="SoapServiceBuilder" /> instance so that additional calls can be chained.</returns>
        public SoapServiceBuilder Configure(Action<SoapServiceOptions> configureOptions)
        {
            Services.Configure(Contract.FullName, configureOptions);
            return this;
        }
    }
}
