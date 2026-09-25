using System;
using System.IO;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Protocols.WsSecurity;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsSecureConversation.Tests;

public class WsSecureConversationSerializerTests
{
    [Theory]
    [InlineData("http://schemas.xmlsoap.org/ws/2005/02/sc")]
    [InlineData("http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512")]
    public void ReadsAndWritesSecurityContextToken(string ns)
    {
        var context = new WsSerializationContext
        {
            SecureConversation = WsSecureConversationConstants.KnownNamespaces[ns],
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10
        };
        var token = new SecurityContextToken
        {
            Id = "token-id",
            Identifier = new Identifier { Value = "urn:uuid:12345678-1234-1234-1234-123456789abc" },
            Instance = "renewed"
        };
        var serializer = new WsSerializer([new WsSecureConversationSerializer()]);
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            serializer.WriteEntity(writer, token, context);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        Assert.Equal(ns, reader.NamespaceURI);
        Assert.Equal("token-id", reader.GetAttribute(WsSecurityUtilityAttributes.Id, context.SecurityUtility.Namespace));
        var actual = serializer.ReadEntity<SecurityContextToken>(reader, context);
        Assert.Equal(token.Id, actual.Id);
        Assert.Equal(token.Identifier.Value, actual.Identifier.Value);
        Assert.Equal(token.Instance, actual.Instance);
    }

    [Fact]
    public void UnknownChildrenAndAttributesArePreserved()
    {
        var context = new WsSerializationContext
        {
            SecureConversation = WsSecureConversationConstants.SecureConversation13,
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10
        };
        var xml = $"<wsc:SecurityContextToken xmlns:wsc=\"{context.SecureConversation.Namespace}\" xmlns:x=\"urn:extension\" x:flag=\"yes\"><wsc:Identifier>urn:uuid:token</wsc:Identifier><x:Metadata>value</x:Metadata></wsc:SecurityContextToken>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var serializer = new WsSerializer([new WsSecureConversationSerializer()]);
        var token = serializer.ReadEntity<SecurityContextToken>(reader, context);
        Assert.Single(token.AdditionalXmlAttributes);
        Assert.Single(token.AdditionalXmlElements);

        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            serializer.WriteEntity(writer, token, context);
        stream.Position = 0;
        using var result = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        result.MoveToContent();
        Assert.Equal("yes", result.GetAttribute("flag", "urn:extension"));
        Assert.Contains("Metadata", result.ReadOuterXml());
    }

    [Fact]
    public void MissingIdentifierIsRejected()
    {
        var context = new WsSerializationContext { SecureConversation = WsSecureConversationConstants.SecureConversation13 };
        var xml = $"<wsc:SecurityContextToken xmlns:wsc=\"{context.SecureConversation.Namespace}\" />";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        Assert.Throws<Microsoft.IdentityModel.Xml.XmlReadException>(() =>
            new WsSerializer([new WsSecureConversationSerializer()]).ReadEntity<SecurityContextToken>(reader, context));
    }

    [Fact]
    public void UnknownNamespaceIsRejectedByReadEntity()
    {
        const string xml = "<wsc:SecurityContextToken xmlns:wsc=\"urn:unknown\" />";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        Assert.Throws<Microsoft.IdentityModel.Xml.XmlReadException>(() =>
            new WsSerializer([new WsSecureConversationSerializer()]).ReadEntity<SecurityContextToken>(reader));
    }

    [Fact]
    public void AnUnqualifiedIdIsNotTreatedAsWsuId()
    {
        var context = new WsSerializationContext { SecureConversation = WsSecureConversationConstants.SecureConversation13 };
        var xml = $"<wsc:SecurityContextToken xmlns:wsc=\"{context.SecureConversation.Namespace}\" Id=\"plain\"><wsc:Identifier>urn:uuid:token</wsc:Identifier></wsc:SecurityContextToken>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        var token = new WsSerializer([new WsSecureConversationSerializer()]).ReadEntity<SecurityContextToken>(reader, context);
        Assert.Null(token.Id);
        Assert.Single(token.AdditionalXmlAttributes);
    }

    [Fact]
    public void UnknownNamespaceIsNotRecognized()
    {
        const string xml = "<wsc:SecurityContextToken xmlns:wsc=\"urn:unknown\"><wsc:Identifier>urn:uuid:token</wsc:Identifier></wsc:SecurityContextToken>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        var serializer = new WsSerializer([new WsSecureConversationSerializer()]);
        Assert.False(serializer.TryReadEntity<SecurityContextToken>(reader, out _));
    }

    [Fact]
    public void InvalidIdentifierIsRejectedWhenWriting()
    {
        var context = new WsSerializationContext { SecureConversation = WsSecureConversationConstants.SecureConversation13 };
        var token = new SecurityContextToken { Identifier = new Identifier { Value = "not an absolute URI" } };
        using var stream = new MemoryStream();
        using var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false);
        Assert.Throws<ArgumentException>(() => new WsSerializer([new WsSecureConversationSerializer()]).WriteEntity(writer, token, context));
    }

    [Fact]
    public void SecureConversationAssemblyDoesNotDependOnTrust()
    {
        Assert.DoesNotContain(typeof(WsSecureConversationSerializer).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name == "Solid.IdentityModel.Protocols.WsTrust");
    }
}
