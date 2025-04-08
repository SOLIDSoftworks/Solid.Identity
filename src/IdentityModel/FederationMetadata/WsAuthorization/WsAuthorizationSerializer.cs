using Solid.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using static Solid.IdentityModel.FederationMetadata.WsAuthorization.WsAuthorizationConstants;

namespace Solid.IdentityModel.FederationMetadata.WsAuthorization
{
    public class WsAuthorizationSerializer
    {
        public virtual bool TryReadClaimType(XmlDictionaryReader reader, out ClaimType claimType)
        {
            if (!reader.IsStartElement(Elements.ClaimType, Namespace)) return Out.False(out claimType);

            if (!reader.TryReadAttributeAsUri(Attributes.Uri, out var uri))
                throw new InvalidOperationException($"Element {Elements.ClaimType} must have a valid {Attributes.Uri} attribute.");

            var type = new ClaimType(uri);
            if (reader.TryReadAttributeAsBoolean(Attributes.Optional, out var optional))
                type.Optional = optional;

            reader.ForEachChild(r =>
            {
                if (r.TryReadElementContent(Elements.DisplayName, Namespace, out var displayName))
                    type.DisplayName = displayName;
                else if (r.TryReadElementContent(Elements.Description, Namespace, out var description))
                    type.Description = description;
                else if (r.TryReadElementContent(Elements.DisplayValue, Namespace, out var displayValue))
                    type.DisplayValue = displayValue;
                else if (r.TryReadElementContent(Elements.Value, Namespace, out var value))
                    type.Value = value;
                else 
                    return false;
                return true;
            });

            claimType = type;
            return true;
        }

        public virtual void WriteClaimType(XmlDictionaryWriter writer, ClaimType claimType)
        {
            if (claimType == null) return;

            writer.WriteStartElement(Elements.ClaimType, Namespace);
            if(!writer.TryWriteAttributeValue(Attributes.Uri, claimType.Uri))
                throw XmlWriterExceptionHelper.CreateRequiredAttributeMissingException(Elements.ClaimType, Attributes.Uri);
            _ = writer.TryWriteAttributeValue(Attributes.Optional, claimType.Optional);

            _ = writer.TryWriteElementValue(Elements.DisplayName, Namespace, claimType.DisplayName);
            _ = writer.TryWriteElementValue(Elements.Description, Namespace, claimType.Description);
            _ = writer.TryWriteElementValue(Elements.DisplayValue, Namespace, claimType.DisplayValue);

            _ = writer.TryWriteElementValue(Elements.Value, Namespace, claimType.Value);

            writer.WriteEndElement();
        }
    }
}
