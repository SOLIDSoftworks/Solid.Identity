using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Logging;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;


/// <summary>
/// Set defaults for TheoryData
/// </summary>
public class TheoryDataBase
{
    public TheoryDataBase()
    {
        IdentityModelEventSource.ShowPII = true;
    }

    public ExpectedException ExpectedException { get; set; } = ExpectedException.NoExceptionExpected;

    public bool First { get; set; } = false;

    public Dictionary<Type, List<string>> PropertiesToIgnoreWhenComparing { get; set; } = new Dictionary<Type, List<string>>();

    public string TestId { get; set; }

    public override string ToString()
    {
        return $"{TestId}, {ExpectedException}";
    }
}