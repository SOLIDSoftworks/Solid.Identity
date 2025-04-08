using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Solid.Extensions.AspNetCore.Soap.Channels
{
    /// <summary>
    /// A <see cref="Message" /> implementation which canconvey fault information.
    /// </summary>
    public abstract class FaultMessage : Message
    {
        /// <summary>
        /// The <see cref="MessageFault" /> of the message.
        /// </summary>
        protected MessageFault Fault { get; }

        /// <summary>
        /// The action that faulted.
        /// </summary>
        protected string Action { get; }

        /// <summary>
        /// Creates a fault message of the correct <see cref="MessageVersion" />.
        /// </summary>
        /// <param name="version">The <see cref="MessageVersion" /> to create the <seealso cref="FaultMessage" /> for.</param>
        /// <param name="fault">The <see cref="MessageFault" /> used to create the <seealso cref="FaultMessage" />.</param>
        /// <param name="action">The action that faulted.</param>
        /// <returns>A <see cref="FaultMessage" />.</returns>
        public static Message CreateFaultMessage(MessageVersion version, MessageFault fault, string action)
        {           
            if (version.Envelope == EnvelopeVersion.Soap12)
                return new Soap12FaultMessage(version, fault, action);
            if (version.Envelope == EnvelopeVersion.Soap11)
                return new Soap11FaultMessage(version, fault, action);

            throw new Exception("Message version has no fault schema");
        }

        private FaultMessage(MessageVersion version, MessageFault fault, string action)
        {
            Fault = fault;
            Action = action;

            Version = version;
            Headers = new MessageHeaders(version);
        }

        /// <summary>
        /// The <see cref="MessageHeaders" /> of the <seealso cref="FaultMessage" />.
        /// </summary>
        public override MessageHeaders Headers { get; }

        /// <summary>
        /// The <see cref="MessageProperties" /> of the <seealso cref="FaultMessage" />.
        /// </summary>
        public override MessageProperties Properties { get; } = new MessageProperties();

        /// <summary>
        /// The <see cref="MessageVersion" /> of the <seealso cref="FaultMessage" />.
        /// </summary>
        public override MessageVersion Version { get; }

        /// <summary>
        /// Says whether the message is a fault or not.
        /// <para>This is always true for <see cref="FaultMessage" />.</para>
        /// </summary>
        public override bool IsFault => true;

        class Soap11FaultMessage : FaultMessage
        {
            private SoapConstants _constants;

            public Soap11FaultMessage(MessageVersion version, MessageFault fault, string action)
                : base(version, fault, action)
            {
                _constants = SoapConstants.Soap11;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("Fault", _constants.EnvelopeNamespace);

                WriteFaultCodeElement(writer, Fault.Code);
                writer.WriteElementString("faultstring", Fault.Reason.GetMatchingTranslation().Text);
                writer.WriteElementString("faultactor", Fault.Actor);

                if (Fault.HasDetail)
                {
                    using (var reader = Fault.GetReaderAtDetailContents())
                    {
                        writer.WriteStartElement("detail");
                        writer.WriteElement(reader);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            private void WriteFaultCodeElement(XmlDictionaryWriter writer, FaultCode code)
            {
                var prefix = writer.LookupPrefix(_constants.EnvelopeNamespace);
                if (!string.IsNullOrEmpty(code.Namespace) && code.Namespace != _constants.EnvelopeNamespace)
                    prefix = writer.LookupPrefix(code.Namespace) ?? "custom";

                var name = code.Name;
                writer.WriteStartElement("faultcode");

                if (!code.IsPredefinedFault)
                {
                    if (prefix == "custom")
                        writer.WriteXmlnsAttribute(prefix, code.Namespace);
                }
                else
                {
                    if (code.IsReceiverFault) name = "Server";
                    if (code.IsSenderFault) name = "Client";
                }

                writer.WriteString($"{prefix}:{name}");
                writer.WriteEndElement();
            }
        }
        class Soap12FaultMessage : FaultMessage
        {
            private SoapConstants _constants;

            public Soap12FaultMessage(MessageVersion version, MessageFault fault, string action) : base(version, fault, action)
            {
                _constants = SoapConstants.Soap12;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("Fault", _constants.EnvelopeNamespace);

                WriteFaultCodeElement(writer, Fault.Code, "Code");
                //writer.WriteElementString("Value", Constants.Soap12EnvelopeNamespace, GetFaultCodeString(Fault.Code));

                var reason = Fault.Reason.GetMatchingTranslation();
                writer.WriteStartElement("Reason", _constants.EnvelopeNamespace);
                writer.WriteStartElement("Text", _constants.EnvelopeNamespace);
                writer.WriteAttributeString("xml", "lang", _constants.XmlNamespace, reason.XmlLang);
                writer.WriteString(reason.Text);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteElementString("Node", _constants.EnvelopeNamespace, Fault.Node);

                if (Fault.HasDetail)
                {
                    using (var reader = Fault.GetReaderAtDetailContents())
                    {
                        writer.WriteStartElement("Detail", _constants.EnvelopeNamespace);
                        writer.WriteElement(reader);
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            private void WriteFaultCodeElement(XmlDictionaryWriter writer, FaultCode code, string localName)
            {
                var prefix = writer.LookupPrefix(_constants.EnvelopeNamespace);
                if (!string.IsNullOrEmpty(code.Namespace) && code.Namespace != _constants.EnvelopeNamespace)
                    prefix = writer.LookupPrefix(code.Namespace) ?? "custom";

                var name = code.Name;
                writer.WriteStartElement(localName, _constants.EnvelopeNamespace);
                writer.WriteStartElement("Value", _constants.EnvelopeNamespace);

                if (!code.IsPredefinedFault)
                {
                    if (prefix == "custom")
                        writer.WriteXmlnsAttribute(prefix, code.Namespace);
                }
                else
                {
                    if (code.IsReceiverFault) name = "Receiver";
                    if (code.IsSenderFault) name = "Sender";
                }

                writer.WriteString($"{prefix}:{name}");
                writer.WriteEndElement();
                if (code.SubCode != null)
                    WriteFaultCodeElement(writer, code.SubCode, "Subcode");
                writer.WriteEndElement();
            }
        }
    }
}