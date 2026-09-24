using System.Collections.Generic;
using System.Xml;
using Microsoft.IdentityModel.Xml;

namespace Solid.IdentityModel.Protocols
{
    /// <summary>
    /// Defines an interface for handling additional elements and attributes
    /// </summary>
    public interface IXmlOpenItem
    {
        /// <summary>
        /// 
        /// </summary>
        IList<XmlElement> AdditionalXmlElements { get; }

        /// <summary>
        /// 
        /// </summary>
        IList<XmlAttributeDescriptor> AdditionalXmlAttributes { get; }
    }
}
