using System;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// This exception is thrown when a security is missing an ExpirationTime.
    /// </summary>
    public class WsTrustException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustException"/> class.
        /// </summary>
        public WsTrustException()
            : base()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustException"/> class.
        /// </summary>
        /// <param name="message">Additional information to be included in the exception and displayed to user.</param>
        public WsTrustException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustException"/> class.
        /// </summary>
        /// <param name="message">Addtional information to be included in the exception and displayed to user.</param>
        /// <param name="innerException">A <see cref="Exception"/> that represents the root cause of the exception.</param>
        public WsTrustException(string message, Exception innerException)
            : base(message, innerException)
        {}
    }
}
