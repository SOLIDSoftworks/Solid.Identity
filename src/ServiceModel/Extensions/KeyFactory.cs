using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.ServiceModel
{
    internal static class KeyFactory
    {
        public static string CreateKey<TProxy>(string name = null)
        {
            var type = typeof(TProxy);
            if (name == null) return type.Name;
            return $"{type.Name}_{name}";
        }
    }
}
