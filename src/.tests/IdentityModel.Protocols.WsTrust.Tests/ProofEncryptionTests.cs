using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Tokens.Xml;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;

public class ProofEncryptionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReferenceSurvivesRequestRoundTrip(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var serializer = new WsTrustSerializer();
        var request = new WsTrustRequest(version.Actions.Issue)
        {
            ProofEncryption = new SecurityTokenElement(new SecurityTokenReference
            {
                KeyIdentifier = new KeyIdentifier { Value = "encryption-key", ValueType = "urn:example:key" }
            })
        };

        var xml = WriteRequest(serializer, version, request);
        Assert.Contains("SecurityTokenReference", xml);
        Assert.Contains("encryption-key", xml);
        var parsed = ReadRequest(serializer, xml);
        Assert.Equal("encryption-key", parsed.ProofEncryption.SecurityTokenReference.KeyIdentifier.Value);
        Assert.Equal("urn:example:key", parsed.ProofEncryption.SecurityTokenReference.KeyIdentifier.ValueType);
        Assert.Equal(version.Actions.Issue, parsed.RequestType);
        Assert.Equal("encryption-key", ReadRequest(serializer, WriteRequest(serializer, version, parsed)).ProofEncryption.SecurityTokenReference.KeyIdentifier.Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RegisteredTokenSurvivesRequestRoundTrip(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var serializer = new WsTrustSerializer();
        serializer.SecurityTokenHandlers.Add(new GenericXmlSecurityTokenHandler());
        var document = new XmlDocument();
        document.LoadXml("<key:PublicKey xmlns:key='urn:example:key'>public-material</key:PublicKey>");
        var request = new WsTrustRequest(version.Actions.Issue)
        {
            ProofEncryption = new SecurityTokenElement(new GenericXmlSecurityToken(document.DocumentElement, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1)))
        };

        var xml = WriteRequest(serializer, version, request);
        Assert.Contains("public-material", xml);
        var parsed = ReadRequest(serializer, xml);
        var token = Assert.IsType<GenericXmlSecurityToken>(parsed.ProofEncryption.SecurityToken);
        Assert.Equal("PublicKey", token.Element.LocalName);
        Assert.Equal("urn:example:key", token.Element.NamespaceURI);
        Assert.Equal("public-material", token.Element.InnerText);
        Assert.Contains("public-material", WriteRequest(serializer, version, parsed));
    }

    [Theory]
    [InlineData("<t:ProofEncryption />")]
    [InlineData("<t:ProofEncryption><wsse:SecurityTokenReference/></t:ProofEncryption>")]
    [InlineData("<t:ProofEncryption><x:Unknown xmlns:x='urn:example:unknown'/></t:ProofEncryption>")]
    [InlineData("<t:ProofEncryption><wsse:SecurityTokenReference/><wsse:SecurityTokenReference/></t:ProofEncryption>")]
    public void InvalidPayloadFailsRatherThanReadingFollowingElement(string payload)
    {
        var xml = $"<t:RequestSecurityToken xmlns:t='{WsTrustConstants.Trust13.Namespace}' xmlns:wsse='{WsSecurityConstants.WsSecurity10.Namespace}'>{payload}<t:TokenType>urn:example:type</t:TokenType></t:RequestSecurityToken>";
        Assert.Throws<XmlReadException>(() => ReadRequest(new WsTrustSerializer(), xml));
    }

    [Fact]
    public void EmbeddedTokenWithoutHandlerFailsRatherThanWritingEmptyWrapper()
    {
        var version = WsTrustConstants.Trust13;
        var document = new XmlDocument();
        document.LoadXml("<PublicKey xmlns='urn:example:key'>material</PublicKey>");
        var request = new WsTrustRequest(version.Actions.Issue)
        {
            ProofEncryption = new SecurityTokenElement(new GenericXmlSecurityToken(document.DocumentElement, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1)))
        };
        Assert.Throws<XmlWriteException>(() => WriteRequest(new WsTrustSerializer(), version, request));
    }

    private static string WriteRequest(WsTrustSerializer serializer, WsTrustConstants version, WsTrustRequest request)
    {
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            serializer.WriteRequest(writer, version, request);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static WsTrustRequest ReadRequest(WsTrustSerializer serializer, string xml)
    {
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        return serializer.ReadRequest(reader);
    }
}
