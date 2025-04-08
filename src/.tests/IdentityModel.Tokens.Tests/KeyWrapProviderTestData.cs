using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Tests
{
    public class KeyWrapProviderTestData
    {
        public bool Wrap { get; set; }
        public bool Unwrap { get; set; }
        public SecurityKey SecurityKey { get; set; }
        public string Algorithm { get; set; }
        public string PlainText { get; set; }
        public string Wrapped { get; set; }
    }
}
