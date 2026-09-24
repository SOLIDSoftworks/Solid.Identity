using System.Xml;
using Solid.IdentityModel.Protocols.WsAddressing;
using Microsoft.IdentityModel.Xml;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsPolicy
{
    /// <summary>
    /// Base class for support of serializing versions of WS-Policy.
    /// </summary>
    internal class WsPolicySerializer
    {
        private WsAddressingSerializer _wsAddressingSerializer = new WsAddressingSerializer();

        public WsPolicySerializer()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader">The xml dictionary reader.</param>
        /// <param name="namespace"></param>
        /// <returns>An <see cref="EndpointReference"/> instance.</returns>
        public virtual AppliesTo ReadAppliesTo(XmlDictionaryReader reader, string @namespace)
        {
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return new AppliesTo();
            }

            reader.ReadStartElement();
            var appliesTo = new AppliesTo(_wsAddressingSerializer.ReadEndpointReference(reader));
            reader.ReadEndElement();

            return appliesTo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="namespace"></param>
        public virtual PolicyReference ReadPolicyReference(XmlDictionaryReader reader, string @namespace)
        {
            //  if this clas becomes public, we will need to check parameters
            //  XmlUtil.CheckReaderOnEntry(reader, WsPolicyElements.PolicyReference, @namespace);

            bool isEmptyElement = reader.IsEmptyElement;
            var attributes = XmlAttributeDescriptor.ReadAttributes(reader);
            var uri = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityPolicyAttributes.URI, @namespace);
            var digest = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityPolicyAttributes.Digest, @namespace);
            var digestAlgorithm = XmlAttributeDescriptor.GetAttribute(attributes, WsSecurityPolicyAttributes.DigestAlgorithm, @namespace);
            reader.ReadStartElement();
            reader.MoveToContent();

            if (!isEmptyElement)
                reader.ReadEndElement();

            return new PolicyReference(uri, digest, digestAlgorithm);
        }

        public static void WriteAppliesTo(XmlDictionaryWriter writer, WsSerializationContext serializationContext, AppliesTo appliesTo)
        {
            //  if this clas becomes public, we will need to check parameters
            //  WsUtils.ValidateParamsForWriting(writer, serializationContext, appliesTo, nameof(appliesTo));

            writer.WriteStartElement(serializationContext.SecurityPolicy.DefaultPrefix, WsSecurityPolicyElements.AppliesTo, serializationContext.SecurityPolicy.Namespace);
            if (appliesTo.EndpointReference != null)
                WsAddressingSerializer.WriteEndpointReference(writer, serializationContext, appliesTo.EndpointReference);

            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serializationContext"></param>
        /// <param name="policyReference"></param>
        public static void WritePolicyReference(XmlDictionaryWriter writer, WsSerializationContext serializationContext, PolicyReference policyReference)
        {
            //  if this clas becomes public, we will need to check parameters
            //  WsUtils.ValidateParamsForWritting(writer, serializationContext, policyReference, nameof(policyReference));

            writer.WriteStartElement(serializationContext.SecurityPolicy.DefaultPrefix, WsSecurityPolicyElements.PolicyReference, serializationContext.SecurityPolicy.Namespace);
            if (!string.IsNullOrEmpty(policyReference.Uri))
                writer.WriteAttributeString(WsSecurityPolicyAttributes.URI, policyReference.Uri);

            if (!string.IsNullOrEmpty(policyReference.Digest))
                writer.WriteAttributeString(WsSecurityPolicyAttributes.Digest, policyReference.Digest);

            if (!string.IsNullOrEmpty(policyReference.DigestAlgorithm))
                writer.WriteAttributeString(WsSecurityPolicyAttributes.DigestAlgorithm, policyReference.DigestAlgorithm);

            writer.WriteEndElement();
        }
    }
}
