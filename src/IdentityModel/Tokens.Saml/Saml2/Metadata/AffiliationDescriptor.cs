using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class AffiliationDescriptor : DescriptorBase
    {
        public ICollection<Uri> AffiliateMember { get; } = new List<Uri>();
        public ICollection<KeyDescriptor> KeyDescriptor { get; } = new List<KeyDescriptor>();
        public Uri AffiliationOwnerId { get; set; }
    }
}
