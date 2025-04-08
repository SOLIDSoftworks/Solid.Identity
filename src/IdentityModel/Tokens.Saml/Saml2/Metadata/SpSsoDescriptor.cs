using System.Collections.Generic;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class SpSsoDescriptor : SsoDescriptor
    {
        public ICollection<IndexedEndpoint> AssertionConsumerService { get; } = new List<IndexedEndpoint>();
        public ICollection<AttributeConsumingService> AttributeConsumingService { get; } = new List<AttributeConsumingService>();
        public bool? AuthnRequestsSigned { get; set; }
        public bool? WantAssertionsSigned { get; set; }
    }

    public class AttributeConsumingService
    {
        public ushort Index { get; set; }
        public bool? IsDefault { get; set; }
        public ICollection<LocalizedName> Name { get; } = new List<LocalizedName>();
        public ICollection<LocalizedName> Description { get; } = new List<LocalizedName>();
        public ICollection<RequestedSaml2Attribute> RequestedAttribute { get; } = new List<RequestedSaml2Attribute>();
    }
}
