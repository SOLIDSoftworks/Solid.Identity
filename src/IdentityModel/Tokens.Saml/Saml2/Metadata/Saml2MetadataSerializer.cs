using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Elements = Solid.IdentityModel.Tokens.Saml2.Metadata.Saml2MetadataConstants.Elements;
using Attributes = Solid.IdentityModel.Tokens.Saml2.Metadata.Saml2MetadataConstants.Attributes;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class Saml2MetadataSerializer
    {
        public Saml2Serializer Saml2Serializer { get; }
        protected DSigSerializer DSigSerializer { get; }

        public Saml2MetadataSerializer()
            : this(new Saml2Serializer(), new DSigSerializer())
        {

        }

        public Saml2MetadataSerializer(Saml2Serializer saml2Serializer, DSigSerializer dSigSerializer)
        {
            Saml2Serializer = saml2Serializer;
            DSigSerializer = dSigSerializer;
        }

        public virtual Saml2Metadata ReadMetadata(XmlDictionaryReader reader)
        {
            if (TryReadSaml2Metadata(reader, out var metadata)) return metadata;
            return null;
        }

        public virtual void WriteMetadata(XmlWriter writer, Saml2Metadata metadata, SecurityKey key, SignatureMethod signatureMethod)
            => WriteMetadata(writer, metadata, key, algorithm: signatureMethod.SignatureAlgortihm, digest: signatureMethod.DigestAlgorithm);

        public virtual void WriteMetadata(XmlWriter writer, Saml2Metadata metadata, SecurityKey key, string algorithm = SecurityAlgorithms.RsaSha256Signature, string digest = SecurityAlgorithms.Sha256Digest)
        {
            var credentials = new SigningCredentials(key, algorithm, digest);
            using (var envelopedSignatureWriter = new EnvelopedSignatureWriter(writer, credentials, $"_{Guid.NewGuid().ToString("N")}"))
                WriteMetadata(envelopedSignatureWriter, metadata);
        }

        public virtual void WriteMetadata(XmlWriter writer, Saml2Metadata metadata)
        {
            metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            if (writer is XmlDictionaryWriter xmlDictionaryWriter)
                WriteSaml2Metadata(xmlDictionaryWriter, metadata);
            else
                WriteSaml2Metadata(XmlDictionaryWriter.CreateDictionaryWriter(writer), metadata);
        }

        #region Elements

        protected virtual void WriteSaml2Metadata(XmlDictionaryWriter writer, Saml2Metadata metadata)
        {
            if (metadata is EntityDescriptor entity)
                WriteEntityDescriptor(writer, entity);
            else if (metadata is EntitiesDescriptor entities)
                WriteEntitiesDescriptor(writer, entities);
            else
                throw new InvalidOperationException($"Unable to serialize type '{metadata.GetType().FullName}'.");
        }

        protected virtual void WriteEntitiesDescriptor(XmlDictionaryWriter writer, EntitiesDescriptor entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            writer.WriteStartElement(Saml2MetadataConstants.Prefix, Elements.EntityDescriptor, Saml2MetadataConstants.Namespace);
            WriteEntitiesDescriptorAttributes(writer, entities);
            WriteEntitiesDescriptorChildren(writer, entities);
            writer.WriteEndElement();
        }

        protected virtual void WriteEntityDescriptor(XmlDictionaryWriter writer, EntityDescriptor entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!IsValidEntityId(entity.EntityId))
                throw new ArgumentException("Invalid entity id.", nameof(entity));

            writer.WriteStartElement(Saml2MetadataConstants.Prefix, Elements.EntityDescriptor, Saml2MetadataConstants.Namespace);

            WriteEntityDescriptorAttributes(writer, entity);
            WriteEntityDescriptorChildren(writer, entity);

            writer.WriteEndElement();
        }

        protected virtual void WriteAffiliationDescriptor(XmlDictionaryWriter writer, AffiliationDescriptor affiliation)
        {
            if (affiliation == null) throw new ArgumentNullException(nameof(affiliation));
            writer.WriteStartElement(Elements.AffiliationDescriptor, Saml2MetadataConstants.Namespace);
            WriteAffiliationDescriptorAttributes(writer, affiliation);
            WriteAffiliationDescriptorChildren(writer, affiliation);
            writer.WriteEndElement();
        }

        protected virtual void WriteRoleDescriptor(XmlDictionaryWriter writer, RoleDescriptor role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            writer.WriteStartElement(Elements.RoleDescriptor, Saml2MetadataConstants.Namespace);
            WriteRoleDescriptorAttributes(writer, role);
            WriteRoleDescriptorChildren(writer, role);
            writer.WriteEndElement();
        }

        protected virtual void WriteAuthnAuthorityDescriptor(XmlDictionaryWriter writer, AuthnAuthorityDescriptor authnAuthority)
        {
            if (authnAuthority == null) throw new ArgumentNullException(nameof(authnAuthority));
            writer.WriteStartElement(Elements.AuthnAuthorityDescriptor, Saml2MetadataConstants.Namespace);
            WriteAuthnAuthorityDescriptorAttributes(writer, authnAuthority);
            WriteAuthnAuthorityDescriptorChildren(writer, authnAuthority);
            writer.WriteEndElement();
        }

        protected virtual void WritePdpDescriptor(XmlDictionaryWriter writer, PdpDescriptor pdp)
        {
            if (pdp == null) throw new ArgumentNullException(nameof(pdp));
            writer.WriteStartElement(Elements.PdpDescriptor, Saml2MetadataConstants.Namespace);
            WritePdpDescriptorAttributes(writer, pdp);
            WritePdpDescriptorChildren(writer, pdp);
            writer.WriteEndElement();
        }

        protected virtual void WriteAttributeAuthorityDescriptor(XmlDictionaryWriter writer, AttributeAuthorityDescriptor attributeAuthority)
        {
            if (attributeAuthority == null) throw new ArgumentNullException(nameof(attributeAuthority));
            writer.WriteStartElement(Elements.AttributeAuthorityDescriptor, Saml2MetadataConstants.Namespace);
            WriteAttributeAuthorityDescriptorAttributes(writer, attributeAuthority);
            WriteAttributeAuthorityDescriptorChildren(writer, attributeAuthority);
            writer.WriteEndElement();
        }

        protected virtual void WriteSpSsoDescriptor(XmlDictionaryWriter writer, SpSsoDescriptor sp)
        {
            if (sp == null) throw new ArgumentNullException(nameof(sp));
            writer.WriteStartElement(Elements.SpSsoDescriptor, Saml2MetadataConstants.Namespace);
            WriteSpSsoDescriptorAttributes(writer, sp);
            WriteSpSsoDescriptorChildren(writer, sp);
            writer.WriteEndElement();
        }

        protected virtual void WriteIdpSsoDescriptor(XmlDictionaryWriter writer, IdpSsoDescriptor idp)
        {
            if (idp == null) throw new ArgumentNullException(nameof(idp));
            writer.WriteStartElement(Elements.IdpSsoDescriptor, Saml2MetadataConstants.Namespace);
            WriteIdpSsoDescriptorAttributes(writer, idp);
            WriteIdpSsoDescriptorChildren(writer, idp);
            writer.WriteEndElement();
        }

        protected virtual void WriteContactPerson(XmlDictionaryWriter writer, ContactPerson contactPerson)
        {
            if (contactPerson == null) throw new ArgumentNullException(nameof(contactPerson));
            writer.WriteStartElement(Elements.ContactPerson, Saml2MetadataConstants.Namespace);
            WriteContactPersonAttributes(writer, contactPerson);
            WriteContactPersonChildren(writer, contactPerson);
            writer.WriteEndElement();
        }

        protected virtual void WriteOrganization(XmlDictionaryWriter writer, Organization organization)
        {
            if (organization == null) throw new ArgumentNullException(nameof(organization));
            writer.WriteStartElement(Elements.Organization, Saml2MetadataConstants.Namespace);
            WriteOrganizationAttributes(writer, organization);
            WriteOrganizationChildren(writer, organization);
            writer.WriteEndElement();
        }

        protected virtual void WriteKeyDescriptor(XmlDictionaryWriter writer, KeyDescriptor keyDescriptor)
        {
            if (keyDescriptor == null) throw new ArgumentNullException(nameof(keyDescriptor));
            writer.WriteStartElement(Elements.KeyDescriptor, Saml2MetadataConstants.Namespace);
            WriteKeyDescriptorAttributes(writer, keyDescriptor);
            WriteKeyDescriptorChildren(writer, keyDescriptor);
            writer.WriteEndElement();
        }

        protected virtual void WriteEndpoint(XmlDictionaryWriter writer, string localName, string ns, Endpoint endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            writer.WriteStartElement(localName, ns);
            WriteEndpointAttributes(writer, localName, endpoint);
            WriteEndpointChildren(writer, localName, endpoint);
            writer.WriteEndElement();
        }

        protected virtual void WriteIndexedEndpoint(XmlDictionaryWriter writer, string localName, string ns, IndexedEndpoint indexedEndpoint)
        {
            if (indexedEndpoint == null) throw new ArgumentNullException(nameof(indexedEndpoint));
            writer.WriteStartElement(localName, ns);
            WriteIndexedEndpointAttributes(writer, localName, indexedEndpoint);
            WriteIndexedEndpointChildren(writer, localName, indexedEndpoint);
            writer.WriteEndElement();
        }

        protected virtual void WriteAttributeConsumingService(XmlDictionaryWriter writer, AttributeConsumingService attributeConsumingService)
        {
            if (attributeConsumingService == null) throw new ArgumentNullException(nameof(attributeConsumingService));
            writer.WriteStartElement(Elements.AttributeConsumingService, Saml2MetadataConstants.Namespace);
            WriteAttributeConsumingServiceAttributes(writer, attributeConsumingService);
            WriteAttributeConsumingServiceChildren(writer, attributeConsumingService);
            writer.WriteEndElement();
        }

        protected virtual void WriteAdditionalMetadataLocation(XmlDictionaryWriter writer, AdditionalMetadataLocation location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            writer.WriteStartElement(Elements.AdditionalMetadataLocation, Saml2MetadataConstants.Namespace);
            WriteAdditionalMetadataLocationAttributes(writer, location);
            WriteAdditionalMetadataLocationChildren(writer, location);
            writer.WriteEndElement();
        } 

        protected virtual void WriteLocalizedName(XmlDictionaryWriter writer, string localName, string ns, LocalizedName localizedName)
        {
            if (localizedName == null) throw new ArgumentNullException(nameof(localizedName));
            writer.WriteStartElement(localName, ns);
            WriteLocalizedNameAttributes(writer, localName, localizedName);
            WriteLocalizedNameChildren(writer, localName, localizedName);
            writer.WriteEndElement();
        }

        protected virtual void WriteLocalizedUri(XmlDictionaryWriter writer, string localName, string ns, LocalizedUri localizedUri)
        {
            if (localizedUri == null) throw new ArgumentNullException(nameof(localizedUri));
            writer.WriteStartElement(localName, ns);
            WriteLocalizedUriAttributes(writer, localName, localizedUri);
            WriteLocalizedUriChildren(writer, localName, localizedUri);
            writer.WriteEndElement();
        }

        protected virtual void WriteSaml2Attribute(XmlDictionaryWriter writer, Saml2Attribute attribute) 
            => Saml2Serializer.WriteAttribute(writer, attribute);

        protected virtual void WriteRequestedAttribute(XmlDictionaryWriter writer, RequestedSaml2Attribute requestedAttribute)
        {
            if (requestedAttribute == null) throw new ArgumentNullException(nameof(requestedAttribute));
            writer.WriteStartElement(Elements.RequestedAttribute, Saml2MetadataConstants.Namespace);
            WriteRequestedAttributeAttributes(writer, requestedAttribute);
            WriteRequestedAttributeChildren(writer, requestedAttribute);
            writer.WriteEndElement();
        }

        protected virtual void WriteEntityIdElement(XmlDictionaryWriter writer, string localName, string ns, Uri entityId)
        {
            if (!IsValidEntityId(entityId))
                throw new ArgumentException($"Invalid value for element '{ns}#{localName}'.");

            writer.WriteStartElement(localName, ns);
            writer.WriteValue(entityId.OriginalString);
            writer.WriteEndElement();
        }

        protected virtual bool TryReadSaml2Metadata(XmlDictionaryReader reader, out Saml2Metadata metadata)
        {
            if (TryReadEntityDescriptor(reader, out var entity)) return Out.True(entity, out metadata);
            if (TryReadEntitiesDescriptor(reader, out var entities)) return Out.True(entities, out metadata);

            return Out.False(out metadata);
        }

        protected virtual bool TryReadEntityDescriptor(XmlDictionaryReader reader, out EntityDescriptor entity)
        {
            if (!reader.IsStartElement(Elements.EntityDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out entity);

            var e = new EntityDescriptor();
            ReadEntityDescriptorAttributes(reader, e);
            reader.ForEachChild(r => TryReadEntityDescriptorChild(reader, e), out var signature);
            e.Signature = signature;
            entity = e;

            return true;
        }

        protected virtual bool TryReadEntitiesDescriptor(XmlDictionaryReader reader, out EntitiesDescriptor entities)
        {
            if (!reader.IsStartElement(Elements.EntitiesDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out entities);

            var e = new EntitiesDescriptor();
            ReadEntitesDescriptorAttributes(reader, e);
            reader.ForEachChild(r => TryReadEntitiesDescriptorChild(r, e), out var signature);
            e.Signature = signature;
            entities = e;
            return true;
        }

        protected virtual bool TryReadIdpSsoDescriptor(XmlDictionaryReader reader, out IdpSsoDescriptor idpSso)
        {
            if (!reader.IsStartElement(Elements.IdpSsoDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out idpSso);

            var idp = new IdpSsoDescriptor();
            ReadIdpSsoDescriptorAttributes(reader, idp);
            reader.ForEachChild(r => TryReadIdpSsoDescriptorChild(r, idp), out var signature);
            idp.Signature = signature;
            idpSso = idp;
            return true;
        }

        protected virtual bool TryReadSpSsoDescriptor(XmlDictionaryReader reader, out SpSsoDescriptor spSso)
        {
            if (!reader.IsStartElement(Elements.SpSsoDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out spSso);

            var sp = new SpSsoDescriptor();
            ReadSpSsoDescriptorAttributes(reader, sp);
            reader.ForEachChild(r => TryReadSpSsoDescriptorChild(r, sp), out var signature);
            sp.Signature = signature;
            spSso = sp;
            return true;
        }

        protected virtual bool TryReadRoleDescriptor(XmlDictionaryReader reader, out RoleDescriptor role)
        {
            if (!reader.IsStartElement(Elements.RoleDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out role);

            var d = new RoleDescriptor();
            ReadRoleDescriptorAttributes(reader, d);

            reader.ForEachChild(r => TryReadRoleDescriptorChild(r, d), out var signature);
            d.Signature = signature;

            role = d;
            return true;
        }

        protected virtual bool TryReadAuthnAuthorityDescriptor(XmlDictionaryReader reader, out AuthnAuthorityDescriptor authnAuthorityDescriptor)
        {
            if (!reader.IsStartElement(Elements.AuthnAuthorityDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out authnAuthorityDescriptor);

            var authority = new AuthnAuthorityDescriptor();
            ReadAuthnAuthorityDescriptorAttributes(reader, authority);
            reader.ForEachChild(r => TryReadAuthnAuthorityDescriptorChild(r, authority), out var signature);
            authority.Signature = signature;
            authnAuthorityDescriptor = authority;
            return true;
        }

        protected virtual bool TryReadAttributeAuthorityDescriptor(XmlDictionaryReader reader, out AttributeAuthorityDescriptor attributeAuthorityDescriptor)
        {
            if (!reader.IsStartElement(Elements.AttributeAuthorityDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out attributeAuthorityDescriptor);

            var authority = new AttributeAuthorityDescriptor();
            ReadAttributeAuthorityDescriptorAttributes(reader, authority);
            reader.ForEachChild(r => TryReadAttributeAuthorityDescriptorChild(r, authority), out var signature);
            authority.Signature = signature;
            attributeAuthorityDescriptor = authority;
            return true;
        }


        protected virtual bool TryReadPdpDescriptor(XmlDictionaryReader reader, out PdpDescriptor pdpDescriptor)
        {
            if (!reader.IsStartElement(Elements.PdpDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out pdpDescriptor);

            var pdp = new PdpDescriptor();
            ReadPdpDescriptorAttributes(reader, pdp);
            reader.ForEachChild(r => TryReadPdpDescriptorChild(r, pdp), out var signature);
            pdp.Signature = signature;
            pdpDescriptor = pdp;
            return true;
        }

        protected virtual bool TryReadAffiliationDescriptor(XmlDictionaryReader reader, out AffiliationDescriptor affiliationDescriptor)
        {
            if (!reader.IsStartElement(Elements.AffiliationDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out affiliationDescriptor);

            var affiliation = new AffiliationDescriptor();
            ReadAffiliationDescriptorAttributes(reader, affiliation);
            reader.ForEachChild(r => TryReadAffiliationDescriptorChild(r, affiliation), out var signature);
            affiliation.Signature = signature;
            affiliationDescriptor = affiliation;
            return true;
        }

        protected virtual bool TryReadSaml2Attribute(XmlDictionaryReader reader, out Saml2Attribute saml2Attribute)
        {
            if (!reader.IsStartElement(Saml2Constants.Elements.Attribute, Saml2Constants.Namespace))
                return Out.False(out saml2Attribute);

            var attribute = Saml2Serializer.ReadAttribute(reader);
            saml2Attribute = attribute;
            return true;
        }

        protected virtual bool TryReadAttributeConsumingService(XmlDictionaryReader reader, out AttributeConsumingService attributeConsumingService)
        {
            if (!reader.IsStartElement(Elements.AttributeConsumingService, Saml2MetadataConstants.Namespace))
                return Out.False(out attributeConsumingService);

            var service = new AttributeConsumingService();
            if (reader.TryReadAttribute(Attributes.Index, out var indexStr) && ushort.TryParse(indexStr, out var index))
                service.Index = index;

            if (reader.TryReadAttribute(Attributes.IsDefault, out var isDefaultStr) && bool.TryParse(isDefaultStr, out var isDefault))
                service.IsDefault = isDefault;

            reader.ForEachChild(r =>
            {
                if (TryReadLocalizedName(reader, Elements.ServiceName, Saml2MetadataConstants.Namespace, out var name))
                    service.Name.Add(name);
                else if (TryReadLocalizedName(reader, Elements.ServiceDescription, Saml2MetadataConstants.Namespace, out var description))
                    service.Description.Add(description);
                else if (TryReadRequestedAttribute(reader, out var attribute))
                    service.RequestedAttribute.Add(attribute);
                else
                    return false;

                return true;
            });

            attributeConsumingService = service;
            return true;
        }

        protected virtual bool TryReadRequestedAttribute(XmlDictionaryReader reader, out RequestedSaml2Attribute requestedAttribute)
        {
            if (!reader.IsStartElement(Elements.RequestedAttribute, Saml2MetadataConstants.Namespace))
                return Out.False(out requestedAttribute);

            if (!reader.TryReadAttribute(Saml2Constants.Attributes.Name, out var name))
                return Out.False(out requestedAttribute);

            var attribute = new RequestedSaml2Attribute(name);

            if (reader.TryReadAttributeAsUri(Saml2Constants.Attributes.NameFormat, out var nameFormat))
                attribute.NameFormat = nameFormat;
            if (reader.TryReadAttribute(Saml2Constants.Attributes.FriendlyName, out var friendlyName))
                attribute.FriendlyName = friendlyName;

            // Should we read attribute values?
            reader.ForEachChild(_ => false);
            requestedAttribute = attribute;
            return true;
        }

        protected virtual bool TryReadOrganization(XmlDictionaryReader reader, out Organization organization)
        {
            if (!reader.IsStartElement(Elements.Organization, Saml2MetadataConstants.Namespace))
                return Out.False(out organization);

            var o = new Organization();
            reader.ForEachChild(r =>
            {
                if (TryReadLocalizedName(reader, Elements.OrganizationName, Saml2MetadataConstants.Namespace, out var organizationName))
                    o.Name.Add(organizationName);
                else if (TryReadLocalizedName(reader, Elements.OrganizationDisplayName, Saml2MetadataConstants.Namespace, out var organizationDisplayName))
                    o.DisplayName.Add(organizationDisplayName);
                else if (TryReadLocalizedUri(reader, Elements.OrganizationUrl, Saml2MetadataConstants.Namespace, out var organizationUrl))
                    o.Url.Add(organizationUrl);
                else
                    return false;

                return true;
            });
            organization = o;
            return true;
        }

        protected virtual bool TryReadKeyDescriptor(XmlDictionaryReader reader, out KeyDescriptor keyDescriptor)
        {
            if (!reader.IsStartElement(Elements.KeyDescriptor, Saml2MetadataConstants.Namespace))
                return Out.False(out keyDescriptor);

            var d = new KeyDescriptor();
            if (reader.TryReadAttribute(Attributes.Use, out var str) && Enum.TryParse<KeyUse>(str, true, out var use))
                d.Use = use;

            reader.ForEachChild(r =>
            {
                if (r.IsStartElement(XmlSignatureConstants.Elements.KeyInfo, XmlSignatureConstants.Namespace))
                    d.KeyInfo = DSigSerializer.ReadKeyInfo(reader);
                else
                    return false;

                return true;
            });

            keyDescriptor = d;
            return true;
        }

        protected virtual bool TryReadIndexedEndpoint(XmlDictionaryReader reader, string localName, string ns, out IndexedEndpoint indexedEndpoint)
        {
            if (!reader.IsStartElement(localName, ns)) return Out.False(out indexedEndpoint);

            var e = new IndexedEndpoint();
            ReadIndexedEndpointAttributes(reader, e);
            _ = reader.ReadOuterXml();
            indexedEndpoint = e;
            return true;
        }

        protected virtual bool TryReadEndpoint(XmlDictionaryReader reader, string localName, string ns, out Endpoint endpoint)
        {
            if (!reader.IsStartElement(localName, ns)) return Out.False(out endpoint);

            var e = new Endpoint();
            ReadEndpointAttributes(reader, e);
            _ = reader.ReadOuterXml();
            endpoint = e;
            return true;
        }

        protected virtual bool TryReadContactPerson(XmlDictionaryReader reader, out ContactPerson contact)
        {
            if (!reader.IsStartElement(Elements.ContactPerson, Saml2MetadataConstants.Namespace))
                return Out.False(out contact);

            var c = new ContactPerson();

            if (reader.TryReadAttribute(Attributes.ContactType, out var value) && Enum.TryParse<ContactType>(value, true, out var contactType))
                c.ContactType = contactType;

            reader.ForEachChild(r => TryReadContactPersonChild(r, c));

            contact = c;
            return true;
        }

        protected virtual bool TryReadAdditionalMetadataLocation(XmlDictionaryReader reader, out AdditionalMetadataLocation additionalMetadataLocation)
        {
            if (!reader.IsStartElement(Elements.AdditionalMetadataLocation, Saml2MetadataConstants.Namespace))
                return Out.False(out additionalMetadataLocation);

            var location = new AdditionalMetadataLocation();

            if (reader.TryReadAttributeAsUri(Attributes.Namespace, out var ns))
                location.Namespace = ns;

            var content = reader.ReadElementContentAsUri();
            location.Value = content;
            additionalMetadataLocation = location;
            return true;
        }

        #endregion Elements

        #region Child elements

        protected virtual void WriteEntitiesDescriptorChild(XmlDictionaryWriter writer, DescriptorBase descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor is EntityDescriptor entity)
                WriteEntityDescriptor(writer, entity);
            else if (descriptor is EntitiesDescriptor entities)
                WriteEntitiesDescriptor(writer, entities);
            else
                throw new InvalidOperationException($"Unable to serialize type '{descriptor.GetType().FullName}'.");
        }

        protected virtual void WriteEntityDescriptorChild(XmlDictionaryWriter writer, DescriptorBase descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor is IdpSsoDescriptor idp)
                WriteIdpSsoDescriptor(writer, idp);
            else if (descriptor is SpSsoDescriptor sp)
                WriteSpSsoDescriptor(writer, sp);
            else if (descriptor is AttributeAuthorityDescriptor attributeAuthority)
                WriteAttributeAuthorityDescriptor(writer, attributeAuthority);
            else if (descriptor is AuthnAuthorityDescriptor authnAuthority)
                WriteAuthnAuthorityDescriptor(writer, authnAuthority);
            else if (descriptor is PdpDescriptor pdp)
                WritePdpDescriptor(writer, pdp);
            else if (descriptor is AffiliationDescriptor affiliation)
                WriteAffiliationDescriptor(writer, affiliation);
            else if (descriptor is RoleDescriptor role)
                WriteRoleDescriptor(writer, role);
            else
                throw new ArgumentException($"Unable to serialize type '{descriptor.GetType().FullName}'");

        }
        protected virtual void WriteEntitiesDescriptorChildren(XmlDictionaryWriter writer, EntitiesDescriptor entities)
        {
            if (writer is EnvelopedSignatureWriter envelopedSignatureWriter)
                envelopedSignatureWriter.WriteSignature();

            foreach (var item in entities.Items)
                WriteEntitiesDescriptorChild(writer, item);
        }

        protected virtual void WriteEntityDescriptorChildren(XmlDictionaryWriter writer, EntityDescriptor entity)
        {
            if (writer is EnvelopedSignatureWriter envelopedSignatureWriter)
                envelopedSignatureWriter.WriteSignature();

            foreach (var item in entity.Items)
                WriteEntityDescriptorChild(writer, item);
            if (entity.Organization != null)
                WriteOrganization(writer, entity.Organization);
            foreach (var contactPerson in entity.ContactPerson)
                WriteContactPerson(writer, contactPerson);
            foreach (var location in entity.AdditionalMetadataLocation)
                WriteAdditionalMetadataLocation(writer, location);
        }

        protected virtual void WriteAffiliationDescriptorChildren(XmlDictionaryWriter writer, AffiliationDescriptor affiliation)
        {
            if (!affiliation.AffiliateMember.Any())
                throw new InvalidOperationException("At least one 'AffiliateMember' element is required.");

            WriteBaseChildren(writer, affiliation);
        }

        protected virtual void WriteRoleDescriptorChildren(XmlDictionaryWriter writer, RoleDescriptor role)
        {
            WriteBaseChildren(writer, role);
            foreach (var keyDescriptor in role.KeyDescriptors)
                WriteKeyDescriptor(writer, keyDescriptor);

            foreach (var contactPerson in role.ContactPerson)
                WriteContactPerson(writer, contactPerson);

            if (role.Organization != null)
                WriteOrganization(writer, role.Organization);
        }

        protected virtual void WriteSaml2MetadataChildren(XmlDictionaryWriter writer, Saml2Metadata metadata) 
            => WriteBaseChildren(writer, metadata);

        protected virtual void WriteSsoDescriptorChildren(XmlDictionaryWriter writer, SsoDescriptor sso)
        {
            WriteRoleDescriptorChildren(writer, sso);
            foreach (var artifactResolutionService in sso.ArtifactResolutionService)
                WriteIndexedEndpoint(writer, Elements.ArtifactResolutionService, Saml2MetadataConstants.Namespace, artifactResolutionService);
            foreach (var singleLogoutService in sso.SingleLogoutService)
                WriteEndpoint(writer, Elements.SingleLogoutService, Saml2MetadataConstants.Namespace, singleLogoutService);
            foreach (var managedNameIdService in sso.ManageNameIdService)
                WriteEndpoint(writer, Elements.ManageNameIdService, Saml2MetadataConstants.Namespace, managedNameIdService);
            foreach (var nameIdFormat in sso.NameIdFormat)
                _ = writer.TryWriteElementValue(Elements.NameIdFormat, Saml2MetadataConstants.Namespace, nameIdFormat);
        }

        protected virtual void WriteBaseChildren(XmlDictionaryWriter writer, DescriptorBase descriptor)
        {
            // No default child elements
        }

        protected virtual void WriteAuthnAuthorityDescriptorChildren(XmlDictionaryWriter writer, AuthnAuthorityDescriptor authnAuthority)
        {
            if (!authnAuthority.AuthnQueryService.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.AuthnAuthorityDescriptor, Elements.AuthnQueryService);

            WriteRoleDescriptorChildren(writer, authnAuthority);

            foreach (var authnQueryService in authnAuthority.AuthnQueryService)
                WriteEndpoint(writer, Elements.AuthnQueryService, Saml2MetadataConstants.Namespace, authnQueryService);
            foreach (var assertionIdRequestService in authnAuthority.AssertionIdRequestService)
                WriteEndpoint(writer, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, assertionIdRequestService);
            foreach (var nameIdFormat in authnAuthority.NameIdFormat)
                _ = writer.TryWriteElementValue(Elements.NameIdFormat, Saml2MetadataConstants.Namespace, nameIdFormat);
        }

        protected virtual void WritePdpDescriptorChildren(XmlDictionaryWriter writer, PdpDescriptor pdp)
        {
            if (!pdp.AuthzService.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.PdpDescriptor, Elements.AuthzService);

            WriteRoleDescriptorChildren(writer, pdp);

            foreach (var authzService in pdp.AuthzService)
                WriteEndpoint(writer, Elements.AuthnQueryService, Saml2MetadataConstants.Namespace, authzService);
            foreach (var assertionIdRequestService in pdp.AssertionIdRequestService)
                WriteEndpoint(writer, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, assertionIdRequestService);
            foreach (var nameIdFormat in pdp.NameIdFormat)
                _ = writer.TryWriteElementValue(Elements.NameIdFormat, Saml2MetadataConstants.Namespace, nameIdFormat);
        }

        protected virtual void WriteAttributeAuthorityDescriptorChildren(XmlDictionaryWriter writer, AttributeAuthorityDescriptor attributeAuthority)
        {
            if (!attributeAuthority.AttributeService.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.AttributeAuthorityDescriptor, Elements.AttributeService);

            WriteRoleDescriptorChildren(writer, attributeAuthority);

            foreach (var attributeService in attributeAuthority.AttributeService)
                WriteEndpoint(writer, Elements.AuthnQueryService, Saml2MetadataConstants.Namespace, attributeService);
            foreach (var assertionIdRequestService in attributeAuthority.AssertionIdRequestService)
                WriteEndpoint(writer, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, assertionIdRequestService);
            foreach (var nameIdFormat in attributeAuthority.NameIdFormat)
                _ = writer.TryWriteElementValue(Elements.NameIdFormat, Saml2MetadataConstants.Namespace, nameIdFormat);
            foreach (var attributeProfile in attributeAuthority.AttributeProfile)
                _ = writer.TryWriteElementValue(Elements.AttributeProfile, Saml2MetadataConstants.Namespace, attributeProfile);
            foreach (var attribute in attributeAuthority.Attributes)
                WriteSaml2Attribute(writer, attribute);
        }

        protected virtual void WriteSpSsoDescriptorChildren(XmlDictionaryWriter writer, SpSsoDescriptor sp)
        {
            if (!sp.AssertionConsumerService.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.SpSsoDescriptor, Elements.AssertionConsumerService);

            WriteSsoDescriptorChildren(writer, sp);
            foreach (var assertionConsumerService in sp.AssertionConsumerService)
                WriteIndexedEndpoint(writer, Elements.AssertionConsumerService, Saml2MetadataConstants.Namespace, assertionConsumerService);
            foreach (var attributeConsumingService in sp.AttributeConsumingService)
                WriteAttributeConsumingService(writer, attributeConsumingService);
        }

        protected virtual void WriteIdpSsoDescriptorChildren(XmlDictionaryWriter writer, IdpSsoDescriptor idp)
        {
            WriteSsoDescriptorChildren(writer, idp);
            foreach (var singleSignOnService in idp.SingleSignOnService)
                WriteEndpoint(writer, Elements.SingleSignOnService, Saml2MetadataConstants.Namespace, singleSignOnService);
            foreach(var nameIdMappingService in idp.NameIdMappingService)
                WriteEndpoint(writer, Elements.NameIdMappingService, Saml2MetadataConstants.Namespace, nameIdMappingService);
            foreach (var assertionIdRequestService in idp.AssertionIdRequestService)
                WriteEndpoint(writer, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, assertionIdRequestService);
            foreach (var attributeProfile in idp.AttributeProfile)
                _ = writer.TryWriteElementValue(Elements.AttributeProfile, Saml2MetadataConstants.Namespace, attributeProfile);
            foreach (var attribute in idp.Attributes)
                WriteSaml2Attribute(writer, attribute);
        }

        protected virtual void WriteContactPersonChildren(XmlDictionaryWriter writer, ContactPerson contactPerson)
        {
            _ = writer.TryWriteElementValue(Elements.Company, Saml2MetadataConstants.Namespace, contactPerson.Company);
            _ = writer.TryWriteElementValue(Elements.GivenName, Saml2MetadataConstants.Namespace, contactPerson.GivenName);
            _ = writer.TryWriteElementValue(Elements.SurName, Saml2MetadataConstants.Namespace, contactPerson.SurName);
            foreach (var emailAddress in contactPerson.EmailAddresses)
                _ = writer.TryWriteElementValue(Elements.EmailAddress, Saml2MetadataConstants.Namespace, emailAddress);
            foreach (var telephoneNumber in contactPerson.TelephoneNumbers)
                _ = writer.TryWriteElementValue(Elements.TelephoneNumber, Saml2MetadataConstants.Namespace, telephoneNumber);
        }

        protected virtual void WriteOrganizationChildren(XmlDictionaryWriter writer, Organization organization)
        {
            if (!organization.Name.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.Organization, Elements.OrganizationName);
            if (!organization.DisplayName.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.Organization, Elements.OrganizationDisplayName);
            if (!organization.Url.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.Organization, Elements.OrganizationUrl);

            foreach (var name in organization.Name)
                WriteLocalizedName(writer, Elements.OrganizationName, Saml2MetadataConstants.Namespace, name);
            foreach (var displayName in organization.DisplayName)
                WriteLocalizedName(writer, Elements.OrganizationDisplayName, Saml2MetadataConstants.Namespace, displayName);
            foreach (var url in organization.Url)
                WriteLocalizedUri(writer, Elements.OrganizationUrl, Saml2MetadataConstants.Namespace, url);
        }

        protected virtual void WriteKeyDescriptorChildren(XmlDictionaryWriter writer, KeyDescriptor keyDescriptor) 
            => DSigSerializer.WriteKeyInfo(writer, keyDescriptor.KeyInfo);

        protected virtual void WriteEndpointChildren(XmlDictionaryWriter writer, string elementLocalName, Endpoint endpoint)
        {
            // No default child elements
        }

        protected virtual void WriteIndexedEndpointChildren(XmlDictionaryWriter writer, string elementLocalName, IndexedEndpoint indexedEndpoint)
        {
            // No default child elements
        }

        protected virtual void WriteAttributeConsumingServiceChildren(XmlDictionaryWriter writer, AttributeConsumingService attributeConsumingService)
        {
            if (!attributeConsumingService.Name.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.AttributeConsumingService, Elements.ServiceName);
            if(!attributeConsumingService.RequestedAttribute.Any())
                throw XmlWriterExceptionHelper.CreateRequiredChildElementMissingException(Elements.AttributeConsumingService, Elements.RequestedAttribute);

            foreach (var name in attributeConsumingService.Name)
                WriteLocalizedName(writer, Elements.ServiceName, Saml2MetadataConstants.Namespace, name);
            foreach (var description in attributeConsumingService.Description)
                WriteLocalizedName(writer, Elements.ServiceDescription, Saml2MetadataConstants.Namespace, description);
            foreach (var attribute in attributeConsumingService.RequestedAttribute)
                WriteRequestedAttribute(writer, attribute);
        }

        protected virtual void WriteAdditionalMetadataLocationChildren(XmlDictionaryWriter writer, AdditionalMetadataLocation location)
        {
            if (!writer.TryWriteValue(location.Value))
                XmlWriterExceptionHelper.CreateRequiredElementValueMissingException(nameof(WriteAdditionalMetadataLocationChildren));
        }

        protected virtual void WriteLocalizedNameChildren(XmlDictionaryWriter writer, string elementLocalName, LocalizedName localizedName)
        {
            if (!writer.TryWriteValue(localizedName.Value))
                throw XmlWriterExceptionHelper.CreateRequiredElementValueMissingException(elementLocalName);
        }

        protected virtual void WriteLocalizedUriChildren(XmlDictionaryWriter writer, string elementLocalName, LocalizedUri localizedUri)
        {
            if (!writer.TryWriteValue(localizedUri.Value))
                throw XmlWriterExceptionHelper.CreateRequiredElementValueMissingException(elementLocalName);
        }

        protected virtual void WriteRequestedAttributeChildren(XmlDictionaryWriter writer, RequestedSaml2Attribute requestedAttribute)
        {
            // No default child elements
        }

        protected virtual bool TryReadEntitiesDescriptorChild(XmlDictionaryReader reader, EntitiesDescriptor entities)
        {
            if (TryReadSaml2Metadata(reader, out var metadata))
                entities.Items.Add(metadata);
            else
                return false;

            return true;
        }

        protected virtual bool TryReadEntityDescriptorChild(XmlDictionaryReader reader, EntityDescriptor entity)
        {
            if (TryReadRoleDescriptor(reader, out var roleDescriptor))
                entity.Items.Add(roleDescriptor);
            else if (TryReadIdpSsoDescriptor(reader, out var idp))
                entity.Items.Add(idp);
            else if (TryReadSpSsoDescriptor(reader, out var sp))
                entity.Items.Add(sp);
            else if (TryReadAffiliationDescriptor(reader, out var affiliation))
                entity.Items.Add(affiliation);
            else if (TryReadAttributeAuthorityDescriptor(reader, out var attributeAuthority))
                entity.Items.Add(attributeAuthority);
            else if (TryReadAuthnAuthorityDescriptor(reader, out var authnAuthority))
                entity.Items.Add(authnAuthority);
            else if (TryReadPdpDescriptor(reader, out var pdp)) 
                entity.Items.Add(pdp);
            else if (TryReadOrganization(reader, out var organization))
                entity.Organization = organization;
            else
                return TryReadSaml2MetadataChild(reader, entity);

            return true;
        }

        protected virtual bool TryReadSaml2MetadataChild(XmlDictionaryReader reader, Saml2Metadata metadata)
        {
            if (TryReadContactPerson(reader, out var contact))
                metadata.ContactPerson.Add(contact);
            else if (TryReadAdditionalMetadataLocation(reader, out var additionalMetadataLocation))
                metadata.AdditionalMetadataLocation.Add(additionalMetadataLocation);
            else
                return false;

            return true;
        }

        protected virtual bool TryReadRoleDescriptorChild(XmlDictionaryReader reader, RoleDescriptor role)
        {
            if (TryReadContactPerson(reader, out var contact))
                role.ContactPerson.Add(contact);
            else if (TryReadKeyDescriptor(reader, out var key))
                role.KeyDescriptors.Add(key);
            else if (TryReadOrganization(reader, out var organization))
                role.Organization = organization;
            else
                return false;

            return true;
        }

        protected virtual bool TryReadContactPersonChild(XmlDictionaryReader reader, ContactPerson contact)
        {
            if (TryReadElementContent(reader, Elements.Company, Saml2MetadataConstants.Namespace, out var company))
                contact.Company = company;
            else if (TryReadElementContent(reader, Elements.EmailAddress, Saml2MetadataConstants.Namespace, out var emailAddress))
                contact.EmailAddresses.Add(emailAddress);
            else if (TryReadElementContent(reader, Elements.TelephoneNumber, Saml2MetadataConstants.Namespace, out var telephoneNumber))
                contact.TelephoneNumbers.Add(telephoneNumber);
            else if (TryReadElementContent(reader, Elements.GivenName, Saml2MetadataConstants.Namespace, out var givenName))
                contact.GivenName = givenName;
            else if (TryReadElementContent(reader, Elements.SurName, Saml2MetadataConstants.Namespace, out var surName))
                contact.SurName = surName;
            else
                return false;

            return true;
        }

        protected virtual bool TryReadIdpSsoDescriptorChild(XmlDictionaryReader reader, IdpSsoDescriptor idp)
        {
            if (TryReadEndpoint(reader, Elements.SingleSignOnService, Saml2MetadataConstants.Namespace, out var singleSignOnService))
                idp.SingleSignOnService.Add(singleSignOnService);
            else if (TryReadEndpoint(reader, Elements.NameIdMappingService, Saml2MetadataConstants.Namespace, out var nameIdMappingService))
                idp.NameIdMappingService.Add(nameIdMappingService);
            else if (TryReadEndpoint(reader, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, out var assertionIdRequestService))
                idp.AssertionIdRequestService.Add(assertionIdRequestService);
            else if (TryReadElementContentAsUri(reader, Elements.AttributeProfile, Saml2MetadataConstants.Namespace, out var attributeProfile))
                idp.AttributeProfile.Add(attributeProfile);
            else if (TryReadSaml2Attribute(reader, out var saml2Attribute))
                idp.Attributes.Add(saml2Attribute);
            else
                return TryReadSsoDescriptorChild(reader, idp);

            return true;
        }

        protected virtual bool TryReadSpSsoDescriptorChild(XmlDictionaryReader reader, SpSsoDescriptor sp)
        {
            if (TryReadIndexedEndpoint(reader, Elements.AssertionConsumerService, Saml2MetadataConstants.Namespace, out var assertionConsumerService))
                sp.AssertionConsumerService.Add(assertionConsumerService);
            else if (TryReadAttributeConsumingService(reader, out var attributeConsumingService))
                sp.AttributeConsumingService.Add(attributeConsumingService);
            else
                return TryReadSsoDescriptorChild(reader, sp);

            return true;
        }

        protected virtual bool TryReadSsoDescriptorChild(XmlDictionaryReader reader, SsoDescriptor sso)
        {
            if (TryReadIndexedEndpoint(reader, Elements.ArtifactResolutionService, Saml2MetadataConstants.Namespace, out var artifactResolutionService))
                sso.ArtifactResolutionService.Add(artifactResolutionService);
            else if (TryReadEndpoint(reader, Elements.SingleLogoutService, Saml2MetadataConstants.Namespace, out var singleLogoutService))
                sso.SingleLogoutService.Add(singleLogoutService);
            else if (TryReadEndpoint(reader, Elements.ManageNameIdService, Saml2MetadataConstants.Namespace, out var manageNameIdService))
                sso.ManageNameIdService.Add(manageNameIdService);
            else if (TryReadElementContentAsUri(reader, Elements.NameIdFormat, Saml2MetadataConstants.Namespace, out var nameIdFormat))
                sso.NameIdFormat.Add(nameIdFormat);
            else
                return TryReadRoleDescriptorChild(reader, sso);

            return true;
        }

        protected virtual bool TryReadAuthnAuthorityDescriptorChild(XmlDictionaryReader reader, AuthnAuthorityDescriptor authority)
        {
            if (TryReadEndpoint(reader, Elements.AuthnQueryService, Saml2MetadataConstants.Namespace, out var authnQueryService))
                authority.AuthnQueryService.Add(authnQueryService);
            else if (TryReadEndpoint(reader, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, out var assertionIdRequestService))
                authority.AssertionIdRequestService.Add(assertionIdRequestService);
            else if (TryReadElementContentAsUri(reader, Elements.NameIdFormat, Saml2MetadataConstants.Namespace, out var nameIdFormat))
                authority.NameIdFormat.Add(nameIdFormat);
            else
                return TryReadRoleDescriptorChild(reader, authority);

            return true;
        }

        protected virtual bool TryReadAttributeAuthorityDescriptorChild(XmlDictionaryReader reader, AttributeAuthorityDescriptor authority)
        {
            if (TryReadEndpoint(reader, Elements.AttributeService, Saml2MetadataConstants.Namespace, out var attributeService))
                authority.AttributeService.Add(attributeService);
            else if (TryReadEndpoint(reader, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, out var assertionIdRequestService))
                authority.AssertionIdRequestService.Add(assertionIdRequestService);
            else if (TryReadElementContentAsUri(reader, Elements.NameIdFormat, Saml2MetadataConstants.Namespace, out var nameIdFormat))
                authority.NameIdFormat.Add(nameIdFormat);
            else if (TryReadSaml2Attribute(reader, out var attribute))
                authority.Attributes.Add(attribute);
            else if (TryReadElementContentAsUri(reader, Elements.AttributeProfile, Saml2MetadataConstants.Namespace, out var attributeProfile))
                authority.AttributeProfile.Add(attributeProfile);
            else
                return TryReadRoleDescriptorChild(reader, authority);

            return true;
        }

        protected virtual bool TryReadPdpDescriptorChild(XmlDictionaryReader reader, PdpDescriptor pdp)
        {
            if (TryReadEndpoint(reader, Elements.AuthzService, Saml2MetadataConstants.Namespace, out var authzService))
                pdp.AuthzService.Add(authzService);
            else if (TryReadEndpoint(reader, Elements.AssertionIdRequestService, Saml2MetadataConstants.Namespace, out var assertionIdRequestService))
                pdp.AssertionIdRequestService.Add(assertionIdRequestService);
            else if (TryReadElementContentAsUri(reader, Elements.NameIdFormat, Saml2MetadataConstants.Namespace, out var nameIdFormat))
                pdp.NameIdFormat.Add(nameIdFormat);
            else
                return TryReadRoleDescriptorChild(reader, pdp);

            return true;
        }

        protected virtual bool TryReadAffiliationDescriptorChild(XmlDictionaryReader reader, AffiliationDescriptor affiliation)
        {
            if (TryReadKeyDescriptor(reader, out var keyDescriptor))
                affiliation.KeyDescriptor.Add(keyDescriptor);
            else if (TryReadElementContentAsUri(reader, Elements.AffiliateMember, Saml2MetadataConstants.Namespace, out var affiliateMember))
                affiliation.AffiliateMember.Add(affiliateMember);
            else
                return false;

            return true;
        }
        #endregion Child elements

        #region Attributes

        protected virtual void WriteEntityDescriptorAttributes(XmlDictionaryWriter writer, EntityDescriptor entity)
        {
            AssertValidEntityId(entity.EntityId, nameof(EntityDescriptor.EntityId));
            writer.TryWriteAttributeValue(Attributes.EntityId, entity.EntityId);
            WriteSaml2MetadataAttributes(writer, entity);
        }

        protected virtual void WriteEntitiesDescriptorAttributes(XmlDictionaryWriter writer, EntitiesDescriptor entities)
        {
            if (!string.IsNullOrEmpty(entities.Name))
                writer.WriteAttributeString(Saml2Constants.Attributes.Name, entities.Name);
            WriteSaml2MetadataAttributes(writer, entities);
        }

        protected virtual void WriteSaml2MetadataAttributes(XmlDictionaryWriter writer, Saml2Metadata metadata)
            => WriteBaseAttributes(writer, metadata);

        protected virtual void WriteAffiliationDescriptorAttributes(XmlDictionaryWriter writer, AffiliationDescriptor affiliation)
        {
            AssertValidEntityId(affiliation.AffiliationOwnerId, nameof(AffiliationDescriptor.AffiliationOwnerId));
            _ = writer.TryWriteAttributeValue(Attributes.AffiliationOwnerId, affiliation.AffiliationOwnerId);
            WriteBaseAttributes(writer, affiliation);
        }

        protected virtual void WriteRoleDescriptorAttributes(XmlDictionaryWriter writer, RoleDescriptor role)
        {
            if (role.ErrorUrl != null)
                writer.TryWriteAttributeValue(Attributes.ErrorUrl, role.ErrorUrl);
            var protocolSupportEnumeration = string.Join(" ", role.ProtocolsSupported.Select(u => u.OriginalString));

            if (!writer.TryWriteAttributeValue(Attributes.ProtocolSupportEnumeration, protocolSupportEnumeration))
                throw new InvalidOperationException("No protocols defined for role.");

            WriteBaseAttributes(writer, role);
        }

        protected virtual void WriteSsoDescriptorAttributes(XmlDictionaryWriter writer, SsoDescriptor sso) 
            => WriteRoleDescriptorAttributes(writer, sso);

        protected virtual void WriteAuthnAuthorityDescriptorAttributes(XmlDictionaryWriter writer, AuthnAuthorityDescriptor authnAuthority) 
            => WriteRoleDescriptorAttributes(writer, authnAuthority);

        protected virtual void WritePdpDescriptorAttributes(XmlDictionaryWriter writer, PdpDescriptor pdp) 
            => WriteRoleDescriptorAttributes(writer, pdp);

        protected virtual void WriteAttributeAuthorityDescriptorAttributes(XmlDictionaryWriter writer, AttributeAuthorityDescriptor attributeAuthority) 
            => WriteRoleDescriptorAttributes(writer, attributeAuthority);

        protected virtual void WriteSpSsoDescriptorAttributes(XmlDictionaryWriter writer, SpSsoDescriptor sp)
        {
            _ = writer.TryWriteAttributeValue(Attributes.WantAssertionsSigned, sp.WantAssertionsSigned);
            _ = writer.TryWriteAttributeValue(Attributes.AuthnRequestsSigned, sp.AuthnRequestsSigned);
            WriteSsoDescriptorAttributes(writer, sp);
        }

        protected virtual void WriteIdpSsoDescriptorAttributes(XmlDictionaryWriter writer, IdpSsoDescriptor idp)
        {
            _ = writer.TryWriteAttributeValue(Attributes.WantAuthnRequestsSigned, idp.WantAuthnRequestsSigned);
            WriteSsoDescriptorAttributes(writer, idp);
        }

        protected virtual void WriteBaseAttributes(XmlDictionaryWriter writer, DescriptorBase descriptor)
        {
           _ = writer.TryWriteAttributeValue(Saml2Constants.Attributes.ID, descriptor.Id);
           _ = writer.TryWriteAttributeValue(Attributes.ValidUntil, descriptor.ValidUntil);
           _ = writer.TryWriteAttributeValue(Attributes.CacheDuration, descriptor.CacheDuration);
        }
        protected virtual void WriteContactPersonAttributes(XmlDictionaryWriter writer, ContactPerson contactPerson)
        {
            _ = writer.TryWriteAttributeValue(Attributes.ContactType, contactPerson.ContactType.ToString().ToLower());
        }

        protected virtual void WriteOrganizationAttributes(XmlDictionaryWriter writer, Organization organization)
        {
        }

        protected virtual void WriteKeyDescriptorAttributes(XmlDictionaryWriter writer, KeyDescriptor keyDescriptor)
        {
            _ = writer.TryWriteAttributeValue(Attributes.Use, keyDescriptor.Use?.ToString().ToLower());
        }

        protected virtual void WriteEndpointAttributes(XmlDictionaryWriter writer, string elementLocalName, Endpoint endpoint)
        {
            if (!writer.TryWriteAttributeValue(Attributes.Binding, endpoint.Binding))
                throw XmlWriterExceptionHelper.CreateRequiredAttributeMissingException(elementLocalName, Attributes.Binding);

            if (!writer.TryWriteAttributeValue(Attributes.Location, endpoint.Location))
                throw XmlWriterExceptionHelper.CreateRequiredAttributeMissingException(elementLocalName, Attributes.Location);

            _ = writer.TryWriteAttributeValue(Attributes.ResponseLocation, endpoint.ResponseLocation);
        }

        protected virtual void WriteIndexedEndpointAttributes(XmlDictionaryWriter writer, string elementLocalName, IndexedEndpoint indexedEndpoint)
        {
            WriteEndpointAttributes(writer, elementLocalName, indexedEndpoint);
            _ = writer.TryWriteAttributeValue(Attributes.Index, indexedEndpoint.Index);
            _ = writer.TryWriteAttributeValue(Attributes.IsDefault, indexedEndpoint.IsDefault);
        }

        protected virtual void WriteAttributeConsumingServiceAttributes(XmlDictionaryWriter writer, AttributeConsumingService attributeConsumingService)
        {
            _ = writer.TryWriteAttributeValue(Attributes.Index, attributeConsumingService.Index);
            _ = writer.TryWriteAttributeValue(Attributes.IsDefault, attributeConsumingService.IsDefault);
        }

        protected virtual void WriteAdditionalMetadataLocationAttributes(XmlDictionaryWriter writer, AdditionalMetadataLocation location)
        {
            if(!writer.TryWriteAttributeValue(Attributes.Namespace, location.Namespace))
                throw XmlWriterExceptionHelper.CreateRequiredAttributeMissingException(Elements.AdditionalMetadataLocation, Attributes.Namespace);
        }

        protected virtual void WriteLocalizedNameAttributes(XmlDictionaryWriter writer, string elementLocalName, LocalizedName localizedName)
        {
            if (!writer.TryWriteAttributeValue(Attributes.Lang, localizedName.Lang))
                throw XmlWriterExceptionHelper.CreateRequiredAttributeMissingException(elementLocalName, Attributes.Lang);
        }

        protected virtual void WriteLocalizedUriAttributes(XmlDictionaryWriter writer, string elementLocalName, LocalizedUri localizedUri)
        {
            if (!writer.TryWriteAttributeValue(Attributes.Lang, localizedUri.Lang))
                throw XmlWriterExceptionHelper.CreateRequiredAttributeMissingException(elementLocalName, Attributes.Lang);
        }

        protected virtual void WriteRequestedAttributeAttributes(XmlDictionaryWriter writer, RequestedSaml2Attribute requestedAttribute)
        {
            if (!writer.TryWriteAttributeValue(Saml2Constants.Attributes.Name, requestedAttribute.Name))
                throw XmlWriterExceptionHelper.CreateRequiredAttributeMissingException(Elements.RequestedAttribute, Saml2Constants.Attributes.Name);
            _ = writer.TryWriteAttributeValue(Attributes.IsRequired, requestedAttribute.IsRequired);
            _ = writer.TryWriteAttributeValue(Saml2Constants.Attributes.NameFormat, requestedAttribute.NameFormat);
            _ = writer.TryWriteAttributeValue(Saml2Constants.Attributes.FriendlyName, requestedAttribute.FriendlyName);
        }

        protected virtual void ReadIndexedEndpointAttributes(XmlDictionaryReader reader, IndexedEndpoint indexedEndpoint)
        {
            if (reader.TryReadAttribute(Attributes.Index, out var indexStr) && ushort.TryParse(indexStr, out var index))
                indexedEndpoint.Index = index;

            if (reader.TryReadAttribute(Attributes.IsDefault, out var isDefaultStr) && bool.TryParse(isDefaultStr, out var isDefault))
                indexedEndpoint.IsDefault = isDefault;

            ReadEndpointAttributes(reader, indexedEndpoint);
        }

        protected virtual void ReadEndpointAttributes(XmlDictionaryReader reader, Endpoint endpoint)
        {
            if (reader.TryReadAttributeAsUri(Attributes.Binding, out var binding))
                endpoint.Binding = binding;

            if (reader.TryReadAttributeAsUri(Attributes.Location, out var location))
                endpoint.Location = location;

            if (reader.TryReadAttributeAsUri(Attributes.ResponseLocation, out var responseLocation))
                endpoint.ResponseLocation = responseLocation;
        }

        protected virtual void ReadEntityDescriptorAttributes(XmlDictionaryReader reader, EntityDescriptor entity)
        {
            if (reader.TryReadAttributeAsUri(Attributes.EntityId, out var entityId))
                entity.EntityId = entityId;

            ReadSaml2MetadataAttributes(reader, entity);
        }
        protected virtual void ReadEntitesDescriptorAttributes(XmlDictionaryReader reader, EntitiesDescriptor entities)
        {
            if (reader.TryReadAttribute(Saml2Constants.Attributes.Name, out var name))
                entities.Name = name;

            ReadSaml2MetadataAttributes(reader, entities);
        }

        protected virtual void ReadSaml2MetadataAttributes(XmlDictionaryReader reader, Saml2Metadata metadata) 
            => ReadBaseAttributes(reader, metadata);

        protected virtual void ReadIdpSsoDescriptorAttributes(XmlDictionaryReader reader, IdpSsoDescriptor idp)
        {
            if (reader.TryReadAttribute(Attributes.WantAuthnRequestsSigned, out var wantAuthnRequestSignedValue) && bool.TryParse(wantAuthnRequestSignedValue, out var wantAuthnRequestSigned))
                idp.WantAuthnRequestsSigned = wantAuthnRequestSigned;

            ReadSsoDescriptorAttributes(reader, idp);
        }

        protected virtual void ReadSpSsoDescriptorAttributes(XmlDictionaryReader reader, SpSsoDescriptor sp)
        {
            if (reader.TryReadAttribute(Attributes.AuthnRequestsSigned, out var authnRequestsSignedValue) && bool.TryParse(authnRequestsSignedValue, out var authnRequestsSigned))
                sp.AuthnRequestsSigned = authnRequestsSigned;

            if (reader.TryReadAttribute(Attributes.WantAssertionsSigned, out var wantAssertionsSignedValue) && bool.TryParse(wantAssertionsSignedValue, out var wantAssertionsSigned))
                sp.WantAssertionsSigned = wantAssertionsSigned;

            ReadSsoDescriptorAttributes(reader, sp);
        }

        protected virtual void ReadSsoDescriptorAttributes(XmlDictionaryReader reader, SsoDescriptor sso) 
            => ReadRoleDescriptorAttributes(reader, sso);

        protected virtual void ReadRoleDescriptorAttributes(XmlDictionaryReader reader, RoleDescriptor role)
        {
            if(reader.TryReadAttribute(Attributes.ProtocolSupportEnumeration, out var protocolSupportEnumeration))
            {
                var list = protocolSupportEnumeration
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(v => Uri.IsWellFormedUriString(v, UriKind.Absolute))
                    .Select(v => new Uri(v))
                    .ToList()
                ; 
                role.ProtocolsSupported = list;
            }

            if (reader.TryReadAttributeAsUri(Attributes.ErrorUrl, out var errorUrl))
                role.ErrorUrl = errorUrl;

            ReadBaseAttributes(reader, role);
        }

        protected virtual void ReadAttributeAuthorityDescriptorAttributes(XmlDictionaryReader reader, AttributeAuthorityDescriptor authority)
            => ReadRoleDescriptorAttributes(reader, authority);

        protected virtual void ReadPdpDescriptorAttributes(XmlDictionaryReader reader, PdpDescriptor pdp)
            => ReadRoleDescriptorAttributes(reader, pdp);

        protected virtual void ReadAffiliationDescriptorAttributes(XmlDictionaryReader reader, AffiliationDescriptor affiliation)
        {
            if (reader.TryReadAttributeAsUri(Attributes.AffiliationOwnerId, out var affiliationOwnerId))
                affiliation.AffiliationOwnerId = affiliationOwnerId;
            ReadBaseAttributes(reader, affiliation);
        }

        protected virtual void ReadBaseAttributes(XmlDictionaryReader reader, DescriptorBase descriptor)
        {
            if (reader.TryReadAttribute(Saml2Constants.Attributes.ID, out var id))
                descriptor.Id = id;
            if (reader.TryReadAttributeAsTimeSpan(Attributes.CacheDuration, out var cacheDuration))
                descriptor.CacheDuration = cacheDuration;
            if (reader.TryReadAttributeAsDateTime(Attributes.ValidUntil, out var validUntil))
                descriptor.ValidUntil = validUntil;
        }

        protected virtual void ReadAuthnAuthorityDescriptorAttributes(XmlDictionaryReader reader, AuthnAuthorityDescriptor authority)
            => ReadRoleDescriptorAttributes(reader, authority);

        #endregion Attributes

        #region Utility
        protected virtual bool TryReadElementContent(XmlDictionaryReader reader, string localName, string ns, out string value)
        {
            if (!reader.IsStartElement(localName, ns)) return Out.False(out value);

            value = reader.ReadElementContentAsString();
            return true;
        }

        protected virtual bool TryReadElementContentAsUri(XmlDictionaryReader reader, string localName, string ns, out Uri value)
        {
            if (!reader.IsStartElement(localName, ns)) return Out.False(out value);

            var content = reader.ReadElementContentAsUri();
            value = content;
            return true;
        }

        protected virtual bool TryReadLocalizedName(XmlDictionaryReader reader, string localName, string ns, out LocalizedName value)
        {
            if (!reader.IsStartElement(localName, ns)) return Out.False(out value);
            var lang = reader.GetAttribute(Attributes.Lang);
            if (string.IsNullOrWhiteSpace(lang)) return Out.False(out value);

            var content = reader.ReadElementContentAsString();
            var localized = new LocalizedName
            {
                Lang = lang,
                Value = content
            };
            value = localized;
            return true;
        }

        protected virtual bool TryReadLocalizedUri(XmlDictionaryReader reader, string localName, string ns, out LocalizedUri value)
        {
            if (!reader.IsStartElement(localName, ns)) return Out.False(out value);
            var lang = reader.GetAttribute(Attributes.Lang);
            if (string.IsNullOrWhiteSpace(lang)) return Out.False(out value);

            var content = reader.ReadElementContentAsUri();
            var localized = new LocalizedUri
            {
                Lang = lang,
                Value = content
            };
            value = localized;
            return true;
        }

        protected virtual void AssertValidEntityId(Uri value, string parameterName)
        {
            if (!IsValidEntityId(value))
                throw new ArgumentException($"Value '{value}' is not a valid entityId.", parameterName);
        }

        protected virtual bool IsValidEntityId(Uri value)
        {
            if (value == null) return false;
            if (!value.IsAbsoluteUri) return false;
            var absolute = value.AbsoluteUri;
            return absolute.Length <= 1024;
        }

        #endregion
    }
}
