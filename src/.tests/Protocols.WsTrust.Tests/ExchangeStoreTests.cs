using System;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using Solid.Identity.Protocols.WsTrust.Exceptions;
using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsPolicy;
using Solid.IdentityModel.Protocols.WsTrust;
using Xunit;

namespace Solid.Identity.Protocols.WsTrust.Tests;

public class ExchangeStoreTests
{
    private static ClaimsPrincipal Principal(string name, string subject, string method = "Password")
        => new(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, name), new Claim(ClaimTypes.NameIdentifier, subject),
            new Claim(ClaimTypes.AuthenticationMethod, method)
        }, method));

    private static WsTrustRequest Request(string context) => new(WsTrustConstants.Trust13.Actions.Issue)
    {
        Context = context,
        AppliesTo = new AppliesTo(new EndpointReference("urn:tests")),
        BinaryExchange = new BinaryExchange(new byte[] { 1 }, WsTrustNegotiation.EchoValueType)
    };

    [Fact]
    public async Task RetainedRequestIsBoundedAndDetachedFromCaller()
    {
        var store = new WsTrustExchangeStore(TimeProvider.System);
        var user = Principal("same", "subject");
        Assert.Throws<InvalidRequestException>(() => store.Begin(user, Request(new string('x', 129))));
        var request = Request("oversized-extension");
        var xml = new System.Xml.XmlDocument();
        xml.LoadXml("<e:Data xmlns:e='urn:test'>" + new string('x', 10000) + "</e:Data>");
        request.AdditionalXmlElements.Add(xml.DocumentElement);
        Assert.Throws<InvalidRequestException>(() => store.Begin(user, request));

        request = Request("oversized-audience");
        request.AppliesTo = new AppliesTo(new EndpointReference("urn:" + new string('a', 2049)));
        Assert.Throws<InvalidRequestException>(() => store.Begin(user, request));

        request = Request("detached");
        var challenge = store.Begin(user, request).RequestSecurityTokenResponseCollection[0];
        request.AppliesTo = new AppliesTo(new EndpointReference("urn:changed"));
        request.TokenType = "urn:changed";
        var service = new CapturingService();
        await store.CompleteAsync(user, new WsTrustResponse(new RequestSecurityTokenResponse
        {
            Context = challenge.Context, BinaryExchange = challenge.BinaryExchange
        }), service, default);
        Assert.Equal("urn:tests", service.Request.AppliesTo.EndpointReference.Uri);
        Assert.Null(service.Request.TokenType);
        Assert.Null(service.Request.BinaryExchange);

    }

    [Fact]
    public async Task AdmissionNeverExceedsCapacityUnderConcurrency()
    {
        var store = new WsTrustExchangeStore(TimeProvider.System);
        var user = Principal("name", "subject");
        var attempts = Enumerable.Range(0, 1300).Select(i => Task.Run(() =>
        {
            try { store.Begin(user, Request(i.ToString())); return true; }
            catch (InvalidRequestException) { return false; }
        }));
        var admitted = await Task.WhenAll(attempts);
        Assert.Equal(1024, admitted.Count(value => value));
    }

    [Theory]
    [InlineData("same", "other-subject", "Password")]
    [InlineData("same", "subject", "X509")]
    public async Task AnotherCredentialCannotCompleteTheExchange(string name, string subject, string method)
    {
        var store = new WsTrustExchangeStore(TimeProvider.System);
        var first = store.Begin(Principal("same", "subject"), Request("credential-check"));
        var result = first.RequestSecurityTokenResponseCollection[0];
        var response = new WsTrustResponse(new RequestSecurityTokenResponse { Context = result.Context, BinaryExchange = result.BinaryExchange });
        await Assert.ThrowsAsync<SecurityException>(async () => await store.CompleteAsync(Principal(name, subject, method), response, null, default));
    }

    [Fact]
    public async Task MissingStableCredentialClaimsAreRejected()
    {
        var store = new WsTrustExchangeStore(TimeProvider.System);
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "same") }, "Password");
        Assert.Throws<SecurityException>(() => store.Begin(new ClaimsPrincipal(identity), Request("no-subject")));
        var issued = store.Begin(Principal("same", "subject"), Request("subject"));
        var response = issued.RequestSecurityTokenResponseCollection[0];
        await Assert.ThrowsAsync<SecurityException>(async () => await store.CompleteAsync(new ClaimsPrincipal(identity),
            new WsTrustResponse(new RequestSecurityTokenResponse { Context = response.Context, BinaryExchange = response.BinaryExchange }), null, default));
    }

    [Theory]
    [InlineData("Lifetime")]
    [InlineData("AppliesTo")]
    [InlineData("KeySize")]
    [InlineData("AttachedReference")]
    [InlineData("Authenticator")]
    [InlineData("EncryptionAlgorithm")]
    public async Task IntermediateResponseRejectsAllOtherSupportedFields(string field)
    {
        var store = new WsTrustExchangeStore(TimeProvider.System);
        var user = Principal("same", "subject");
        var challenge = store.Begin(user, Request("unexpected-field")).RequestSecurityTokenResponseCollection[0];
        var response = new RequestSecurityTokenResponse { Context = challenge.Context, BinaryExchange = challenge.BinaryExchange };
        switch (field)
        {
            case "Lifetime": response.Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1)); break;
            case "AppliesTo": response.AppliesTo = new AppliesTo(new EndpointReference("urn:tests")); break;
            case "KeySize": response.KeySizeInBits = 256; break;
            case "AttachedReference": response.AttachedReference = new Solid.IdentityModel.Protocols.WsSecurity.SecurityTokenReference(); break;
            case "Authenticator": response.Authenticator = new Authenticator(); break;
            case "EncryptionAlgorithm": response.EncryptionAlgorithm = "urn:algorithm"; break;
        }
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await store.CompleteAsync(user, new WsTrustResponse(response), null, default));
    }

    private sealed class CapturingService : ISecurityTokenService
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
