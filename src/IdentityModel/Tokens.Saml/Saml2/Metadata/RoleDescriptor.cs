using System;
using System.Collections.Generic;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class RoleDescriptor : DescriptorBase
    {
        public Organization Organization { get; set; }
        public ICollection<ContactPerson> ContactPerson { get; } = new List<ContactPerson>();
        public ICollection<KeyDescriptor> KeyDescriptors { get; } = new List<KeyDescriptor>();
        public ICollection<Uri> ProtocolsSupported { get; internal set; } = new List<Uri>();
        public Uri ErrorUrl { get; set; }
    }
}
