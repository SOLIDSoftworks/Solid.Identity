using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class DescriptorBase
    {
        public string Id { get; set; }
        public DateTime? ValidUntil { get; set; }
        public TimeSpan? CacheDuration { get; set; }
        public Signature Signature { get; set; }
    }
}
