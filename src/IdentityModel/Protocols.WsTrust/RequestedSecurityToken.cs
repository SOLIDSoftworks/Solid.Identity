using System;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Protocols.WsTrust.WsSecureConversion;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// Represents the contents of a RequestedSecurityToken element.
    /// <see cref="RequestedSecurityToken"/> represents the security token returned in a WsTrust response.
    /// see: http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html
    /// </summary>
    public class RequestedSecurityToken
    {
        private SecurityContextToken _securityContextToken;
        private SecurityToken _securityToken;
        private XmlElement _xmlElement;

        /// <summary>
        /// Creates an instance of <see cref="RequestedSecurityToken"/>.
        /// This constructor is useful when deserializing from a stream such as xml.
        /// </summary>
        public RequestedSecurityToken()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlElement"></param>
        public RequestedSecurityToken(XmlElement xmlElement)
        {
            _xmlElement = xmlElement;
        }

        /// <summary>
        /// Creates an instance of <see cref="RequestedSecurityToken"/>.
        /// </summary>
        /// <param name="securityToken">a <see cref="SecurityToken"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="securityToken"/> is null.</exception>
        public RequestedSecurityToken(SecurityToken securityToken)
        {
            SecurityToken = securityToken;
        }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <exception cref="ArgumentNullException">if TokenElement is null.</exception>
        public XmlElement TokenElement
        {
            get => _xmlElement;
            set => _xmlElement = value ?? throw LogHelper.LogArgumentNullException(nameof(TokenElement));
        }

        /// <summary>
        /// Gets or set the <see cref="SecurityToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">if SecurityToken is null.</exception>
        public SecurityToken SecurityToken
        {
            get => _securityToken;
            set => _securityToken = value ?? throw LogHelper.LogArgumentNullException(nameof(SecurityToken));
        }

        /// <summary>
        /// Gets or set the <see cref="SecurityContextToken"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">if SecurityContextToken is null.</exception>
        public SecurityContextToken SecurityContextToken
        {
            get => _securityContextToken;
            set => _securityContextToken = value ?? throw LogHelper.LogArgumentNullException(nameof(SecurityContextToken));
        }
    }
}
