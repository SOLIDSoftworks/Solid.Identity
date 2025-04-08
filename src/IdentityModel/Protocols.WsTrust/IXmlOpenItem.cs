using System.Collections.Generic;
using System.Xml;

namespace Solid.IdentityModel.Protocols.WsTrust
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
        IList<XmlAttribute> AdditionalXmlAttributes { get; }
    }
}
