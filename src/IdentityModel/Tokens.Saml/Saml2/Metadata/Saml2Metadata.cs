using System.Collections.Generic;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class Saml2Metadata : DescriptorBase
    {
        public ICollection<ContactPerson> ContactPerson { get; } = new List<ContactPerson>();
        public ICollection<AdditionalMetadataLocation> AdditionalMetadataLocation { get; } = new List<AdditionalMetadataLocation>();
        public ICollection<DescriptorBase> Items { get; } = new List<DescriptorBase>();
    }
}
