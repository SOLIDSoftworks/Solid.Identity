using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Solid.IdentityModel.Protocols.WsSecurity;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;

public class WsTrustParameterWireTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LifecycleTargetsAndStatusRoundTrip(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var serializer = new WsTrustSerializer();
        var token = new XmlDocument();
        token.LoadXml("<Assertion xmlns='urn:example:token' ID='a1'/>");
        foreach (var kind in new[] { WsTrustElements.RenewTarget, WsTrustElements.CancelTarget, WsTrustElements.ValidateTarget })
        {
            var request = new WsTrustRequest(version.Actions.Renew) { Context = "request-123", Lifetime = new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5)) };
            var target = new TokenTarget(token.DocumentElement);
            if (kind == WsTrustElements.RenewTarget) request.RenewTarget = target;
            if (kind == WsTrustElements.CancelTarget) request.CancelTarget = target;
            if (kind == WsTrustElements.ValidateTarget) request.ValidateTarget = target;
            using var stream = new MemoryStream();
            using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
                serializer.WriteRequest(writer, version, request);
            stream.Position = 0;
            using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
            var result = serializer.ReadRequest(reader);
            var actual = kind == WsTrustElements.RenewTarget ? result.RenewTarget : kind == WsTrustElements.CancelTarget ? result.CancelTarget : result.ValidateTarget;
            Assert.Equal("a1", actual?.TokenElement?.GetAttribute("ID"));
            Assert.NotNull(result.Lifetime);
            Assert.Equal("request-123", result.Context);
        }

        var response = new WsTrustResponse(new RequestSecurityTokenResponse { Context = "first", Status = new TrustStatus("urn:valid", "ok"), RequestedTokenCancelled = true });
        response.RequestSecurityTokenResponseCollection.Add(new RequestSecurityTokenResponse { Context = "second", Status = new TrustStatus("urn:invalid") });
        using var output = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(output, Encoding.UTF8, false))
            serializer.WriteResponse(writer, version, response);
        output.Position = 0;
        using var responseReader = XmlDictionaryReader.CreateTextReader(output, XmlDictionaryReaderQuotas.Max);
        var roundTrip = serializer.ReadResponse(responseReader);
        Assert.Equal("first", roundTrip.RequestSecurityTokenResponseCollection[0].Context);
        Assert.Equal("second", roundTrip.RequestSecurityTokenResponseCollection[1].Context);
        Assert.Equal("urn:valid", roundTrip.RequestSecurityTokenResponseCollection[0].Status?.Code);
        Assert.Equal("ok", roundTrip.RequestSecurityTokenResponseCollection[0].Status?.Reason);
        Assert.True(roundTrip.RequestSecurityTokenResponseCollection[0].RequestedTokenCancelled);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RequestParametersRoundTrip(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var serializer = new WsTrustSerializer();
        var request = new WsTrustRequest(version.Actions.Issue)
        {
            AuthenticationType = "urn:auth", SignatureAlgorithm = "urn:signature", KeyWrapAlgorithm = "urn:wrap",
            Renewing = new Renewing { Allow = false, RenewAfterExpiration = true },
            Forwardable = true, Delegatable = false, AllowPostdating = true
        };
        if (!feb2005) request.SecondaryParameters = new WsTrustRequest(version.Actions.Issue) { TokenType = "urn:secondary" };
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            serializer.WriteRequest(writer, version, request);
        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        var result = serializer.ReadRequest(reader);
        Assert.Equal("urn:auth", result.AuthenticationType);
        Assert.Equal("urn:signature", result.SignatureAlgorithm);
        Assert.Equal("urn:wrap", result.KeyWrapAlgorithm);
        Assert.False(result.Renewing.Allow);
        Assert.True(result.Renewing.RenewAfterExpiration);
        Assert.True(result.Forwardable);
        Assert.False(result.Delegatable);
        Assert.True(result.AllowPostdating);
        if (!feb2005) Assert.Equal("urn:secondary", result.SecondaryParameters.TokenType);
        Assert.Null(result.TokenType); // secondary values do not overwrite explicit primary parameters
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MalformedKnownElementsAreRejected(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var serializer = new WsTrustSerializer();
        foreach (var child in new[] { "<t:ValidateTarget/>", "<t:SecondaryParameters><t:SecondaryParameters/></t:SecondaryParameters>", "<t:RequestedTokenCancelled/>", "<t:Renewing Allow='maybe'/>" })
        {
            var xml = $"<t:RequestSecurityToken xmlns:t='{version.Namespace}'><t:RequestType>{version.Actions.Issue}</t:RequestType>{child}</t:RequestSecurityToken>";
            using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
            Assert.ThrowsAny<Exception>(() => serializer.ReadRequest(reader));
        }
        var responseXml = $"<t:RequestSecurityTokenResponse xmlns:t='{version.Namespace}'><t:Status><t:Reason>missing code</t:Reason></t:Status></t:RequestSecurityTokenResponse>";
        using var responseReader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(responseXml), XmlDictionaryReaderQuotas.Max);
        Assert.ThrowsAny<Exception>(() => serializer.ReadRequestSecurityTokenResponse(responseReader, new WsSerializationContext(version)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TargetReferenceAndResponseExtensionsSurvive(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var xml = $"<t:RequestSecurityToken xmlns:t='{version.Namespace}' xmlns:s='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'><t:RequestType>{version.Actions.Validate}</t:RequestType><t:ValidateTarget><s:SecurityTokenReference><s:KeyIdentifier ValueType='urn:key'>abc</s:KeyIdentifier></s:SecurityTokenReference></t:ValidateTarget></t:RequestSecurityToken>";
        var serializer = new WsTrustSerializer();
        using var input = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        var request = serializer.ReadRequest(input);
        Assert.Equal("abc", request.ValidateTarget.Reference?.KeyIdentifier?.Value);
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false)) serializer.WriteRequest(writer, version, request);
        stream.Position = 0;
        using var output = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        Assert.Equal("abc", serializer.ReadRequest(output).ValidateTarget.Reference.KeyIdentifier.Value);

        var responseXml = $"<t:RequestSecurityTokenResponse xmlns:t='{version.Namespace}' xmlns:e='urn:extension' Context='ctx'><e:Custom value='retained'/></t:RequestSecurityTokenResponse>";
        using var responseReader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(responseXml), XmlDictionaryReaderQuotas.Max);
        var response = serializer.ReadRequestSecurityTokenResponse(responseReader, new WsSerializationContext(version));
        Assert.Equal("retained", Assert.Single(response.AdditionalXmlElements).GetAttribute("value"));
        using var responseStream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(responseStream, Encoding.UTF8, false)) serializer.WriteRequestSecurityTokenResponse(writer, version, response);
        responseStream.Position = 0;
        Assert.Equal("retained", (string)XDocument.Load(responseStream).Root?.Element(XName.Get("Custom", "urn:extension"))?.Attribute("value"));
    }
}
