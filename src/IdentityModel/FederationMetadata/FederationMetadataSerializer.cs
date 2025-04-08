using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.FederationMetadata.WsAddressing;
using Solid.IdentityModel.FederationMetadata.WsAuthorization;
using Solid.IdentityModel.Tokens.Saml2.Metadata;
using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using static Solid.IdentityModel.FederationMetadata.WsFederationConstants;

namespace Solid.IdentityModel.FederationMetadata
{
    public class FederationMetadataSerializer : Saml2MetadataSerializer
    {
        protected WsAuthorizationSerializer WsAuthorizationSerializer { get; }
        protected WsAddressingSerializer WsAddressingSerializer { get; }

        public FederationMetadataSerializer()
            : this(new WsAuthorizationSerializer(), new WsAddressingSerializer(), new Saml2Serializer(), new DSigSerializer())
        {
        }

        public FederationMetadataSerializer(WsAuthorizationSerializer wsAuthorizationSerializer, WsAddressingSerializer wsAddressingSerializer, Saml2Serializer saml2Serializer, DSigSerializer dSigSerializer) 
            : base(saml2Serializer, dSigSerializer)
        {
            WsAuthorizationSerializer = wsAuthorizationSerializer;
            WsAddressingSerializer = wsAddressingSerializer;
        }

        protected override void WriteSaml2MetadataAttributes(XmlDictionaryWriter writer, Saml2Metadata metadata)
        {
            base.WriteSaml2MetadataAttributes(writer, metadata);
            writer.WriteXmlnsAttribute(Prefix, Namespace);
        }

        protected override bool TryReadRoleDescriptor(XmlDictionaryReader reader, out RoleDescriptor role)
        {
            if (!reader.IsStartElement(Saml2MetadataConstants.Elements.RoleDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out role);

            var d = null as RoleDescriptor;
            if(reader.TryReadFederationEndpointType(out var type))
            {
                if (type == FederationEndpointType.ApplicationService)
                    d = new ApplicationServiceDescriptor();
                if (type == FederationEndpointType.AttributeService)
                    d = new AttributeServiceDescriptor();
                if (type == FederationEndpointType.PseudonymService)
                    d = new PseudonymServiceDescriptor();
                if (type == FederationEndpointType.SecurityTokenService)
                    d = new SecurityTokenServiceDescriptor();
            }
            if(d == null)
                d = new RoleDescriptor();
            ReadRoleDescriptorAttributes(reader, d);

            reader.ForEachChild(r => TryReadRoleDescriptorChild(r, d), out var signature);
            d.Signature = signature;

            role = d;
            return true;
        }

        protected override bool TryReadRoleDescriptorChild(XmlDictionaryReader reader, RoleDescriptor role)
        {
            if (base.TryReadRoleDescriptorChild(reader, role))
                return true;

            if (role is AttributeServiceDescriptor attributeServiceDescriptor)
                return TryReadAttributeServiceDescriptorChild(reader, attributeServiceDescriptor);

            if (role is ApplicationServiceDescriptor applicationServiceDescriptor)
                return TryReadApplicationServiceDescriptorChild(reader, applicationServiceDescriptor);

            if (role is PseudonymServiceDescriptor pseudonymServiceDescriptor)
                return TryReadPseudonymServiceDescriptorChild(reader, pseudonymServiceDescriptor);

            if (role is SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
                return TryReadSecurityTokenServiceDescriptorChild(reader, securityTokenServiceDescriptor);

            return false;
        }

        protected virtual bool TryReadPseudonymServiceDescriptorChild(XmlDictionaryReader reader, PseudonymServiceDescriptor pseudonymServiceDescriptor)
        {
            if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.SingleSignOutNotificationEndpoint, Namespace, out var singleSignOutNotificationEndpoint))
                pseudonymServiceDescriptor.SingleSignOutNotificationEndpoint.Add(singleSignOutNotificationEndpoint);
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.PseudonymServiceEndpoint, Namespace, out var pseudonymServiceEndpoint))
                pseudonymServiceDescriptor.PseudonymServiceEndpoint.Add(pseudonymServiceEndpoint);
            else
                return TryReadWebServiceDescriptorChild(reader, pseudonymServiceDescriptor);

