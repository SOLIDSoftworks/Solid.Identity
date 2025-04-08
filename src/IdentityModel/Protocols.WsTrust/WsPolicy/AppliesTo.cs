using Microsoft.IdentityModel.Logging;
using Solid.IdentityModel.Protocols.WsAddressing;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsPolicy
{
    public class AppliesTo
    {
        internal AppliesTo()
        {
        }

        public AppliesTo(EndpointReference endpointReference)
        {
            EndpointReference = endpointReference ?? throw LogHelper.LogArgumentNullException(nameof(endpointReference));
        }

        public EndpointReference EndpointReference { get; }
    }
}
