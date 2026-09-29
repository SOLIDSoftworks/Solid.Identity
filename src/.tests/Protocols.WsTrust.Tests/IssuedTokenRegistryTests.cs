using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;
using Xunit;

namespace Solid.Identity.Protocols.WsTrust.Tests;

public class IssuedTokenRegistryTests
{
    private static ClaimsPrincipal Principal(string subject = "subject", string issuer = "issuer", string method = "password")
        => new(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, subject, ClaimValueTypes.String, issuer),
            new Claim(ClaimTypes.AuthenticationMethod, method)
        }, "Password"));

    private static XmlElement Token(string id)
    {
        var document = new XmlDocument();
        document.LoadXml($"<Assertion xmlns='urn:token' ID='{id}'/>");
        return document.DocumentElement;
    }

    [Fact]
    public void IssuerQualifiedSubjectDeterminesOwnerEvenWithoutName()
    {
        var registry = new IssuedTokenRegistry(TimeProvider.System);
        var token = Token("owner");
        registry.Register(token, Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        Assert.True(registry.TryGet(token, out var entry));
        Assert.True(registry.IsOwner(entry, Principal()));
        Assert.False(registry.IsOwner(entry, Principal(issuer: "other")));
        Assert.False(registry.IsOwner(entry, Principal(method: "x509")));
        Assert.False(registry.IsOwner(entry, Principal(subject: "other")));
        Assert.Throws<InvalidRequestException>(() => registry.Register(Token("no-subject"),
            new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "name") }, "Password")),
            "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key"));
    }

    [Fact]
    public void ExpiredEntriesAreEvictedAndCapacityIsBounded()
    {
        var registry = new IssuedTokenRegistry(TimeProvider.System);
        Assert.Throws<InvalidRequestException>(() => registry.Register(Token("expired"), Principal(),
            "urn:audience", DateTime.UtcNow.AddSeconds(-1), "type", "key"));
        Assert.False(registry.TryGet(Token("expired"), out _));
        for (var i = 0; i < 1024; i++)
            registry.Register(Token(i.ToString()), Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        Assert.Throws<InvalidRequestException>(() => registry.Register(Token("over-capacity"), Principal(),
            "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key"));
    }

    [Fact]
    public void FailedRenewalLeavesOriginalActiveAndSuccessfulRenewalReplacesIt()
    {
        var registry = new IssuedTokenRegistry(TimeProvider.System);
        var original = Token("original");
        registry.Register(original, Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        Assert.True(registry.TryGet(original, out var entry));
        registry.Register(Token("replacement"), Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        Assert.True(registry.TryReplace(original, entry, Token("replacement"), Principal()));
        Assert.True(registry.TryGet(original, out var old));
        Assert.True(old.Cancelled);
        Assert.True(registry.TryGet(Token("replacement"), out var replacement));
        Assert.False(replacement.Cancelled);
    }

    [Fact]
    public void LosingConcurrentRenewalDoesNotRemoveWinningReplacement()
    {
        var registry = new IssuedTokenRegistry(TimeProvider.System);
        var original = Token("original-concurrent");
        var next = Token("next-concurrent");
        registry.Register(original, Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        registry.TryGet(original, out var current);
        registry.Register(next, Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        registry.TryGet(next, out var winningEntry);
        Assert.True(registry.TryReplace(original, current, next, Principal()));
        Assert.False(registry.TryReplace(original, current, next, Principal()));
        Assert.True(registry.TryGet(next, out var stillPresent));
        Assert.Same(winningEntry, stillPresent);
    }

    [Fact]
    public void SharedStorePreservesLifecycleAcrossRegistryInstances()
    {
        var store = new InMemoryIssuedTokenStore(TimeProvider.System);
        var first = new IssuedTokenRegistry(TimeProvider.System, store);
        var second = new IssuedTokenRegistry(TimeProvider.System, store);
        var token = Token("shared");
        first.Register(token, Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        Assert.True(second.TryGet(token, out var entry));
        Assert.True(second.TryCancel(token, entry));
        Assert.True(first.TryGet(token, out var cancelled));
        Assert.True(cancelled.Cancelled);
        Assert.False(first.TryCancel(token, entry));
    }

    [Fact]
    public void BuilderRegistersCustomStoreForRegistry()
    {
        var services = new ServiceCollection();
        services.AddSingleton(TimeProvider.System);
        services.AddWsTrust(builder => builder.AddIssuedTokenStore<TestIssuedTokenStore>());
        using var provider = services.BuildServiceProvider();
        Assert.IsType<TestIssuedTokenStore>(provider.GetRequiredService<IIssuedTokenStore>());
        Assert.IsType<TestIssuedTokenStore>(provider.GetRequiredService<IssuedTokenRegistry>().Store);
    }

    [Fact]
    public void BuilderFactoryRegistersCustomStoreForRegistry()
    {
        var store = new TestIssuedTokenStore(TimeProvider.System);
        var services = new ServiceCollection();
        services.AddSingleton(TimeProvider.System);
        services.AddWsTrust(builder => builder.AddIssuedTokenStore(_ => store));
        using var provider = services.BuildServiceProvider();
        Assert.Same(store, provider.GetRequiredService<IssuedTokenRegistry>().Store);
    }

    [Fact]
    public void StaleRevisionCannotCancelOrRemoveUpdatedEntryAcrossRegistries()
    {
        var store = new InMemoryIssuedTokenStore(TimeProvider.System);
        var first = new IssuedTokenRegistry(TimeProvider.System, store);
        var second = new IssuedTokenRegistry(TimeProvider.System, store);
        var original = Token("old-node");
        var replacement = Token("new-node");
        first.Register(original, Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        second.Register(replacement, Principal(), "urn:audience", DateTime.UtcNow.AddMinutes(5), "type", "key");
        Assert.True(first.TryGet(original, out var entry));
        Assert.True(second.TryReplace(original, entry, replacement, Principal()));
        Assert.False(first.TryCancel(original, entry));
        Assert.False(first.TryRemove(original, entry));
        Assert.True(first.TryGet(original, out var cancelled));
        Assert.True(cancelled.Cancelled);
    }

    private sealed class TestIssuedTokenStore : InMemoryIssuedTokenStore
    {
        public TestIssuedTokenStore(TimeProvider clock) : base(clock) { }
    }
}
