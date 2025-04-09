using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Solid.ServiceModel
{
    internal class IssuedSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public IssuedSecurityTokenManager(ClientCredentials clientCredentials, ILoggerFactory loggerFactory) : base(clientCredentials)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory?.CreateLogger<IssuedSecurityTokenManager>();
        }

        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            LogMessages.CreatingIssuedTokenProvider(_logger);
            var parameters = tokenRequirement.GetProperty<ChannelParameterCollection>($"http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement/ChannelParametersCollection");
            var credentials = parameters.OfType<ExtendedClientCredentials>().FirstOrDefault();
            if (credentials == null)
            {
                LogMessages.ExtendedClientCredentialsNotFound(_logger);
                LogMessages.CreatingDefaultSecurityTokenProvider(_logger);
                return base.CreateSecurityTokenProvider(tokenRequirement);
            }

            return new IssuedTokenProvider(credentials, _loggerFactory);
        }
    }
}
