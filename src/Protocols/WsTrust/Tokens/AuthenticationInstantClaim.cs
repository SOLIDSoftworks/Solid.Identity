using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Xml;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Solid.Identity.Tokens
{
    public static class AuthenticationInstantClaim
    {
        public static Claim Now => new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.UtcNow, "yyyy-MM-ddTHH:mm:ss.fffZ"), ClaimValueTypes.DateTime);
    }
}
