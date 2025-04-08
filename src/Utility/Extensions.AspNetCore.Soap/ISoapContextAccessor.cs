using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap
{
    /// <summary>
    /// An interface that describes a way to access the <see cref="SoapContext" /> of the current SOAP request.
    /// </summary>
    public interface ISoapContextAccessor
    {
        /// <summary>
        /// The <see cref="SoapContext" /> of the current SOAP request.
        /// </summary>
        SoapContext SoapContext { get; }
    }
}