            return true;
        }

        protected virtual bool TryReadAttributeServiceDescriptorChild(XmlDictionaryReader reader, AttributeServiceDescriptor attributeServiceDescriptor)
        {
            if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.SingleSignOutNotificationEndpoint, Namespace, out var singleSignOutNotificationEndpoint))
                attributeServiceDescriptor.SingleSignOutNotificationEndpoint.Add(singleSignOutNotificationEndpoint);
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.AttributeServiceEndpoint, Namespace, out var attributeServiceEndpoint))
                attributeServiceDescriptor.AttributeServiceEndpoint.Add(attributeServiceEndpoint);
            else
                return TryReadWebServiceDescriptorChild(reader, attributeServiceDescriptor);

            return true;
        }

        protected virtual bool TryReadSecurityTokenServiceDescriptorChild(XmlDictionaryReader reader, SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
        {
            if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.SecurityTokenServiceEndpoint, Namespace, out var securityTokenServiceEndpoint))
                securityTokenServiceDescriptor.SecurityTokenServiceEndpoint.Add(securityTokenServiceEndpoint);
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.SingleSignOutSubscriptionEndpoint, Namespace, out var singleSignOutSubscriptionEndpoint))
                securityTokenServiceDescriptor.SingleSignOutSubscriptionEndpoint.Add(singleSignOutSubscriptionEndpoint);
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.SingleSignOutNotificationEndpoint, Namespace, out var singleSignOutNotificationEndpoint))
                securityTokenServiceDescriptor.SingleSignOutNotificationEndpoint.Add(singleSignOutNotificationEndpoint);
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.PassiveRequestorEndpoint, Namespace, out var passiveRequestorNotificationEndpoint))
                securityTokenServiceDescriptor.PassiveRequestorEndpoint.Add(passiveRequestorNotificationEndpoint);
            else
                return TryReadWebServiceDescriptorChild(reader, securityTokenServiceDescriptor);

            return true;
        }

        protected virtual bool TryReadApplicationServiceDescriptorChild(XmlDictionaryReader reader, ApplicationServiceDescriptor applicationServiceDescriptor)
        {
            if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.SingleSignOutNotificationEndpoint, Namespace, out var singleSignOutNotificationEndpoint))
                applicationServiceDescriptor.SingleSignOutNotificationEndpoint.Add(singleSignOutNotificationEndpoint);
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.ApplicationServiceEndpoint, Namespace, out var applicationServiceEndpoint))
                applicationServiceDescriptor.ApplicationServiceEndpoint.Add(applicationServiceEndpoint);
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.PassiveRequestorEndpoint, Namespace, out var passiveRequestorNotificationEndpoint))
                applicationServiceDescriptor.PassiveRequestorEndpoint.Add(passiveRequestorNotificationEndpoint);
            else
                return TryReadWebServiceDescriptorChild(reader, applicationServiceDescriptor);

            return true;
        }

        protected virtual bool TryReadWebServiceDescriptorChild(XmlDictionaryReader reader, WebServiceDescriptor webServiceDescriptor)
        {
            if (TryReadLogicalServiceNamesOffered(reader, out var logicalServiceNamesOffered))
                webServiceDescriptor.IssuerNames.AddRange(logicalServiceNamesOffered);
            else if (TryReadTokenTypesOffered(reader, out var tokenTypesOffered))
                webServiceDescriptor.TokenTypesOffered.AddRange(tokenTypesOffered);
            else if (TryReadClaimDialectsOffered(reader, out var claimDialectsOffered))
                webServiceDescriptor.ClaimDialectsOffered.AddRange(claimDialectsOffered);
            else if (TryReadClaimTypes(reader, Elements.ClaimTypesOffered, Namespace, out var claimTypesOffered))
                webServiceDescriptor.ClaimTypesOffered.AddRange(claimTypesOffered);
            else if (TryReadClaimTypes(reader, Elements.ClaimTypesRequested, Namespace, out var claimTypesRequested))
                webServiceDescriptor.ClaimTypesRequested.AddRange(claimTypesRequested);
            else if (TryReadAutomaticPseudonyms(reader, out var automaticPseudonyms))
                webServiceDescriptor.AutomaticPseudonyms = automaticPseudonyms;
            else if (WsAddressingSerializer.TryReadEndpointReferenceCollection(reader, Elements.TargetScopes, Namespace, out var targetScopes))
                webServiceDescriptor.TargetScopes.AddRange(targetScopes);
            else
                return base.TryReadRoleDescriptorChild(reader, webServiceDescriptor);

            return true;
        }

        protected virtual bool TryReadAutomaticPseudonyms(XmlDictionaryReader reader, out bool? automaticPseudonyms)
        {
            if (!reader.TryReadElementContent(Elements.AutomaticPseudonyms, Namespace, out var content)) return Out.False(out automaticPseudonyms);

            if (!bool.TryParse(content, out var parsed))
                // The element has been read, but the contents of the element wasn't a valid boolean value. Default to false.
                return Out.True(false, out automaticPseudonyms);

            automaticPseudonyms = parsed;
            return true;
        }

        protected virtual bool TryReadClaimDialectsOffered(XmlDictionaryReader reader, out List<Uri> claimDialectsOffered)
        {
            if (!reader.IsStartElement(Elements.ClaimDialectsOffered, Namespace)) return Out.False(out claimDialectsOffered);
            var list = new List<Uri>();

            reader.ForEachChild(r =>
            {
                if (TryReadClaimDialect(reader, out var claimDialect))
                {
                    list.Add(claimDialect);
                    return true;
                }
                return false;
            });
            claimDialectsOffered = list;
            return true;
        }

        protected virtual bool TryReadClaimDialect(XmlDictionaryReader reader, out Uri claimDialect)
        {
            if (!reader.IsStartElement(Elements.ClaimDialect, Namespace)) return Out.False(out claimDialect);

            _ = reader.TryReadAttributeAsUri(Attributes.Uri, out var uri);
            _ = reader.ReadOuterXml();
            claimDialect = uri;
            return true;
        }

        protected virtual bool TryReadClaimTypes(XmlDictionaryReader reader, string name, string ns, out List<ClaimType> claimTypes)
        {
            if (!reader.IsStartElement(name, ns)) return Out.False(out claimTypes);
            var list = new List<ClaimType>();

            reader.ForEachChild(r =>
            {
                if (WsAuthorizationSerializer.TryReadClaimType(reader, out var claimType))
                {
                    list.Add(claimType);
                    return true;
                }
                return false;
            });
            claimTypes = list;
            return true;
        }

        protected virtual bool TryReadLogicalServiceNamesOffered(XmlDictionaryReader reader, out List<Uri> logicalServiceNamesOffered)
        {
            if (!reader.IsStartElement(Elements.LogicalServiceNamesOffered, Namespace)) return Out.False(out logicalServiceNamesOffered);
            var list = new List<Uri>();

            reader.ForEachChild(r =>
            {
                if (TryReadIssuerName(reader, out var issuerName))
                {
                    list.Add(issuerName);
                    return true;
                }
                return false;
            });
            logicalServiceNamesOffered = list;
            return true;
        }

        protected virtual bool TryReadIssuerName(XmlDictionaryReader reader, out Uri issuerName)
        {
            if (!reader.IsStartElement(Elements.IssuerName, Namespace)) return Out.False(out issuerName);

            _ = reader.TryReadAttributeAsUri(Attributes.Uri, out var uri);
            _ = reader.ReadOuterXml();
            issuerName = uri;
            return true;
        }

        protected virtual bool TryReadTokenType(XmlDictionaryReader reader, out Uri tokenType)
        {
            if (!reader.IsStartElement(Elements.TokenType, Namespace)) return Out.False(out tokenType);

            _ = reader.TryReadAttributeAsUri(Attributes.Uri, out var uri);
            _ = reader.ReadOuterXml();
            tokenType = uri;
            return true;
        }

        protected virtual bool TryReadTokenTypesOffered(XmlDictionaryReader reader, out List<Uri> tokenTypes)
        {
            if (!reader.IsStartElement(Elements.TokenTypesOffered, Namespace)) return Out.False(out tokenTypes);
            var list = new List<Uri>();

            reader.ForEachChild(r =>
            {
                if (TryReadTokenType(reader, out var tokenType))
                {
                    list.Add(tokenType);
                    return true;
                }
                return false;
            });
            tokenTypes = list;
            return true;
        }

        protected override void ReadRoleDescriptorAttributes(XmlDictionaryReader reader, RoleDescriptor role)
        {
            if (role is AttributeServiceDescriptor attributeServiceDescriptor)
                ReadAttributeServiceDescriptorAttributes(reader, attributeServiceDescriptor);

            if (role is ApplicationServiceDescriptor applicationServiceDescriptor)
                ReadApplicationServiceDescriptorAttributes(reader, applicationServiceDescriptor);

            if (role is PseudonymServiceDescriptor pseudonymServiceDescriptor)
                ReadPseudonymServiceDescriptorAttributes(reader, pseudonymServiceDescriptor);

            if (role is SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
                ReadSecurityTokenServiceDescriptorAttributes(reader, securityTokenServiceDescriptor);

            base.ReadRoleDescriptorAttributes(reader, role);
        }

        protected virtual void ReadAttributeServiceDescriptorAttributes(XmlDictionaryReader reader, AttributeServiceDescriptor attributeServiceDescriptor)
        {
            // No default attributes
        }

        protected virtual void ReadPseudonymServiceDescriptorAttributes(XmlDictionaryReader reader, PseudonymServiceDescriptor pseudonymServiceDescriptor)
        {
            // No default attributes
        }

        protected virtual void ReadApplicationServiceDescriptorAttributes(XmlDictionaryReader reader, ApplicationServiceDescriptor applicationServiceDescriptor)
        {
            // No default attributes
        }

        protected virtual void ReadSecurityTokenServiceDescriptorAttributes(XmlDictionaryReader reader, SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
        {
            // No default attributes
        }

        protected override void WriteRoleDescriptorAttributes(XmlDictionaryWriter writer, RoleDescriptor role)
        {
            base.WriteRoleDescriptorAttributes(writer, role);
            if (role is ApplicationServiceDescriptor applicationServiceDescriptor)
                WriteApplicationServiceDescriptorAttributes(writer, applicationServiceDescriptor);

            if (role is AttributeServiceDescriptor attributeServiceDescriptor)
                WriteAttributeServiceDescriptorAttributes(writer, attributeServiceDescriptor);

            if (role is PseudonymServiceDescriptor pseudonymServiceDescriptor)
                WritePseudonymServiceDescriptorAttributes(writer, pseudonymServiceDescriptor);

            if (role is SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
                WriteSecurityTokenServiceDescriptorAttributes(writer, securityTokenServiceDescriptor);
        }

        protected override void WriteRoleDescriptorChildren(XmlDictionaryWriter writer, RoleDescriptor role)
        {
            base.WriteRoleDescriptorChildren(writer, role);
            if (role is ApplicationServiceDescriptor applicationServiceDescriptor)
                WriteApplicationServiceDescriptorChildren(writer, applicationServiceDescriptor);

            if (role is AttributeServiceDescriptor attributeServiceDescriptor)
                WriteAttributeServiceDescriptorChildren(writer, attributeServiceDescriptor);

            if (role is PseudonymServiceDescriptor pseudonymServiceDescriptor)
                WritePseudonymServiceDescriptorChildren(writer, pseudonymServiceDescriptor);

            if (role is SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
                WriteSecurityTokenServiceDescriptorChildren(writer, securityTokenServiceDescriptor);
        }

        protected virtual void WriteSecurityTokenServiceDescriptorAttributes(XmlDictionaryWriter writer, SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
        {
            WriteWebServiceDescriptorAttributes(writer, securityTokenServiceDescriptor);
        }

        protected virtual void WriteSecurityTokenServiceDescriptorChildren(XmlDictionaryWriter writer, SecurityTokenServiceDescriptor securityTokenServiceDescriptor)
        {
            if (securityTokenServiceDescriptor == null) return;
            if (!securityTokenServiceDescriptor.SecurityTokenServiceEndpoint.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Saml2MetadataConstants.Elements.RoleDescriptor, securityTokenServiceDescriptor.GetXmlTypeName());

            WriteWebServiceDescriptorChildren(writer, securityTokenServiceDescriptor);

            foreach (var securityTokenServiceEndpoint in securityTokenServiceDescriptor.SecurityTokenServiceEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.SecurityTokenServiceEndpoint, Namespace, securityTokenServiceEndpoint);

            foreach (var singleSignOutSubscriptionEndpoint in securityTokenServiceDescriptor.SingleSignOutSubscriptionEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.SingleSignOutSubscriptionEndpoint, Namespace, singleSignOutSubscriptionEndpoint);

            foreach (var singleSignOutNotificationEndpoint in securityTokenServiceDescriptor.SingleSignOutNotificationEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.SingleSignOutNotificationEndpoint, Namespace, singleSignOutNotificationEndpoint);

            foreach (var passiveRequestorEndpoint in securityTokenServiceDescriptor.PassiveRequestorEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.PassiveRequestorEndpoint, Namespace, passiveRequestorEndpoint);
        }

        protected virtual void WriteApplicationServiceDescriptorAttributes(XmlDictionaryWriter writer, ApplicationServiceDescriptor applicationServiceDescriptor)
        {
            WriteWebServiceDescriptorAttributes(writer, applicationServiceDescriptor);
        }

        protected virtual void WriteApplicationServiceDescriptorChildren(XmlDictionaryWriter writer, ApplicationServiceDescriptor applicationServiceDescriptor)
        {
            if (applicationServiceDescriptor == null) return;
            if (!applicationServiceDescriptor.ApplicationServiceEndpoint.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Saml2MetadataConstants.Elements.RoleDescriptor, applicationServiceDescriptor.GetXmlTypeName());

            WriteWebServiceDescriptorChildren(writer, applicationServiceDescriptor);

            foreach (var applicationServiceEndpoint in applicationServiceDescriptor.ApplicationServiceEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.ApplicationServiceEndpoint, Namespace, applicationServiceEndpoint);

            foreach (var singleSignOutNotificationEndpoint in applicationServiceDescriptor.SingleSignOutNotificationEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.SingleSignOutNotificationEndpoint, Namespace, singleSignOutNotificationEndpoint);

            foreach (var passiveRequestorEndpoint in applicationServiceDescriptor.PassiveRequestorEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.PassiveRequestorEndpoint, Namespace, passiveRequestorEndpoint);
        }

        protected virtual void WriteAttributeServiceDescriptorAttributes(XmlDictionaryWriter writer, AttributeServiceDescriptor attributeServiceDescriptor)
        {
            WriteWebServiceDescriptorAttributes(writer, attributeServiceDescriptor);
        }

        protected virtual void WriteAttributeServiceDescriptorChildren(XmlDictionaryWriter writer, AttributeServiceDescriptor attributeServiceDescriptor)
        {
            if (attributeServiceDescriptor == null) return;
            if (!attributeServiceDescriptor.AttributeServiceEndpoint.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Saml2MetadataConstants.Elements.RoleDescriptor, attributeServiceDescriptor.GetXmlTypeName());

            WriteWebServiceDescriptorChildren(writer, attributeServiceDescriptor);

            foreach (var attributeServiceEndpoint in attributeServiceDescriptor.AttributeServiceEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.AttributeServiceEndpoint, Namespace, attributeServiceEndpoint);

            foreach (var singleSignOutNotificationEndpoint in attributeServiceDescriptor.SingleSignOutNotificationEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.SingleSignOutNotificationEndpoint, Namespace, singleSignOutNotificationEndpoint);
        }

        protected virtual void WritePseudonymServiceDescriptorAttributes(XmlDictionaryWriter writer, PseudonymServiceDescriptor pseudonymServiceDescriptor)
        {
            WriteWebServiceDescriptorAttributes(writer, pseudonymServiceDescriptor);
        }

        protected virtual void WritePseudonymServiceDescriptorChildren(XmlDictionaryWriter writer, PseudonymServiceDescriptor pseudonymServiceDescriptor)
        {
            if (pseudonymServiceDescriptor == null) return;
            if (!pseudonymServiceDescriptor.PseudonymServiceEndpoint.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Saml2MetadataConstants.Elements.RoleDescriptor, pseudonymServiceDescriptor.GetXmlTypeName());

            WriteWebServiceDescriptorChildren(writer, pseudonymServiceDescriptor);

            foreach (var pseudonymServiceEndpoint in pseudonymServiceDescriptor.PseudonymServiceEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.PseudonymServiceEndpoint, Namespace, pseudonymServiceEndpoint);

            foreach (var singleSignOutNotificationEndpoint in pseudonymServiceDescriptor.SingleSignOutNotificationEndpoint)
                WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Prefix, Elements.SingleSignOutNotificationEndpoint, Namespace, singleSignOutNotificationEndpoint);
        }

        protected virtual void WriteWebServiceDescriptorChildren(XmlDictionaryWriter writer, WebServiceDescriptor webServiceDescriptor)
        {
            WriteLogicalServiceNamesOffered(writer, webServiceDescriptor.IssuerNames);
            WriteTokenTypesOffered(writer, webServiceDescriptor.TokenTypesOffered);
            WriteClaimDialectsOffered(writer, webServiceDescriptor.ClaimDialectsOffered);
            WriteClaimTypes(writer, Prefix, Elements.ClaimTypesOffered, Namespace, webServiceDescriptor.ClaimTypesOffered);
            WriteClaimTypes(writer, Prefix, Elements.ClaimTypesRequested, Namespace, webServiceDescriptor.ClaimTypesRequested);
            _ = writer.TryWriteElementValue(Elements.AutomaticPseudonyms, Namespace, webServiceDescriptor.AutomaticPseudonyms);
            WsAddressingSerializer.WriteEndpointReferenceCollection(writer, Elements.TargetScopes, Namespace, webServiceDescriptor.TargetScopes);
        }

        protected virtual void WriteWebServiceDescriptorAttributes(XmlDictionaryWriter writer, WebServiceDescriptor webServiceDescriptor)
        {
            if (webServiceDescriptor == null) return;

            WriteWebServiceDescriptorTypeAttribute(writer, webServiceDescriptor);

            _ = writer.TryWriteAttributeValue(Attributes.ServiceDisplayName, webServiceDescriptor.DisplayName);
            _ = writer.TryWriteAttributeValue(Attributes.ServiceDescription, webServiceDescriptor.Description);
        }

        protected virtual void WriteWebServiceDescriptorTypeAttribute(XmlDictionaryWriter writer, WebServiceDescriptor webServiceDescriptor)
        {
            var prefix = writer.LookupPrefix(Namespace);
            if (string.IsNullOrEmpty(prefix))
            {
                // this is added for testing purposes
                writer.WriteXmlnsAttribute(Prefix, Namespace);
                prefix = Prefix;
            }
            var type = $"{prefix}:{webServiceDescriptor.GetType().Name.Replace("Descriptor", string.Empty)}Type";
            writer.WriteAttributeString(XsiConstants.Prefix, XsiConstants.Attributes.Type, XsiConstants.Namespace, type);
        }

        protected virtual void WriteLogicalServiceNamesOffered(XmlDictionaryWriter writer, ICollection<Uri> issuerNames)
        {
            if (!issuerNames.Any()) return;
            writer.WriteStartElement(Prefix, Elements.LogicalServiceNamesOffered, Namespace);

            foreach (var issuerName in issuerNames)
                WriteElementWithUriAttribute(writer, Prefix, Elements.IssuerName, Namespace, issuerName);

            writer.WriteEndElement();
        }

        protected virtual void WriteTokenTypesOffered(XmlDictionaryWriter writer, ICollection<Uri> tokenTypes)
        {
            if (!tokenTypes.Any()) return;
            writer.WriteStartElement(Prefix, Elements.TokenTypesOffered, Namespace);

            foreach (var tokenType in tokenTypes)
                WriteElementWithUriAttribute(writer, Prefix, Elements.TokenType, Namespace, tokenType);

            writer.WriteEndElement();
        }

        protected virtual void WriteClaimDialectsOffered(XmlDictionaryWriter writer, ICollection<Uri> claimDialectsOffered)
        {
            if (!claimDialectsOffered.Any()) return;
            writer.WriteStartElement(Prefix, Elements.ClaimDialectsOffered, Namespace);

            foreach (var claimDialect in claimDialectsOffered)
                WriteElementWithUriAttribute(writer, Prefix, Elements.ClaimDialect, Namespace, claimDialect);

            writer.WriteEndElement();
        }

        protected virtual void WriteClaimTypes(XmlDictionaryWriter writer, string prefix, string name, string ns, ICollection<ClaimType> claimTypes)
        {
            if (!claimTypes.Any()) return;
            writer.WriteStartElement(prefix, name, ns);

            foreach (var claimType in claimTypes)
                WsAuthorizationSerializer.WriteClaimType(writer, claimType);

            writer.WriteEndElement();
        }

        protected virtual void WriteElementWithUriAttribute(XmlDictionaryWriter writer, string prefix, string name, string ns, Uri uri)
        {
            if (uri == null) return;
            writer.WriteStartElement(prefix, name, ns);
            _ = writer.TryWriteAttributeValue(Attributes.Uri, uri);
            writer.WriteEndElement();
        }
    }
}
