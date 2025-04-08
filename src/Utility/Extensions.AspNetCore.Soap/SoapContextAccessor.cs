using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Solid.Extensions.AspNetCore.Soap
{
    internal class SoapContextAccessor : ISoapContextAccessor
    {
        private IHttpContextAccessor _httpContextAccessor;

        public SoapContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public SoapContext SoapContext => _httpContextAccessor?.HttpContext.GetSoapContext();
    }
}
