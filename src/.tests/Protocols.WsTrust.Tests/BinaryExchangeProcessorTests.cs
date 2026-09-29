using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;
using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsPolicy;
using Solid.IdentityModel.Protocols.WsTrust;
using Xunit;

namespace Solid.Identity.Protocols.WsTrust.Tests;

public class BinaryExchangeProcessorTests
{
    [Fact]
    public async Task RegisteredValueTypeCanRequireMultipleRounds()
    {
        var processor = new WsTrustBinaryExchangeProcessor(new WsTrustExchangeStore(TimeProvider.System),
            new IBinaryExchangeProcessor[] { new TwoRoundProcessor() }, TimeProvider.System);
        var principal = Principal();
        var first = processor.Begin(principal, Request("urn:test:two-round", "two-round"));
        Assert.Equal(1, first.RequestSecurityTokenResponseCollection[0].BinaryExchange.Data[0]);
        var second = await processor.CompleteAsync(principal, Reply("two-round", "urn:test:two-round", 1), new Issuer(), default);
        Assert.Equal(2, second.RequestSecurityTokenResponseCollection[0].BinaryExchange.Data[0]);
        var issuer = new Issuer();
        await processor.CompleteAsync(principal, Reply("two-round", "urn:test:two-round", 2), issuer, default);
        Assert.Equal("urn:tests", issuer.Request.AppliesTo.EndpointReference.Uri);
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await processor.CompleteAsync(principal, Reply("two-round", "urn:test:two-round", 2), issuer, default));
    }

    [Fact]
    public async Task UnknownDuplicateAndSwitchedValueTypesFail()
    {
        var store = new WsTrustExchangeStore(TimeProvider.System);
        Assert.Throws<InvalidOperationException>(() => new WsTrustBinaryExchangeProcessor(store,
            new IBinaryExchangeProcessor[] { new TwoRoundProcessor(), new TwoRoundProcessor() }, TimeProvider.System));
        var processor = new WsTrustBinaryExchangeProcessor(store, new IBinaryExchangeProcessor[] { new TwoRoundProcessor() }, TimeProvider.System);
        Assert.Throws<InvalidRequestException>(() => processor.Begin(Principal(), Request("urn:unknown", "unknown")));
        processor.Begin(Principal(), Request("urn:test:two-round", "switched"));
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await processor.CompleteAsync(Principal(), Reply("switched", "urn:other", 1), new Issuer(), default));
    }

    [Fact]
    public async Task StoreContainsOnlyStateAndCanBeReplaced()
    {
        var store = new RecordingStore();
        var processors = new IBinaryExchangeProcessor[] { new TwoRoundProcessor() };
        var first = new WsTrustBinaryExchangeProcessor(store, processors, TimeProvider.System);
        first.Begin(Principal(), Request("urn:test:two-round", "shared"));
        Assert.Equal("urn:test:two-round", store.Current.ValueType);
        Assert.Equal(1, store.Current.Round);

        // A separate orchestrator can resume from the same store, without implementing processing in the store.
        var second = new WsTrustBinaryExchangeProcessor(store, processors, TimeProvider.System);
        await second.CompleteAsync(Principal(), Reply("shared", "urn:test:two-round", 1), new Issuer(), default);
        Assert.Equal(2, store.Current.Round);
        await second.CompleteAsync(Principal(), Reply("shared", "urn:test:two-round", 2), new Issuer(), default);
        Assert.Null(store.Current);
    }

    [Fact]
    public void ProcessorRegistrationsAreKeyedByValueType()
    {
        using var provider = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
            .AddWsTrust(builder => builder.AddBinaryExchangeProcessor<TwoRoundProcessor>())
            .BuildServiceProvider();
        var processors = provider.GetServices<IBinaryExchangeProcessor>();
        Assert.Contains(processors, p => p.ValueType == WsTrustNegotiation.EchoValueType);
        Assert.Contains(processors, p => p.ValueType == "urn:test:two-round");
        Assert.NotNull(provider.GetRequiredService<WsTrustBinaryExchangeProcessor>());
    }

    [Fact]
    public void CustomStoreAndProcessorFactoryAreUsedByOrchestrator()
    {
        var store = new RecordingStore();
        using var provider = new ServiceCollection()
            .AddWsTrust(builder =>
            {
                builder.AddWsTrustExchangeStore(_ => store);
                builder.AddBinaryExchangeProcessor(_ => new TwoRoundProcessor());
            })
            .BuildServiceProvider();
        var orchestrator = provider.GetRequiredService<WsTrustBinaryExchangeProcessor>();
        orchestrator.Begin(Principal(), Request("urn:test:two-round", "custom-store"));
        Assert.Same(store.Current, provider.GetRequiredService<IWsTrustExchangeStore>().TryGet("custom-store", out var pending) ? pending : null);
    }

    [Fact]
    public void OversizedProcessorStateIsRejectedBeforeAdmission()
    {
        var store = new RecordingStore();
        var orchestrator = new WsTrustBinaryExchangeProcessor(store,
            new IBinaryExchangeProcessor[] { new OversizedProcessor() }, TimeProvider.System);
        Assert.Throws<InvalidRequestException>(() => orchestrator.Begin(Principal(), Request("urn:test:oversized", "oversized")));
        Assert.Null(store.Current);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExpiryDuringContinueDoesNotIssueOrAdvance(bool completes)
    {
        var clock = new AdvancingClock();
        var store = new RecordingStore();
        var processor = new ExpiringProcessor(clock, completes);
        var orchestrator = new WsTrustBinaryExchangeProcessor(store, new IBinaryExchangeProcessor[] { processor }, clock);
        var issuer = new Issuer();
        orchestrator.Begin(Principal(), Request(processor.ValueType, "expires-during-continue"));

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await orchestrator.CompleteAsync(
            Principal(), Reply("expires-during-continue", processor.ValueType, 1), issuer, default));

        Assert.Null(issuer.Request);
        Assert.Null(store.Current);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConcurrentRepliesRunProcessorOnlyOnce(bool completes)
    {
        var store = new RecordingStore();
        using var processor = new BlockingProcessor(completes);
        var processors = new IBinaryExchangeProcessor[] { processor };
        var first = new WsTrustBinaryExchangeProcessor(store, processors, TimeProvider.System);
        var second = new WsTrustBinaryExchangeProcessor(store, processors, TimeProvider.System);
        var issuer = new Issuer();
        first.Begin(Principal(), Request(processor.ValueType, "concurrent"));

        var inFlight = Task.Run(async () => await first.CompleteAsync(Principal(), Reply("concurrent", processor.ValueType, 1), issuer, default));
        try
        {
            Assert.True(processor.Entered.Wait(TimeSpan.FromSeconds(10)));
            var competing = Task.Run(async () => await second.CompleteAsync(
                Principal(), Reply("concurrent", processor.ValueType, 1), issuer, default));
            Assert.Same(competing, await Task.WhenAny(competing, Task.Delay(TimeSpan.FromSeconds(3))));
            await Assert.ThrowsAsync<InvalidRequestException>(async () => await competing);
            Assert.Equal(1, processor.Calls);
        }
        finally
        {
            processor.Release.Set();
        }

        var result = await inFlight;
        Assert.Equal(1, processor.Calls);
        if (completes)
        {
            Assert.NotNull(issuer.Request);
            Assert.Null(store.Current);
        }
        else
        {
            Assert.Null(issuer.Request);
            Assert.Equal(2, store.Current.Round);
            Assert.Equal(2, result.RequestSecurityTokenResponseCollection[0].BinaryExchange.Data[0]);
        }
    }

    [Fact]
    public async Task FailedProcessorConsumesClaimedRound()
    {
        var store = new RecordingStore();
        var processor = new FailingProcessor();
        var orchestrator = new WsTrustBinaryExchangeProcessor(store, new IBinaryExchangeProcessor[] { processor }, TimeProvider.System);
        orchestrator.Begin(Principal(), Request(processor.ValueType, "failed-processor"));

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await orchestrator.CompleteAsync(
            Principal(), Reply("failed-processor", processor.ValueType, 1), new Issuer(), default));
        Assert.Null(store.Current);
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await orchestrator.CompleteAsync(
            Principal(), Reply("failed-processor", processor.ValueType, 1), new Issuer(), default));
        Assert.Equal(1, processor.Calls);
    }

    private sealed class FailingProcessor : IBinaryExchangeProcessor
    {
        public string ValueType => "urn:test:failed-processor";
        public int Calls { get; private set; }
        public BinaryExchangeStep Begin(BinaryExchange exchange) => BinaryExchangeStep.Challenge(new byte[] { 1 }, new byte[] { 1 });
        public BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange)
        {
            Calls++;
            throw new InvalidRequestException("Failed challenge.");
        }
    }

    private sealed class BlockingProcessor : IBinaryExchangeProcessor, IDisposable
    {
        private readonly bool _completes;
        private int _calls;
        public BlockingProcessor(bool completes) => _completes = completes;
        public string ValueType => "urn:test:concurrent";
        public ManualResetEventSlim Entered { get; } = new(false);
        public ManualResetEventSlim Release { get; } = new(false);
        public int Calls => Volatile.Read(ref _calls);
        public BinaryExchangeStep Begin(BinaryExchange exchange) => BinaryExchangeStep.Challenge(new byte[] { 1 }, new byte[] { 1 });
        public BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange)
        {
            Interlocked.Increment(ref _calls);
            Entered.Set();
            if (!Release.Wait(TimeSpan.FromSeconds(10))) throw new TimeoutException("Processor was not released.");
            return _completes ? BinaryExchangeStep.Completed() : BinaryExchangeStep.Challenge(new byte[] { 2 }, new byte[] { 2 });
        }
        public void Dispose() { Entered.Dispose(); Release.Dispose(); }
    }

    private sealed class AdvancingClock : TimeProvider
    {
        private DateTimeOffset _now = DateTimeOffset.Parse("2026-09-29T12:00:00Z");
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now += duration;
    }

    private sealed class ExpiringProcessor : IBinaryExchangeProcessor
    {
        private readonly AdvancingClock _clock;
        private readonly bool _completes;
        public ExpiringProcessor(AdvancingClock clock, bool completes) { _clock = clock; _completes = completes; }
        public string ValueType => "urn:test:expires-during-continue";
        public BinaryExchangeStep Begin(BinaryExchange exchange) => BinaryExchangeStep.Challenge(new byte[] { 1 }, new byte[] { 1 });
        public BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange)
        {
            _clock.Advance(TimeSpan.FromMinutes(2));
            return _completes ? BinaryExchangeStep.Completed() : BinaryExchangeStep.Challenge(new byte[] { 2 }, new byte[] { 2 });
        }
    }

    private sealed class RecordingStore : IWsTrustExchangeStore
    {
        public WsTrustPendingExchange Current { get; private set; }
        public bool TryAdd(string context, WsTrustPendingExchange exchange)
        {
            if (Current != null) return false;
            Current = exchange;
            return true;
        }
        public bool TryGet(string context, out WsTrustPendingExchange exchange)
        {
            exchange = Current;
            return exchange != null;
        }
        public bool TryUpdate(string context, WsTrustPendingExchange current, WsTrustPendingExchange next)
        {
            if (!ReferenceEquals(Current, current)) return false;
            Current = next;
            return true;
        }
        public bool TryRemove(string context, WsTrustPendingExchange current)
        {
            if (!ReferenceEquals(Current, current)) return false;
            Current = null;
            return true;
        }
    }

    private static ClaimsPrincipal Principal() => new(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, "subject"), new Claim(ClaimTypes.AuthenticationMethod, "password")
    }, "Password"));

    private static WsTrustRequest Request(string valueType, string context) => new(WsTrustConstants.Trust13.Actions.Issue)
    {
        Context = context, AppliesTo = new AppliesTo(new EndpointReference("urn:tests")),
        BinaryExchange = new BinaryExchange(new byte[] { 0 }, valueType)
    };

    private static WsTrustResponse Reply(string context, string valueType, byte data) => new(new RequestSecurityTokenResponse
    {
        Context = context, BinaryExchange = new BinaryExchange(new[] { data }, valueType)
    });

    private sealed class TwoRoundProcessor : IBinaryExchangeProcessor
    {
        public string ValueType => "urn:test:two-round";
        public BinaryExchangeStep Begin(BinaryExchange exchange) => BinaryExchangeStep.Challenge(new byte[] { 1 }, new byte[] { 1 });
        public BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange)
            => state[0] == 1 && exchange.Data[0] == 1
                ? BinaryExchangeStep.Challenge(new byte[] { 2 }, new byte[] { 2 })
                : state[0] == 2 && exchange.Data[0] == 2
                    ? BinaryExchangeStep.Completed()
                    : throw new InvalidRequestException("Wrong challenge.");
    }

    private sealed class OversizedProcessor : IBinaryExchangeProcessor
    {
        public string ValueType => "urn:test:oversized";
        public BinaryExchangeStep Begin(BinaryExchange exchange) => BinaryExchangeStep.Challenge(new byte[] { 1 }, new byte[4097]);
        public BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange) => throw new NotSupportedException();
    }

    private sealed class Issuer : ISecurityTokenService
    {
        public WsTrustRequest Request { get; private set; }
        public ValueTask<WsTrustResponse> IssueAsync(ClaimsPrincipal principal, WsTrustRequest request, CancellationToken cancellationToken)
        {
            Request = request;
            return new(new WsTrustResponse());
        }
        public ValueTask<WsTrustResponse> RenewAsync(ClaimsPrincipal principal, WsTrustRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();
        public ValueTask<WsTrustResponse> CancelAsync(ClaimsPrincipal principal, WsTrustRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();
        public ValueTask<WsTrustResponse> ValidateAsync(ClaimsPrincipal principal, WsTrustRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
