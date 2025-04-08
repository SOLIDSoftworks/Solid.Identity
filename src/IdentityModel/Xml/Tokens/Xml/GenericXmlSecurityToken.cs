using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Solid.IdentityModel.Tokens.Xml
{
    /// <summary>
    /// A <see cref="SecurityToken"/> implementation for a generic XML element.
    /// </summary>
    public class GenericXmlSecurityToken : SecurityToken
    {
        /// <summary>
        /// Creates an instance of <see cref="GenericXmlSecurityToken"/>.
        /// </summary>
        /// <param name="element">The <see cref="XmlElement"/> that contains the security token.</param>
        /// <param name="validFrom">The <see cref="DateTime"/> when the security token starts being valid.</param>
        /// <param name="validTo">The <see cref="DateTime"/> when the security token expires.</param>
        /// <param name="securityKey">The <see cref="Microsoft.IdentityModel.Tokens.SecurityKey"/> associated with the security token.</param>
        /// <param name="issuer">The issuer of the security token.</param>
        /// <param name="internalTokenReference">The internal token reference for this security token.</param>
        /// <param name="externalTokenReference">The external token reference for this security token.</param>
        public GenericXmlSecurityToken(XmlElement element, DateTime validFrom, DateTime validTo,
                                       SecurityKey securityKey = null,
                                       string issuer = null,
                                       GenericXmlSecurityKeyIdentifierClause internalTokenReference = null,
                                       GenericXmlSecurityKeyIdentifierClause externalTokenReference = null)
        {
            Id = Guid.NewGuid().ToString();
            ValidFrom = validFrom;
            ValidTo = validTo;
            Element = element ?? throw new ArgumentNullException(nameof(element));
            SecurityKey = securityKey;
            Issuer = issuer;
            InternalTokenReference = internalTokenReference;
            ExternalTokenReference = externalTokenReference;
        }

        /// <summary>
        /// The id of the security token. 
        /// </summary>
        /// <value><see cref="Guid.NewGuid"/></value>
        public override string Id { get; }

        /// <summary>
        /// The issuer of the security token. 
        /// </summary>
        public override string Issuer { get; }

        /// <summary>
        /// The security token as a security key.
        /// <para>Since we can't parse the token, this value is always null.</para>
        /// </summary>
        public override SecurityKey SecurityKey { get; }

        /// <summary>
        /// The issuer signing key of the security token. 
        /// <para>Since we can't parse the token, this value is always null.</para>
        /// </summary>
        public override SecurityKey SigningKey { get; set; }

        /// <summary>
        /// The date where the security token starts being valid.
        /// </summary>
        public override DateTime ValidFrom { get; }

        /// <summary>
        /// The date where the security token expires.
        /// </summary>
        public override DateTime ValidTo { get; }

        /// <summary>
        /// The security token element.
        /// </summary>
        public XmlElement Element { get; }

        /// <summary>
        /// The internal token reference.
        /// </summary>
        public GenericXmlSecurityKeyIdentifierClause InternalTokenReference { get; }

        /// <summary>
        /// The external token reference.
        /// </summary>
        public GenericXmlSecurityKeyIdentifierClause ExternalTokenReference { get; }
    }
}
