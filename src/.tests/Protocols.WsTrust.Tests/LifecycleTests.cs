using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Solid.IdentityModel.Protocols.WsTrust;
using Solid.IdentityModel.Protocols.WsPolicy;
using Solid.IdentityModel.Protocols.WsAddressing;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Identity.Protocols.WsTrust.Tests;

public class LifecycleTests : IClassFixture<WsTrustTestsFixture>
{
    private readonly WsTrustTestsFixture _fixture;
    public LifecycleTests(WsTrustTestsFixture fixture, ITestOutputHelper output)
    {
        fixture.SetOutput(output);
        _fixture = fixture;
    }

    [Fact]
    public async Task IssueValidateRenewCancelAndRejectRepeatedCancel()
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var issue = new WsTrustRequest(WsTrustConstants.Trust13.Actions.Issue) { Context = Guid.NewGuid().ToString("N"), KeyType = WsTrustConstants.Trust13.KeyTypes.Bearer, AppliesTo = new AppliesTo(new EndpointReference("urn:tests")) };
        var issued = await client.IssueAsync(issue);
        var token = issued.RequestSecurityTokenResponseCollection[0].RequestedSecurityToken.TokenElement;
        Assert.NotNull(token);

        var otherClient = _fixture.CreateWsTrust13UserNameClient("otherUser", "password");
        await Assert.ThrowsAnyAsync<Exception>(() => otherClient.ValidateAsync(new WsTrustRequest(WsTrustConstants.Trust13.Actions.Validate)
        {
            ValidateTarget = new TokenTarget(token)
        }));

        var validate = new WsTrustRequest(WsTrustConstants.Trust13.Actions.Validate) { Context = issue.Context, ValidateTarget = new TokenTarget(token) };
        var valid = await client.ValidateAsync(validate);
        Assert.Equal("http://docs.oasis-open.org/ws-sx/ws-trust/200512/status/valid", valid.RequestSecurityTokenResponseCollection[0].Status.Code);
        Assert.Equal(issue.Context, valid.RequestSecurityTokenResponseCollection[0].Context);

        var renew = new WsTrustRequest(WsTrustConstants.Trust13.Actions.Renew) { Context = issue.Context, RenewTarget = new TokenTarget(token) };
        var renewed = await client.RenewAsync(renew);
        var newToken = renewed.RequestSecurityTokenResponseCollection[0].RequestedSecurityToken.TokenElement;
        Assert.NotNull(newToken);
        Assert.NotEqual(token.OuterXml, newToken.OuterXml);

        var cancel = new WsTrustRequest(WsTrustConstants.Trust13.Actions.Cancel) { Context = issue.Context, CancelTarget = new TokenTarget(newToken) };
        var cancelled = await client.CancelAsync(cancel);
        Assert.True(cancelled.RequestSecurityTokenResponseCollection[0].RequestedTokenCancelled);
        var invalid = await client.ValidateAsync(new WsTrustRequest(WsTrustConstants.Trust13.Actions.Validate) { ValidateTarget = new TokenTarget(newToken) });
        Assert.Equal("http://docs.oasis-open.org/ws-sx/ws-trust/200512/status/invalid", invalid.RequestSecurityTokenResponseCollection[0].Status.Code);
        await Assert.ThrowsAnyAsync<Exception>(() => client.CancelAsync(cancel));
        await Assert.ThrowsAnyAsync<Exception>(() => client.RenewAsync(renew));
    }

    [Fact]
    public async Task UnknownAndMissingTargetsAreRejected()
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        await Assert.ThrowsAnyAsync<Exception>(() => client.ValidateAsync(new WsTrustRequest(WsTrustConstants.Trust13.Actions.Validate)));
        var doc = new System.Xml.XmlDocument();
        doc.LoadXml("<Assertion xmlns='urn:unknown' ID='missing'/>");
        await Assert.ThrowsAnyAsync<Exception>(() => client.CancelAsync(new WsTrustRequest(WsTrustConstants.Trust13.Actions.Cancel) { CancelTarget = new TokenTarget(doc.DocumentElement) }));
    }

    [Fact]
    public async Task Feb2005IssueValidateRenewCancelAtSoapLevel()
    {
        var client = _fixture.CreateWsTrustFeb2005UserNameClient("userName", "password");
        var version = WsTrustConstants.TrustFeb2005;
        var issued = await client.IssueAsync(new WsTrustRequest(version.Actions.Issue)
        {
            KeyType = version.KeyTypes.Bearer,
            AppliesTo = new AppliesTo(new EndpointReference("urn:tests"))
        });
        var token = issued.RequestSecurityTokenResponseCollection[0].RequestedSecurityToken.TokenElement;
        var status = await client.ValidateAsync(new WsTrustRequest(version.Actions.Validate) { ValidateTarget = new TokenTarget(token) });
        Assert.Equal(version.Namespace + "/status/valid", status.RequestSecurityTokenResponseCollection[0].Status.Code);
        var renewed = await client.RenewAsync(new WsTrustRequest(version.Actions.Renew) { RenewTarget = new TokenTarget(token) });
        var newToken = renewed.RequestSecurityTokenResponseCollection[0].RequestedSecurityToken.TokenElement;
        var cancelled = await client.CancelAsync(new WsTrustRequest(version.Actions.Cancel) { CancelTarget = new TokenTarget(newToken) });
        Assert.True(cancelled.RequestSecurityTokenResponseCollection[0].RequestedTokenCancelled);
    }

    [Fact]
    public async Task FailedRenewalKeepsOriginalTokenValid()
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var version = WsTrustConstants.Trust13;
        var issued = await client.IssueAsync(new WsTrustRequest(version.Actions.Issue)
        {
            KeyType = version.KeyTypes.Bearer,
            AppliesTo = new AppliesTo(new EndpointReference("urn:tests"))
        });
        var token = issued.RequestSecurityTokenResponseCollection[0].RequestedSecurityToken.TokenElement;
        var expiredLifetime = new Lifetime(DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));
        await Assert.ThrowsAnyAsync<Exception>(() => client.RenewAsync(new WsTrustRequest(version.Actions.Renew)
        {
            RenewTarget = new TokenTarget(token), Lifetime = expiredLifetime
        }));
        var anotherClient = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var status = await anotherClient.ValidateAsync(new WsTrustRequest(version.Actions.Validate) { ValidateTarget = new TokenTarget(token) });
        Assert.Equal(version.Namespace + "/status/valid", status.RequestSecurityTokenResponseCollection[0].Status.Code);
    }
}
