using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Solid.IdentityModel.Tokens.Xml
{
    /// <summary>
    /// A generic xml security key identifier clause.
    /// </summary>
    public class GenericXmlSecurityKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        /// <summary>
        /// Creates an instance of <see cref="GenericXmlSecurityKeyIdentifierClause"/>.
        /// </summary>
        /// <param name="id">The id of the security key identifier clause.</param>
        /// <param name="element">The element containing the security key identifier clause.</param>
        public GenericXmlSecurityKeyIdentifierClause(string id, XmlElement element)
        {
            Id = id;
            Element = element;
        }

        /// <summary>
        /// The id of the security key identifier clause.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The element containing the security key identifier clause.
        /// </summary>
        public XmlElement Element { get; }
    }
}
