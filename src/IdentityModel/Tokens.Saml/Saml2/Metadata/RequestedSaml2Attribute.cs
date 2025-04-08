using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class RequestedSaml2Attribute : Saml2Attribute
    {
        public RequestedSaml2Attribute(string name) 
            : base(name)
        {

        }

        public bool? IsRequired { get; set; }
    }
}
