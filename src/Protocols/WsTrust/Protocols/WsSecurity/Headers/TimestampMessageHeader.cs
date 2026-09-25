using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Protocols;
using Solid.IdentityModel.Protocols.WsSecurity;

namespace Solid.Identity.Protocols.WsSecurity.Headers
{
    class TimestampMessageHeader : MessageHeader
    {
        private DateTime _created;
        private DateTime _expires;

        public TimestampMessageHeader(DateTime created, DateTime expires)
        {
            _created = created;
            _expires = expires;
        }

        public override string Name => "Security";

        public override string Namespace => WsSecurityConstants.WsSecurity10.Namespace;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement(WsSecurityUtilityElements.Timestamp, WsSecurityUtilityConstants.SecurityUtility10.Namespace);
            writer.WriteAttributeString(WsSecurityUtilityConstants.SecurityUtility10.DefaultPrefix, WsSecurityUtilityAttributes.Id, WsSecurityUtilityConstants.SecurityUtility10.Namespace, "_0");
            writer.WriteStartElement(WsSecurityUtilityElements.Created, WsSecurityUtilityConstants.SecurityUtility10.Namespace);
            writer.WriteString(XmlConvert.ToString(_created.ToUniversalTime(), XmlDateTimeSerializationMode.Utc));
            writer.WriteEndElement();
            writer.WriteStartElement(WsSecurityUtilityElements.Expires, WsSecurityUtilityConstants.SecurityUtility10.Namespace);
            writer.WriteString(XmlConvert.ToString(_expires.ToUniversalTime(), XmlDateTimeSerializationMode.Utc));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
