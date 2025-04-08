using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Xml
{
    public static class XmlWriterExceptionHelper
    {
        public static Exception CreateRequiredAttributeMissingException(string elementName, string missingAttributeName)
            => new InvalidOperationException($"Unable to write element '{elementName}'. The attribute '{missingAttributeName}' is required.");

        public static Exception CreateRequiredChildElementMissingException(string elementName, string missingElementName)
            => new InvalidOperationException($"Unable to write element '{elementName}'. A at least one '{missingElementName}' if required.");

        public static Exception CreateRequiredChildElementMissingException(string elementName, string type, string missingElementName)
            => new InvalidOperationException($"Unable to write element '{elementName}' of type '{type}'. A at least one '{missingElementName}' if required.");

        public static Exception CreateRequiredElementValueMissingException(string elementName)
            => new InvalidOperationException($"Unable to write element '{elementName}'. Element content is required.");
    }
}
