using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    /// <summary>
    /// Options class for a SOAP service.
    /// </summary>
    public class SoapServiceOptions
    {
        /// <summary>
        /// The name of the SOAP service.
        /// <para>This is automatically generated from the SOAP contract.</para>
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The namespace of the SOAP service.
        /// <para>This is automatically generated from the SOAP contract.</para>
        /// </summary>
        public string Namespace { get; internal set; }

        /// <summary>
        /// The max size of headers for SOAP messages.
        /// </summary>
        public int MaxSizeOfHeaders { get; set; } = int.MaxValue;

        /// <summary>
        /// A flag which sets whether all incoming SOAP headers must be understood or not.
        /// </summary>
        public bool ValidateMustUnderstand { get; set; } = true;

        /// <summary>
        /// A flag to set whether exception information be sent in the response when the SOAP service faults.
        /// </summary>
        public bool IncludeExceptionDetailInFaults { get; set; } = false;

        // TODO: UnkownMessageREceived handler
    }
}
