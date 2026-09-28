using System;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Solid.IdentityModel.Protocols.WsSecurity;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>A WS-Trust lifecycle target containing either a token or a WS-Security reference.</summary>
    public class TokenTarget
    {
        public TokenTarget(XmlElement tokenElement) => TokenElement = tokenElement ?? throw LogHelper.LogArgumentNullException(nameof(tokenElement));
        public TokenTarget(SecurityTokenReference reference) => Reference = reference ?? throw LogHelper.LogArgumentNullException(nameof(reference));

        public XmlElement TokenElement { get; }
        public SecurityTokenReference Reference { get; }
    }
}
