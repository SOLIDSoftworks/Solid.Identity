using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    /// <summary>
    /// Constants for SOAP serialization and deserialization.
    /// </summary>
    public abstract class SoapConstants
    {
        /// <summary>
        /// SOAP envelope namespace
        /// </summary>
        public abstract string EnvelopeNamespace { get; }

        /// <summary>
        /// XML schema namespace
        /// </summary>
        public string XmlSchemaNamespace { get; protected set; } = "http://www.w3.org/2001/XMLSchema";

        /// <summary>
        /// XML namespace
        /// </summary>
        public string XmlNamespace { get; protected set; } = "http://www.w3.org/XML/1998/namespace";

        /// <summary>
        /// Constants for SOAP 1.1
        /// </summary>
        public static SoapConstants Soap11 { get; } = new Soap11Constants();

        /// <summary>
        /// Constants for SOAP 1.2
        /// </summary>
        public static SoapConstants Soap12 { get; } = new Soap12Constants();
    }

    internal class Soap11Constants : SoapConstants
    {
        public override string EnvelopeNamespace => "http://schemas.xmlsoap.org/soap/envelope/";
    }

    internal class Soap12Constants : SoapConstants
    {
        public override string EnvelopeNamespace => "http://www.w3.org/2003/05/soap-envelope";
    }
}
