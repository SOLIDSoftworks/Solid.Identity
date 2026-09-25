using System.IO;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Protocols.WsAddressing;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;

public class WsAddressingSerializerTests
{
    [Theory]
    [InlineData("http://www.w3.org/2005/08/addressing")]
    [InlineData("http://schemas.xmlsoap.org/ws/2004/08/addressing")]
    public void ReadsEndpointReferenceWithUnknownChildren(string ns)
    {
        var xml = $"<a:EndpointReference xmlns:a=\"{ns}\"><a:Address>https://example.test/service</a:Address><x:Extension xmlns:x=\"urn:ext\" code=\"1\" /></a:EndpointReference>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        var result = new WsAddressingSerializer().ReadEndpointReference(reader);
        Assert.Equal("https://example.test/service", result.Uri);
        Assert.Single(result.AdditionalXmlElements);
        Assert.Equal("Extension", result.AdditionalXmlElements[0].LocalName);
    }

    [Fact]
    public void WritesAddressInAddressingNamespace()
    {
        var context = new WsSerializationContext { Addressing = WsAddressingConstants.Addressing10 };
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            new WsAddressingSerializer().WriteEntity(writer, new EndpointReference("https://example.test/service"), null, context);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        Assert.Equal("https://example.test/service", new WsAddressingSerializer().ReadEntity(reader, null, context).Uri);
    }
}
