using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata
{
    public class LogicalServiceNamesOffered
    {
        public ICollection<Uri> IssuerName { get; } = new List<Uri>();
    }
}
