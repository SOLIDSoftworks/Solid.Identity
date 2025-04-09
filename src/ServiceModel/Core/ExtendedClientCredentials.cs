using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.ServiceModel.Description;
using System.Text;

namespace Solid.ServiceModel
{
    internal class ExtendedClientCredentials : ClientCredentials
    {
        private SecurityTokenManager _securityTokenManager;
        private ILogger<ExtendedClientCredentials> _logger;
        private ILoggerFactory _loggerFactory;

        public ExtendedClientCredentials(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<ExtendedClientCredentials>();
            _loggerFactory = loggerFactory;
        }

        public ExtendedClientCredentials(ExtendedClientCredentials other, ILoggerFactory loggerFactory) 
            : this(other as ClientCredentials, loggerFactory)
        {
            ClientCredentials = other.ClientCredentials;
            _securityTokenManager = other._securityTokenManager;
        }

        public ExtendedClientCredentials(ClientCredentials other, ILoggerFactory loggerFactory) : base(other)
        {
            _logger = loggerFactory?.CreateLogger<ExtendedClientCredentials>();
            _loggerFactory = loggerFactory;
        }

        public IssuedTokenClientCredential IssuedToken { get; private set; } = new IssuedTokenClientCredential();
        public ClientCredentials ClientCredentials { get; }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            LogMessages.CreatingSecurityTokenManager(_logger);
            if (ClientCredentials != null)
                _securityTokenManager = ClientCredentials.CreateSecurityTokenManager();

            return new IssuedSecurityTokenManager((ExtendedClientCredentials)Clone(), _loggerFactory);
        }

        protected override ClientCredentials CloneCore() => new ExtendedClientCredentials(_loggerFactory) { IssuedToken = IssuedToken };
    }
}
