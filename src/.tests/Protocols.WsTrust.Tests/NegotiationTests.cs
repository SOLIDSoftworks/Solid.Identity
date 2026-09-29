using System;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Solid.IdentityModel.Protocols.WsTrust;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.Identity.Protocols.WsTrust.Abstractions;
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

    [Theory]
    [InlineData("Context")]
    [InlineData("TokenType")]
    [InlineData("KeyType")]
    [InlineData("Extension")]
    public async Task OversizedIssueBodyIsRejectedBeforeParsing(string field)
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var version = WsTrustConstants.Trust13;
        var huge = new string('x', 70000);
        var context = field == "Context" ? huge : Guid.NewGuid().ToString("N");
        var tokenType = field == "TokenType" ? $"<t:TokenType>{huge}</t:TokenType>" : "";
        var keyType = field == "KeyType" ? $"<t:KeyType>{huge}</t:KeyType>" : "";
        var extension = field == "Extension" ? $"<e:Data xmlns:e='urn:test'>{huge}</e:Data>" : "";
        var xml = $"<t:RequestSecurityToken xmlns:t='{version.Namespace}' xmlns:wsp='http://schemas.xmlsoap.org/ws/2004/09/policy' xmlns:wsa='http://www.w3.org/2005/08/addressing' Context='{context}'><t:RequestType>{version.Actions.Issue}</t:RequestType>{tokenType}{keyType}<wsp:AppliesTo><wsa:EndpointReference><wsa:Address>urn:tests</wsa:Address></wsa:EndpointReference></wsp:AppliesTo>{extension}<t:BinaryExchange ValueType='{WsTrustNegotiation.EchoValueType}' EncodingType='{WsSecurityEncodingTypes.WsSecurity11.Base64}'>AQID</t:BinaryExchange></t:RequestSecurityToken>";
        using var request = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueRequest, XmlReader.Create(new System.IO.StringReader(xml)));

        await Assert.ThrowsAnyAsync<Exception>(() => client.IssueAsync(request));
    }

    [Theory]
    [InlineData("Context")]
    [InlineData("Extension")]
    public async Task OversizedIntermediateResponseIsRejectedBeforeParsing(string field)
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var version = WsTrustConstants.Trust13;
        var huge = new string('x', 70000);
        var context = field == "Context" ? huge : "urn:unknown";
        var extension = field == "Extension" ? $"<e:Data xmlns:e='urn:test'>{huge}</e:Data>" : "";
        var xml = $"<t:RequestSecurityTokenResponse xmlns:t='{version.Namespace}' Context='{context}'>{extension}<t:BinaryExchange ValueType='{WsTrustNegotiation.EchoValueType}' EncodingType='{WsSecurityEncodingTypes.WsSecurity11.Base64}'>AQID</t:BinaryExchange></t:RequestSecurityTokenResponse>";
        using var request = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueResponse, XmlReader.Create(new System.IO.StringReader(xml)));

        await Assert.ThrowsAnyAsync<Exception>(() => client.IssueAsync(request));
    }

    [Fact]
    public async Task AdditionalChallengeUsesIntermediateActionAndBody()
    {
        var client = _fixture.CreateWsTrust13UserNameClient("userName", "password");
        var version = WsTrustConstants.Trust13;
        var context = Guid.NewGuid().ToString("N");
        var xml = $"<t:RequestSecurityToken xmlns:t='{version.Namespace}' xmlns:wsp='http://schemas.xmlsoap.org/ws/2004/09/policy' xmlns:wsa='http://www.w3.org/2005/08/addressing' Context='{context}'><t:RequestType>{version.Actions.Issue}</t:RequestType><wsp:AppliesTo><wsa:EndpointReference><wsa:Address>urn:tests</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><t:BinaryExchange ValueType='{TwoRoundSoapProcessor.Type}' EncodingType='{WsSecurityEncodingTypes.WsSecurity11.Base64}'>AA==</t:BinaryExchange></t:RequestSecurityToken>";
        using var request = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueRequest, XmlReader.Create(new System.IO.StringReader(xml)));
        using var first = await client.IssueAsync(request);
        Assert.Equal(version.Actions.IssueResponse, first.Headers.Action);
        var serializer = new WsTrustSerializer();
        using var firstReader = first.GetReaderAtBodyContents();
        Assert.Equal(WsTrustElements.RequestSecurityTokenResponse, firstReader.LocalName);
        var challenge = serializer.ReadResponse(firstReader).RequestSecurityTokenResponseCollection[0];

        using var response = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueResponse,
            new WsTrustResponse(new RequestSecurityTokenResponse { Context = context, BinaryExchange = challenge.BinaryExchange }),
            new ResponseSerializer(serializer, version));
        using var second = await client.IssueAsync(response);
        Assert.Equal(version.Actions.IssueResponse, second.Headers.Action);
        using var secondReader = second.GetReaderAtBodyContents();
        Assert.Equal(WsTrustElements.RequestSecurityTokenResponse, secondReader.LocalName);
        var next = serializer.ReadResponse(secondReader).RequestSecurityTokenResponseCollection[0];
        Assert.Equal(context, next.Context);
        Assert.Equal(new byte[] { 2 }, next.BinaryExchange.Data);

        using var completion = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueResponse,
            new WsTrustResponse(new RequestSecurityTokenResponse { Context = context, BinaryExchange = next.BinaryExchange }),
            new ResponseSerializer(serializer, version));
        using var final = await client.IssueAsync(completion);
        Assert.Equal(version.Actions.IssueFinal, final.Headers.Action);
        using var finalReader = final.GetReaderAtBodyContents();
        Assert.Equal(WsTrustElements.RequestSecurityTokenResponseCollection, finalReader.LocalName);
        Assert.NotNull(serializer.ReadResponse(finalReader).RequestSecurityTokenResponseCollection[0].RequestedSecurityToken);
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

public sealed class TwoRoundSoapProcessor : IBinaryExchangeProcessor
{
    public const string Type = "urn:test:soap:two-round";
    public string ValueType => Type;
    public BinaryExchangeStep Begin(BinaryExchange exchange) => BinaryExchangeStep.Challenge(new byte[] { 1 }, new byte[] { 1 });
    public BinaryExchangeStep Continue(byte[] state, BinaryExchange exchange)
        => state[0] == 1 && exchange.Data[0] == 1
            ? BinaryExchangeStep.Challenge(new byte[] { 2 }, new byte[] { 2 })
            : state[0] == 2 && exchange.Data[0] == 2
                ? BinaryExchangeStep.Completed()
                : throw new InvalidOperationException("Unexpected challenge response.");
}
