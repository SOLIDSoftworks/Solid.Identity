using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Xml
{
    public static class Out
    {
        public static bool False<T>(out T obj)
        {
            obj = default;
            return false;
        }

        public static bool True<T>(T result, out T obj)
        {
            obj = result;
            return true;
        }
    }
}
