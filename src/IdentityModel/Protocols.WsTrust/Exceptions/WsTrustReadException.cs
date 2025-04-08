//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.

using System;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// This exception is thrown when reading a <see cref="WsTrustMessage"/>.
    /// </summary>
    public class WsTrustReadException : WsTrustException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustReadException"/> class.
        /// </summary>
        public WsTrustReadException()
            : base()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustReadException"/> class.
        /// </summary>
        /// <param name="message">Addtional information to be included in the exception and displayed to user.</param>
        public WsTrustReadException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WsTrustReadException"/> class.
        /// </summary>
        /// <param name="message">Addtional information to be included in the exception and displayed to user.</param>
        /// <param name="innerException">A <see cref="Exception"/> that represents the root cause of the exception.</param>
        public WsTrustReadException(string message, Exception innerException)
            : base(message, innerException)
        {}
    }
}
