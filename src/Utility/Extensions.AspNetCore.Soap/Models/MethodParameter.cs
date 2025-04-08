using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Soap.Models
{
    internal class MethodParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool Out { get; set; }
    }
}
