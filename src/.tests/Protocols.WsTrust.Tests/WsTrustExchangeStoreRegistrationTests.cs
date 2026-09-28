using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.IdentityModel.Protocols.WsTrust;
using Xunit;

namespace Solid.Identity.Protocols.WsTrust.Tests;

public class WsTrustExchangeStoreRegistrationTests
{
    [Fact]
    public void DefaultStoreIsRegistered()
    {
        using var services = new ServiceCollection()
            .AddWsTrust(_ => { })
            .BuildServiceProvider();

        Assert.IsType<WsTrustExchangeStore>(services.GetRequiredService<IWsTrustExchangeStore>());
    }

    [Fact]
    public void BuilderAcceptsCustomStoreType()
    {
        using var services = new ServiceCollection()
            .AddWsTrust(builder => builder.AddWsTrustExchangeStore<TestExchangeStore>())
            .BuildServiceProvider();

        Assert.IsType<TestExchangeStore>(services.GetRequiredService<IWsTrustExchangeStore>());
    }

    [Fact]
    public void BuilderAcceptsCustomStoreFactory()
    {
        var expected = new TestExchangeStore();
        using var services = new ServiceCollection()
            .AddWsTrust(builder => builder.AddWsTrustExchangeStore(_ => expected))
            .BuildServiceProvider();

        Assert.Same(expected, services.GetRequiredService<IWsTrustExchangeStore>());
    }

    private sealed class TestExchangeStore : IWsTrustExchangeStore
    {
        public bool TryAdd(string context, WsTrustPendingExchange exchange) => throw new NotSupportedException();
        public bool TryGet(string context, out WsTrustPendingExchange exchange) => throw new NotSupportedException();
        public bool TryUpdate(string context, WsTrustPendingExchange current, WsTrustPendingExchange next) => throw new NotSupportedException();
        public bool TryRemove(string context, WsTrustPendingExchange current) => throw new NotSupportedException();
    }
}
