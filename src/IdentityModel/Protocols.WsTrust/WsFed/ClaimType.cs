using Microsoft.IdentityModel.Logging;

#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsFed
{
    /// <summary>
    /// This class is used to represent a ClaimType found in the WsFed specification: http://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html .
    /// </summary>
    /// <remarks>Only 'Value' is read.</remarks>
    public class ClaimType
    {
        private string _uri;
        private string _value;

        /// <summary>
        /// Instantiates a <see cref="ClaimType"/> instance.
        /// </summary>
        public ClaimType() {}

        /// <summary>
        /// Gets ClaimType optional attribute.
        /// </summary>
        /// <remarks>This is an optional attribute.</remarks>
        public bool? IsOptional { get; set; }

        /// <summary>
        /// Gets ClaimType value element.
        /// </summary>
        /// <remarks>this is an optional value.</remarks>
        public string Value
        {
            get => _value;
            set => _value = (string.IsNullOrEmpty(value)) ? throw LogHelper.LogArgumentNullException(nameof(Value)) : value;
        }

        /// <summary>
        /// Gets ClaimType uri attribute.
        /// </summary>
        /// <remarks>this is a required value.</remarks>
        public string Uri
        {
            get => _uri;
            set => _uri = (string.IsNullOrEmpty(value)) ? throw LogHelper.LogArgumentNullException(nameof(Uri)) : value;
        }
    }
}
