using System;
using System.Collections.Generic;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class SsoDescriptor : RoleDescriptor
    {
        public ICollection<IndexedEndpoint> ArtifactResolutionService { get; } = new List<IndexedEndpoint>();
        public ICollection<Endpoint> ManageNameIdService { get; } = new List<Endpoint>();
        public ICollection<Endpoint> SingleLogoutService { get; } = new List<Endpoint>();
        public ICollection<Uri> NameIdFormat { get; } = new List<Uri>();
    }
}
