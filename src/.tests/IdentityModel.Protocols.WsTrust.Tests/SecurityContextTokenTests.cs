using System.IO;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Protocols.WsSecureConversation;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;

public class SecurityContextTokenTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RequestedSecurityContextTokenRoundTrips(bool feb2005)
    {
        var context = new WsSerializationContext(feb2005 ? WsTrustConstants.TrustFeb2005 : WsTrustConstants.Trust13);
        var requested = new RequestedSecurityToken
        {
            SecurityContextToken = new SecurityContextToken
            {
                Id = "sct-id",
                Identifier = new Identifier { Value = "urn:uuid:12345678-1234-1234-1234-123456789abc" }
            }
        };

        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            new WsTrustSerializer().WriteRequestedSecurityToken(writer, context, requested);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var actual = WsTrustSerializer.ReadRequestedSecurityToken(reader, context);
        Assert.Equal(requested.SecurityContextToken.Id, actual.SecurityContextToken.Id);
        Assert.Equal(requested.SecurityContextToken.Identifier.Value, actual.SecurityContextToken.Identifier.Value);
        Assert.Null(actual.TokenElement);
    }

    [Fact]
    public void MalformedContextTokenIsNotTreatedAsOpaqueXml()
    {
        var context = new WsSerializationContext(WsTrustConstants.Trust13);
        var xml = $"<trust:RequestedSecurityToken xmlns:trust=\"{context.Trust.Namespace}\" xmlns:wsc=\"{context.SecureConversation.Namespace}\"><wsc:SecurityContextToken /></trust:RequestedSecurityToken>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        Assert.Throws<Microsoft.IdentityModel.Xml.XmlReadException>(() => WsTrustSerializer.ReadRequestedSecurityToken(reader, context));
    }
}
