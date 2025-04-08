using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Solid.Identity.Protocols.WsTrust
{
    public class WsTrustSecurityTokenDescriptor : RequestedSecurityTokenDescriptor
    {
        public SecurityToken Token { get; set; }
        public SecurityTokenReference AttachedReference { get; set; }
        public SecurityTokenReference UnattachedReference { get; set; }
        public XmlElement TokenElement { get; set; }

        public virtual void ApplyTo(RequestSecurityTokenResponse response)
        {
            if (TokenType != null)
                response.TokenType = TokenType;

            if (TokenElement != null)
            {
                response.RequestedSecurityToken = new RequestedSecurityToken(TokenElement)
                {
                    SecurityToken = Token
                };
            }
            else if (Token != null)
            {
                response.RequestedSecurityToken = new RequestedSecurityToken(Token);
            }

            if (AttachedReference != null)
                response.AttachedReference = AttachedReference;

            if (UnattachedReference != null)
                response.UnattachedReference = UnattachedReference;

            if (IssuedAt != null && Expires != null)
                response.Lifetime = new Lifetime(IssuedAt, Expires);
        }
    }
}
