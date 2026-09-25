using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Logging;

namespace Solid.IdentityModel.Protocols.TestUtilities;

public class ProtocolTestData
{
    public ProtocolTestData()
    {
        IdentityModelEventSource.ShowPII = true;
    }
    
    public XmlDictionaryReader? Reader { get; set; }

    public Type? ExpectedExceptionType { get; set; }
    
    public string? ExpectedError { get; set; }

    public string TestId { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(TestId);
        if (ExpectedExceptionType != null)
            builder.Append($", {ExpectedExceptionType.Name}");
        if (ExpectedError != null)
            builder.Append($" ({ExpectedError})");
        return builder.ToString();
    }
}