using Solid.Extensions.AspNetCore.Soap;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.ServiceModel.Channels
{
    internal static class MessageVersionExtensions
    {
        public static FaultCode CreateFaultCode(this MessageVersion version, string code = null, FaultCode sub = null)
        {
            var envelope = version.Envelope;
            var ns = envelope == EnvelopeVersion.Soap11 ? SoapConstants.Soap11.EnvelopeNamespace : SoapConstants.Soap12.EnvelopeNamespace;
            if (string.IsNullOrWhiteSpace(code))
                code = envelope == EnvelopeVersion.Soap11 ? "Server" : "Receiver";
            if (sub != null)
                return new FaultCode(code, ns, sub);
            return new FaultCode(code, ns);
        }
        public static FaultCode CreateSenderFaultCode(this MessageVersion version, string code = null, FaultCode sub = null)
        {
            var envelope = version.Envelope;
            var ns = envelope == EnvelopeVersion.Soap11 ? SoapConstants.Soap11.EnvelopeNamespace : SoapConstants.Soap12.EnvelopeNamespace;
            if (string.IsNullOrWhiteSpace(code))
                code = envelope == EnvelopeVersion.Soap11 ? "Client" : "Sender";
            if (sub != null)
                return new FaultCode(code, ns, sub);
            return new FaultCode(code, ns);
        }

        public static FaultReason CreateFaultReason(this MessageVersion version, string reason = "There was a problem with the server so the message could not proceed.")
            => new FaultReason(reason);
    }
}
