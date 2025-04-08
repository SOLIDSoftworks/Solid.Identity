using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Solid.IdentityModel.FederationMetadata.WsAddressing
{
    public static class WsAddressingConstants
    {
        public static readonly string Namespace = "http://www.w3.org/2005/08/addressing";
        public static readonly string Prefix = "wsa";
        public static class Elements
        {
            public static readonly string EndpointReference = nameof(EndpointReference);
            public static readonly string Address = nameof(Address);
        }

        public static class Attributes
        {

        }
    }
}
