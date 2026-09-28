using System;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Solid.IdentityModel.Protocols.WsTrust;
using Solid.IdentityModel.Protocols.WsSecurity;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Identity.Protocols.WsTrust.Tests;

public class NegotiationTests : IClassFixture<WsTrustTestsFixture>
{
    private readonly WsTrustTestsFixture _fixture;
    public NegotiationTests(WsTrustTestsFixture fixture, ITestOutputHelper output)
    {
        fixture.SetOutput(output);
        _fixture = fixture;
    }

    [Fact]
    public async Task ChallengeResponseCompletesIssueAndRejectsReplay()
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var version = WsTrustConstants.Trust13;
        var context = Guid.NewGuid().ToString("N");
        var xml = $"<t:RequestSecurityToken xmlns:t='{version.Namespace}' xmlns:wsp='http://schemas.xmlsoap.org/ws/2004/09/policy' xmlns:wsa='http://www.w3.org/2005/08/addressing' Context='{context}'><t:RequestType>{version.Actions.Issue}</t:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>urn:tests</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><t:BinaryExchange ValueType='{WsTrustNegotiation.EchoValueType}' EncodingType='{WsSecurityEncodingTypes.WsSecurity11.Base64}'>AQID</t:BinaryExchange></t:RequestSecurityToken>";
        using var message = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueRequest, XmlReader.Create(new System.IO.StringReader(xml)));
        using var challenge = await client.IssueAsync(message);
        Assert.Equal(version.Actions.IssueResponse, challenge.Headers.Action);
        var serializer = new WsTrustSerializer();
        using var reader = challenge.GetReaderAtBodyContents();
        Assert.Equal(WsTrustElements.RequestSecurityTokenResponse, reader.LocalName);
        var intermediate = serializer.ReadResponse(reader).RequestSecurityTokenResponseCollection[0];
        Assert.Equal(context, intermediate.Context);
        Assert.NotEmpty(intermediate.BinaryExchange.Data);

        var reply = new WsTrustResponse(new RequestSecurityTokenResponse { Context = context, BinaryExchange = intermediate.BinaryExchange });
        using var finalRequest = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueResponse, reply, new ResponseSerializer(serializer, version));
        using var final = await client.IssueAsync(finalRequest);
        Assert.Equal(version.Actions.IssueFinal, final.Headers.Action);
        using var finalReader = final.GetReaderAtBodyContents();
        Assert.NotNull(serializer.ReadResponse(finalReader).RequestSecurityTokenResponseCollection[0].RequestedSecurityToken);

        using var replay = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueResponse, reply, new ResponseSerializer(serializer, version));
        await Assert.ThrowsAnyAsync<Exception>(() => client.IssueAsync(replay));
    }

    [Fact]
    public async Task WrongChallengeAndUnknownContextAreRejected()
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var version = WsTrustConstants.Trust13;
        var serializer = new WsTrustSerializer();
        var bad = new WsTrustResponse(new RequestSecurityTokenResponse
        {
            Context = "unknown",
            BinaryExchange = new BinaryExchange(new byte[] { 1, 2 }, WsTrustNegotiation.EchoValueType)
        });
        using var message = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueResponse, bad, new ResponseSerializer(serializer, version));
        await Assert.ThrowsAnyAsync<Exception>(() => client.IssueAsync(message));
    }

    private sealed class ResponseSerializer : System.Runtime.Serialization.XmlObjectSerializer
    {
        private readonly WsTrustSerializer _serializer;
        private readonly WsTrustConstants _version;
        public ResponseSerializer(WsTrustSerializer serializer, WsTrustConstants version) { _serializer = serializer; _version = version; }
        public override bool IsStartObject(XmlDictionaryReader reader) => throw new NotSupportedException();
        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName) => throw new NotSupportedException();
        public override void WriteStartObject(XmlDictionaryWriter writer, object graph) { }
        public override void WriteEndObject(XmlDictionaryWriter writer) { }
        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph) => _serializer.WriteResponse(writer, _version, (WsTrustResponse)graph);
    }
}
