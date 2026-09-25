using Solid.IdentityModel.Protocols.WsTrust;
using Solid.Identity.Tokens.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.WsTrust.Logging
{
    internal class WsTrustMessageInformation : LogMessageState
    {
        public string RequestAction { get; set; }

        public string ResponseAction { get; set; }

        public string TrustNamespace { get; set; }

        [JsonIgnore]
        public WsTrustConstants WsTrustVersion { get; set; }

        public string Version => GetWsTrustVersionString(WsTrustVersion);

        private string GetWsTrustVersionString(WsTrustConstants version)
        {
            if (version == WsTrustConstants.Trust13) return nameof(WsTrustConstants.Trust13);
            if (version == WsTrustConstants.Trust14) return nameof(WsTrustConstants.Trust14);
            if (version == WsTrustConstants.TrustFeb2005) return nameof(WsTrustConstants.TrustFeb2005);

            return "unknown";
        }
    }
}
