using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Testing.Exceptions
{
    internal class SolidTestingAssertionException : Exception
    {
        public SolidTestingAssertionException(string message)
            : base(message)
        {
        }
    }
}
