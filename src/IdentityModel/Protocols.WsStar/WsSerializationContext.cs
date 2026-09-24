using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Collections.Generic;
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
            AddressingVersion = addressingVersion;
            MessageSecurityVersion = messageSecurityVersion;
            Federation = WsFederationConstants.Federation12;
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10;
            
            if (messageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
                Trust = WsTrustConstants.TrustFeb2005;
            else if (messageSecurityVersion.TrustVersion == TrustVersion.WSTrust13)
                Trust = WsTrustConstants.Trust13;
            
            if (addressingVersion == AddressingVersion.WSAddressing10)
                Addressing = WsAddressingConstants.Addressing10;
            else if (addressingVersion == AddressingVersion.WSAddressingAugust2004)
                Addressing = WsAddressingConstants.Addressing200408;

            if (messageSecurityVersion.SecurityVersion == SecurityVersion.WSSecurity10)
                Security = WsSecurityConstants.WsSecurity10;
            else if (messageSecurityVersion.SecurityVersion == SecurityVersion.WSSecurity11)
                Security = WsSecurityConstants.WsSecurity11;
            
            if (WsSecureConversationConstants.KnownNamespaces.TryGetValue(messageSecurityVersion.SecureConversationVersion.Namespace, out var secureConversation))
            {
                SecureConversation = secureConversation;
            }
            else
            {
                var wsc = new WsSecureConversationConstants
                {
                    DefaultPrefix = "sc",
                    Namespace = messageSecurityVersion.SecureConversationVersion.Namespace.ToString()
                };
                WsSecureConversationConstants.KnownNamespaces.Add(wsc.Namespace, wsc);
                SecureConversation = wsc;
            }
            
            if (WsSecurityPolicyConstants.KnownNamespaces.TryGetValue(messageSecurityVersion.SecurityPolicyVersion.Namespace, out var securityPolicy))
            {
                SecurityPolicy = securityPolicy;
            }
            else
            {
                var wsp = new WsSecurityPolicyConstants
                {
                    DefaultPrefix = "wsp",
                    Namespace = messageSecurityVersion.SecurityPolicyVersion.Namespace
                };
                WsSecurityPolicyConstants.KnownNamespaces.Add(wsp.Namespace, wsp);
                SecurityPolicy = wsp;
            }
        }
        
        public WsSerializationContext(TrustVersion trustVersion)
            : this(AddressingVersion.WSAddressing10, CreateMessageSecurityVersion(trustVersion))
        {
            // if (wsTrustVersion is WsTrustFeb2005Version)
            // {
            //     Addressing = WsAddressingConstants.Addressing10;
            //     FederationVersion = WsFederationConstants.Federation12;
            //     SecurityPolicy = WsSecurityPolicyConstants.SecurityPolicy12;
            //     Security = WsSecurityConstants.WsSecurity10;
            //     Trust = WsTrustConstants.TrustFeb2005;
            //     SecureConversation = WsSecureConversationConstants.SecureConversationFeb2005;
            // }
            // else if (wsTrustVersion is WsTrust13Version)
            // {
            //     Addressing = WsAddressingConstants.Addressing10;
            //     FederationVersion = WsFederationConstants.Federation12;
            //     SecurityPolicy = WsSecurityPolicyConstants.SecurityPolicy12;
            //     Security = WsSecurityConstants.WsSecurity11;
            //     TrustActions = WsTrustActions.Trust13;
            //     Trust = WsTrustConstants.Trust13;
            //     TrustKeyTypes = WsTrustKeyTypes.Trust13;
            //     SecureConversation = WsSecureConversationConstants.SecureConversation13;
            // }
            // else if (wsTrustVersion is WsTrust14Version)
            // {
            //     Addressing = WsAddressingConstants.Addressing10;
            //     FederationVersion = WsFederationConstants.Federation12;
            //     SecurityPolicy = WsSecurityPolicyConstants.SecurityPolicy12;
            //     Security = WsSecurityConstants.WsSecurity11;
            //     TrustActions = WsTrustActions.Trust14;
            //     Trust = WsTrustConstants.Trust14;
            //     TrustKeyTypes = WsTrustKeyTypes.Trust14;
            // }
        }

        public WsAddressingConstants Addressing { get; init; }

        public WsFederationConstants Federation { get; init; }

        public WsSecurityConstants Security { get; init; }
       
        public WsSecurityPolicyConstants SecurityPolicy { get; init; }
       
        public WsSecurityUtilityConstants SecurityUtility { get; init; }
        
        public WsSecureConversationConstants SecureConversation { get; init; }


        public WsTrustConstants Trust { get; init; }

        public AddressingVersion AddressingVersion { get; init; }

        public MessageSecurityVersion MessageSecurityVersion { get; init; }
        

        private static MessageSecurityVersion CreateMessageSecurityVersion(TrustVersion trustVersion)
        {
            throw new System.NotImplementedException();
        }
    }
}
