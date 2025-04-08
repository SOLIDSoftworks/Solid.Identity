using System;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// This exception is thrown when reading a <see cref="WsTrustMessage"/>.
    /// </summary>
    public class WsTrustWriteException : WsTrustException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustWriteException"/> class.
        /// </summary>
        public WsTrustWriteException()
            : base()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustWriteException"/> class.
        /// </summary>
        /// <param name="message">Addtional information to be included in the exception and displayed to user.</param>
        public WsTrustWriteException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustWriteException"/> class.
        /// </summary>
        /// <param name="message">Addtional information to be included in the exception and displayed to user.</param>
        /// <param name="innerException">A <see cref="Exception"/> that represents the root cause of the exception.</param>
        public WsTrustWriteException(string message, Exception innerException)
            : base(message, innerException)
        {}
    }
}
