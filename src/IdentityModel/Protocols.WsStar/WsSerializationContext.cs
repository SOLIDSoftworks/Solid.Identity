using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsFederation;
using Solid.IdentityModel.Protocols.WsSecureConversation;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols
{
    /// <summary>
    /// Used to remember the prefix, namespace to use / expect when reading and writing WsTrust Requests and Responses.
    /// </summary>
    public class WsSerializationContext
    {
        public WsSerializationContext()
        {
        }

        public WsSerializationContext(AddressingVersion addressingVersion, MessageSecurityVersion messageSecurityVersion)
        {
            if (addressingVersion == null)
                throw new ArgumentNullException(nameof(addressingVersion));
            if (messageSecurityVersion == null)
                throw new ArgumentNullException(nameof(messageSecurityVersion));

            AddressingVersion = addressingVersion;
            MessageSecurityVersion = messageSecurityVersion;
            Federation = WsFederationConstants.Federation12;
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10;
            
            if (messageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                Trust = WsTrustConstants.TrustFeb2005;
            else if (messageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                Trust = WsTrustConstants.Trust13;
            else
                throw new NotSupportedException($"Unsupported WS-Trust version: {messageSecurityVersion.TrustVersion}");
            
            if (addressingVersion == AddressingVersion.WSAddressing10)
                Addressing = WsAddressingConstants.Addressing10;
            else if (addressingVersion == AddressingVersion.WSAddressingAugust2004)
                Addressing = WsAddressingConstants.Addressing200408;
            else
                throw new NotSupportedException($"Unsupported WS-Addressing version: {addressingVersion}");

            if (messageSecurityVersion.SecurityVersion == SecurityVersion.WSSecurity10)
                Security = WsSecurityConstants.WsSecurity10;
            else if (messageSecurityVersion.SecurityVersion == SecurityVersion.WSSecurity11)
                Security = WsSecurityConstants.WsSecurity11;
            else
                throw new NotSupportedException($"Unsupported WS-Security version: {messageSecurityVersion.SecurityVersion}");
            
            if (WsSecureConversationConstants.KnownNamespaces.TryGetValue(messageSecurityVersion.SecureConversationVersion.Namespace.ToString(), out var secureConversation))
            {
                SecureConversation = secureConversation;
            }
            else
            {
                SecureConversation = new WsSecureConversationConstants
                {
                    DefaultPrefix = "sc",
                    Namespace = messageSecurityVersion.SecureConversationVersion.Namespace.ToString()
                };
            }
            
            if (WsSecurityPolicyConstants.KnownNamespaces.TryGetValue(messageSecurityVersion.SecurityPolicyVersion.Namespace, out var securityPolicy))
            {
                SecurityPolicy = securityPolicy;
            }
            else
            {
                SecurityPolicy = new WsSecurityPolicyConstants
                {
                    DefaultPrefix = "wsp",
                    Namespace = messageSecurityVersion.SecurityPolicyVersion.Namespace
                };
            }
        }
        
        public WsSerializationContext(WsTrustConstants trustVersion)
        {
            Trust = trustVersion ?? throw new System.ArgumentNullException(nameof(trustVersion));
            TrustActions = trustVersion.Actions;
            TrustKeyTypes = trustVersion.KeyTypes;
            AddressingVersion = AddressingVersion.WSAddressing10;
            Addressing = WsAddressingConstants.Addressing10;
            Federation = WsFederationConstants.Federation12;
            SecurityPolicy = WsSecurityPolicyConstants.SecurityPolicy12;
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10;
            Security = trustVersion == WsTrustConstants.TrustFeb2005
                ? WsSecurityConstants.WsSecurity10 : WsSecurityConstants.WsSecurity11;
            SecureConversation = trustVersion == WsTrustConstants.TrustFeb2005
                ? WsSecureConversationConstants.SecureConversationFeb2005
                : WsSecureConversationConstants.SecureConversation13;
        }

        public WsAddressingConstants Addressing { get; init; }

        public WsFederationConstants Federation { get; init; }

        public WsSecurityConstants Security { get; init; }
       
        public WsSecurityPolicyConstants SecurityPolicy { get; init; }
       
        public WsSecurityUtilityConstants SecurityUtility { get; init; }
        
        public WsSecureConversationConstants SecureConversation { get; init; }

        public WsTrustConstants Trust { get; init; }

        public WsTrustActions TrustActions { get; init; }

        public WsTrustKeyTypes TrustKeyTypes { get; init; }

        public AddressingVersion AddressingVersion { get; init; }

        public MessageSecurityVersion MessageSecurityVersion { get; init; }
    }
}
