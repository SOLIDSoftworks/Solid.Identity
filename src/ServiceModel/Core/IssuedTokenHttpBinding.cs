using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Text;

namespace Solid.ServiceModel
{
    /// <summary>
    /// A binding used when a service requires an issued <see cref="SecurityToken"/>.
    /// </summary>
    public class IssuedTokenHttpBinding : WSHttpBinding
    {
        private SecurityBindingElement _messageSecurity;
        private IssuedSecurityTokenParameters _parameters;

        /// <summary>
        /// The <see cref="MessageSecurityVersion"/> of the <see cref="IssuedTokenHttpBinding"/>.
        /// </summary>
        public MessageSecurityVersion MessageSecurityVersion { get; set; } = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
        
        /// <summary>
        /// Creates an instance of <see cref="IssuedTokenHttpBinding"/>.
        /// </summary>
        public IssuedTokenHttpBinding()
            : base(SecurityMode.TransportWithMessageCredential, false)
        {
            _parameters = new IssuedSecurityTokenParameters();
            _parameters.RequireDerivedKeys = false;
            _parameters.KeyType = SecurityKeyType.BearerKey;
            Security.Message.ClientCredentialType = MessageCredentialType.IssuedToken;
            Security.Message.NegotiateServiceCredential = false;
            Security.Message.EstablishSecurityContext = false;
        }

        /// <summary>
        /// The <see cref="SecurityKeyType"/> of the <see cref="SecurityToken.SecurityKeys"/>.
        /// </summary>
        public SecurityKeyType KeyType { get => _parameters.KeyType; set => _parameters.KeyType = value; }

        /// <summary>
        /// Returns a value that indicates whether the current binding can build a channel factory stack on the client that satisfies the collection of binding parameters specified.
        /// </summary>
        /// <typeparam name="TChannel">The type of channel for which the factory is being tested.</typeparam>
        /// <param name="parameters">The <see cref="BindingParameterCollection"/> that specifies requirements for the channel factory that is built.</param>
        /// <returns><code>true</code> if the specified channel factory stack can be build on the client; otherwise, <code>false</code>.</returns>
        public override bool CanBuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            if (!parameters.Contains(typeof(ExtendedClientCredentials))) return false;
            return base.CanBuildChannelFactory<TChannel>(parameters);
        }

        /// <summary>
        /// Returns the security binding element from the current binding.
        /// </summary>
        /// <returns>A <see cref="SecurityBindingElement"/> from the current binding.</returns>
        protected override SecurityBindingElement CreateMessageSecurity()
        {
            var element = new TransportSecurityBindingElement
            {
                IncludeTimestamp = true,
                MessageSecurityVersion = MessageSecurityVersion
            }; 

            if (KeyType == SecurityKeyType.BearerKey)
                element.EndpointSupportingTokenParameters.Signed.Add(_parameters);
            else
                element.EndpointSupportingTokenParameters.Endorsing.Add(_parameters);

            _messageSecurity = element;

            return element;
        }

        /// <summary>
        /// Returns an ordered collection of binding elements contained in the current binding.
        /// </summary>
        /// <returns>The <see cref="BindingElementCollection"/> that contains the ordered stack of binding elements described by the <see cref="IssuedTokenHttpBinding"/> binding.</returns>
        public override BindingElementCollection CreateBindingElements()
        {
            var bindingElementCollection = base.CreateBindingElements();
            bindingElementCollection.Insert(0, new IssuedTokenBindingElement(_parameters, _messageSecurity));
            return bindingElementCollection;
        }
    }
}
