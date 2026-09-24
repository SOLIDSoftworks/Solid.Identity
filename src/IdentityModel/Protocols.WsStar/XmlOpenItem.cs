using System.Collections.Generic;
using System.Xml;
using Microsoft.IdentityModel.Xml;

namespace Solid.IdentityModel.Protocols;

public class XmlOpenItem : IXmlOpenItem
{
    /// <summary>
    /// Gets additional attributes that should be added to or were found when reading or writing a trust message.
    /// </summary>
    public IList<XmlAttributeDescriptor> AdditionalXmlAttributes { get; } = new List<XmlAttributeDescriptor>();

    /// <summary>
    /// Gets additional elements that should be added to or were found when reading or writing a trust message.
    /// </summary>
    public IList<XmlElement> AdditionalXmlElements { get; } = new List<XmlElement>();
}