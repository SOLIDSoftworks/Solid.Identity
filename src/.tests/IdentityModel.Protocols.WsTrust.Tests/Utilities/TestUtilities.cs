using System.Collections.Generic;
using System.Linq;
using Xunit;
using Solid.IdentityModel.Protocols.WsTrust.Tests.Utilities;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;

public static class TestUtilities
{
    public static CompareContext WriteHeader(string name, TheoryDataBase data)
        => new($"{name}: {data.TestId}", data);

    public static void AssertFailIfErrors(CompareContext context)
        => Assert.True(context.Diffs.Count == 0, $"{context.Title}: {string.Join("; ", context.Diffs)}");

    public static string SerializeAsSingleCommaDelimitedString(IEnumerable<string> values)
        => string.Join(",", values);
}
