using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.Text;

namespace Solid.ServiceModel
{
    internal class IssuedTokenBindingElement : BindingElement
    {
        private IssuedSecurityTokenParameters _parameters;
        private SecurityBindingElement _messageSecurity;

        public IssuedTokenBindingElement(IssuedSecurityTokenParameters parameters, SecurityBindingElement messageSecurity)
        {
            _parameters = parameters;
            _messageSecurity = messageSecurity;
        }

        public override BindingElement Clone() => new IssuedTokenBindingElement(_parameters, _messageSecurity);

        public override T GetProperty<T>(BindingContext context) => _messageSecurity.GetProperty<T>(context);

        //public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        //{
        //    return base.CanBuildChannelFactory<TChannel>(context);
        //}

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            var loggerFactory = Find<ILoggerFactory>(context.BindingParameters);
            var credentials = Find<ExtendedClientCredentials>(context.BindingParameters);
            if (credentials == null)
            {
                var clientCredentials = Find<ClientCredentials>(context.BindingParameters);
                if (clientCredentials != null)
                {
                    credentials = new ExtendedClientCredentials(clientCredentials, loggerFactory);
                    context.BindingParameters.Remove(typeof(ClientCredentials));
                    context.BindingParameters.Add(credentials);
                }
                else
                {
                    credentials = new ExtendedClientCredentials(loggerFactory);
                    context.BindingParameters.Add(credentials);
                }
            }

            var channelFactory = base.BuildChannelFactory<TChannel>(context);
            return channelFactory;
        }

        private T Find<T>(BindingParameterCollection bindingParameterCollection)
        {
            for (int i = 0; i < bindingParameterCollection.Count; i++)
            {
                object settings = bindingParameterCollection[i];
                if (settings is T)
                {
                    return (T)settings;
                }
            }
            return default(T);
        }
    }
}
