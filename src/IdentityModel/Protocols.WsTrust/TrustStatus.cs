using System;
using Microsoft.IdentityModel.Logging;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>The result of a WS-Trust Validate request.</summary>
    public class TrustStatus
    {
        public TrustStatus(string code, string reason = null)
        {
            Code = !string.IsNullOrWhiteSpace(code) ? code : throw LogHelper.LogArgumentNullException(nameof(code));
            Reason = reason;
        }

        public string Code { get; }
        public string Reason { get; }
    }
}
