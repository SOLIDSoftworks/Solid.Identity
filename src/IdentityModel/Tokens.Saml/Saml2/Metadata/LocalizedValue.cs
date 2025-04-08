using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class LocalizedValue<T>
    {
        public string Lang { get; set; }
        public T Value { get; set; }
    }

    public class LocalizedName : LocalizedValue<string> { }

    public class LocalizedUri : LocalizedValue<Uri> { }
}
