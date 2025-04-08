using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class EntityDescriptor : Saml2Metadata
    {
        // entityId
        public Uri EntityId { get; set; }
        public Organization Organization { get; set; }
    }

    public class AttributeAuthorityDescriptor : RoleDescriptor
    {
        public ICollection<Endpoint> AttributeService { get; } = new List<Endpoint>();
        public ICollection<Endpoint> AssertionIdRequestService { get; } = new List<Endpoint>();
        public ICollection<Uri> NameIdFormat { get; } = new List<Uri>();
        public ICollection<Uri> AttributeProfile { get; } = new List<Uri>();
        public ICollection<Saml2Attribute> Attributes { get; } = new List<Saml2Attribute>();
    }

    public class AuthnAuthorityDescriptor : RoleDescriptor
    {
        public ICollection<Endpoint> AuthnQueryService { get; } = new List<Endpoint>();
        public ICollection<Endpoint> AssertionIdRequestService { get; } = new List<Endpoint>();
        public ICollection<Uri> NameIdFormat { get; } = new List<Uri>();
    }

    public class PdpDescriptor : RoleDescriptor
    {
        public ICollection<Endpoint> AuthzService { get; } = new List<Endpoint>();
        public ICollection<Endpoint> AssertionIdRequestService { get; } = new List<Endpoint>();
        public ICollection<Uri> NameIdFormat { get; } = new List<Uri>();
    }

    public class KeyDescriptor
    {
        public KeyUse? Use { get; set; }
        public KeyInfo KeyInfo { get; set; }

        // EncryptionMethod needed here
    }

    public enum KeyUse
    {
        Encryption,
        Signing
    }

    public class AdditionalMetadataLocation
    {
        public Uri Namespace { get; set; }
        public Uri Value { get; set; }
    }

    public class Organization
    {
        public ICollection<LocalizedName> Name { get; } = new List<LocalizedName>();
        public ICollection<LocalizedName> DisplayName { get; } = new List<LocalizedName>();
        public ICollection<LocalizedUri> Url { get; } = new List<LocalizedUri>();
    }
}
