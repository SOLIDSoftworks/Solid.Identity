using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.ServiceModel
{
    internal class IssuedTokenClientCredential
    {
        public SecurityToken Token { get; set; }
        public SecurityTokenHandler[] Handlers { get; set; }
    }
}
