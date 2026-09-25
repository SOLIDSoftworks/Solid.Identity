using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Solid.IdentityModel.Protocols.WsSecureConversation;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;

public class WsTrustResponseWireTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AuthenticatorRoundTrips(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var context = new WsSerializationContext(version);
        var serializer = new WsTrustSerializer();
        var response = new RequestSecurityTokenResponse
        {
            Authenticator = new Authenticator { CombinedHash = new CombinedHash { Value = "AQIDBA==" } }
        };

        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            serializer.WriteRequestSecurityTokenResponse(writer, version, response);

        stream.Position = 0;
        var xml = XDocument.Load(stream);
        Assert.Equal("AQIDBA==", (string)xml.Root?.Element(XName.Get(WsTrustElements.Authenticator, version.Namespace))?.Element(XName.Get(WsTrustElements.CombinedHash, version.Namespace)));

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        var result = serializer.ReadRequestSecurityTokenResponse(reader, context);
        Assert.Equal("AQIDBA==", result.Authenticator?.CombinedHash?.Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SecurityContextTokenRoundTrips(bool feb2005)
    {
        var version = feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13;
        var context = new WsSerializationContext(version);
        var serializer = new WsTrustSerializer();
        var token = new RequestedSecurityToken
        {
            SecurityContextToken = new SecurityContextToken
            {
                Id = "token-id",
                Identifier = new Identifier { Value = "urn:test:context" }
            }
        };

        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            serializer.WriteRequestedSecurityToken(writer, context, token);

        stream.Position = 0;
        var xml = XDocument.Load(stream);
        var securityContext = xml.Root?.Element(XName.Get(WsSecureConversationElements.SecurityContextToken, context.SecureConversation.Namespace));
        Assert.Equal("token-id", (string)securityContext?.Attribute(XName.Get("Id", context.SecurityUtility.Namespace)));
        Assert.Equal("urn:test:context", (string)securityContext?.Element(XName.Get(WsSecureConversationElements.Identifier, context.SecureConversation.Namespace)));

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        var result = WsTrustSerializer.ReadRequestedSecurityToken(reader, context);
        Assert.Equal("token-id", result.SecurityContextToken?.Id);
        Assert.Equal("urn:test:context", result.SecurityContextToken?.Identifier?.Value);
    }
}
