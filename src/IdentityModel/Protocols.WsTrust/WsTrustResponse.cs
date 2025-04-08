using System.Collections.Generic;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// The class defines the wst:RequestSecurityToken element which 
    /// is used to request a security token.
    /// </summary>
    public class WsTrustResponse
    {
        public WsTrustResponse()
        {
        }

        public WsTrustResponse(RequestSecurityTokenResponse requestSecurityTokenResponse)
        {
            RequestSecurityTokenResponseCollection.Add(requestSecurityTokenResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<RequestSecurityTokenResponse> RequestSecurityTokenResponseCollection { get; } = new List<RequestSecurityTokenResponse>();
    }
}
