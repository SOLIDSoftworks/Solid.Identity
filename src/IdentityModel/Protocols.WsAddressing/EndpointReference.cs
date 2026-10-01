using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.IdentityModel.Logging;

#pragma warning disable 1591
namespace Solid.IdentityModel.Protocols.WsAddressing
{
    public class EndpointReference : XmlOpenItem
    {
        public string Uri { get; set; } = null!;
    }
}
