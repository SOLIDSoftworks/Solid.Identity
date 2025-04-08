using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public static class Saml2MetadataConstants
    {
        public static readonly string Namespace = "urn:oasis:names:tc:SAML:2.0:metadata";
        public static readonly string Prefix = "md";

        public static class Attributes
        {
            public static readonly string EntityId = "entityID";
            public static readonly string CacheDuration = "cacheDuration";
            public static readonly string ValidUntil = "validUntil";
            public static readonly string ContactType = "contactType";
            public static readonly string ErrorUrl = "errorURL";
            public static readonly string ProtocolSupportEnumeration = "protocolSupportEnumeration";
            public static readonly string Use = "use";
            public static readonly string Binding = nameof(Binding);
            public static readonly string Location = nameof(Location);
            public static readonly string ResponseLocation = nameof(ResponseLocation);
            public static readonly string Index = "index";
            public static readonly string IsDefault = "isDefault";
            public static readonly string WantAuthnRequestsSigned = nameof(WantAuthnRequestsSigned);
            public static readonly string AuthnRequestsSigned = nameof(AuthnRequestsSigned);
            public static readonly string WantAssertionsSigned = nameof(WantAssertionsSigned);
            public static readonly string Namespace = "namespace";
            public static readonly string AffiliationOwnerId = "affiliationOwnerID";
            public static readonly string Lang = "lang";
            public static readonly string IsRequired = "isRequired";
        }

        public static class Elements
        {
            public static readonly string EntityDescriptor = nameof(EntityDescriptor);
            public static readonly string EntitiesDescriptor = nameof(EntitiesDescriptor);
            public static readonly string RoleDescriptor = nameof(RoleDescriptor);
            public static readonly string SpSsoDescriptor = "SPSSODescriptor";
            public static readonly string IdpSsoDescriptor = "IDPSSODescriptor";
            public static readonly string Company = nameof(Company);
            public static readonly string GivenName = nameof(GivenName);
            public static readonly string SurName = nameof(SurName);
            public static readonly string EmailAddress = nameof(EmailAddress);
            public static readonly string TelephoneNumber = nameof(TelephoneNumber);
            public static readonly string ContactPerson = nameof(ContactPerson);
            public static readonly string KeyDescriptor = nameof(KeyDescriptor);
            public static readonly string Organization = nameof(Organization);
            public static readonly string OrganizationName = nameof(OrganizationName);
            public static readonly string OrganizationDisplayName = nameof(OrganizationDisplayName);
            public static readonly string OrganizationUrl = "OrganizationURL";
            public static readonly string ArtifactResolutionService = nameof(ArtifactResolutionService);
            public static readonly string SingleLogoutService = nameof(SingleLogoutService);
            public static readonly string ManageNameIdService = "ManageNameIDService";
            public static readonly string NameIdFormat = "NameIDFormat";
            public static readonly string SingleSignOnService = nameof(SingleSignOnService);
            public static readonly string NameIdMappingService = "NameIDMappingService";
            public static readonly string AssertionIdRequestService = "AssertionIDRequestService";
            public static readonly string AttributeProfile = nameof(AttributeProfile);
            public static readonly string AssertionConsumerService = nameof(AssertionConsumerService);
            public static readonly string AttributeConsumingService = nameof(AttributeConsumingService);
            public static readonly string ServiceName = nameof(ServiceName);
            public static readonly string ServiceDescription = nameof(ServiceDescription);
            public static readonly string RequestedAttribute = nameof(RequestedAttribute);
            public static readonly string AdditionalMetadataLocation = nameof(AdditionalMetadataLocation);
            public static readonly string AuthnAuthorityDescriptor = nameof(AuthnAuthorityDescriptor);
            public static readonly string PdpDescriptor = "PDPDescriptor";
            public static readonly string AttributeAuthorityDescriptor = nameof(AttributeAuthorityDescriptor);
            public static readonly string AffiliationDescriptor = nameof(AffiliationDescriptor);
            public static readonly string AuthnQueryService = nameof(AuthnQueryService);
            public static readonly string AuthzService = nameof(AuthzService);
            public static readonly string AttributeService = nameof(AttributeService);
            public static readonly string AffiliateMember = nameof(AffiliateMember);
        }
    }
}
