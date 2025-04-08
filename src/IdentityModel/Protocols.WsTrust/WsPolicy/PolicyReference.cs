#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsPolicy
{
    /// <summary>
    /// Defines the wsp:PolicyReference element.
    /// </summary>
    public class PolicyReference
    {
        public PolicyReference()
        {
        }

        public PolicyReference(string uri, string digest, string digestAlgorithm)
        {
            Uri = uri;
            Digest = digest;
            DigestAlgorithm = digestAlgorithm;
        }

        public string Digest { get; set; }

        public string DigestAlgorithm { get; set; }


        public string Uri { get; set; }
    }
}
