using System.IO;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Protocols.WsAddressing;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsAddressing.Tests;

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
    public void RoundTripsUnknownAttributesAndChildrenWithoutContext()
    {
        var ns = WsAddressingConstants.Addressing10.Namespace;
        var xml = $"<a:EndpointReference xmlns:a=\"{ns}\" xmlns:x=\"urn:ext\" x:flag=\"yes\"><a:Address>https://example.test/service</a:Address><x:Extension /></a:EndpointReference>";
        var serializer = new WsSerializer([new WsAddressingSerializer()]);
        using var input = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        var entity = serializer.ReadEntity<EndpointReference>(input);
        Assert.Single(entity.AdditionalXmlAttributes);
        Assert.Single(entity.AdditionalXmlElements);

        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            serializer.WriteEntity(writer, entity, new WsSerializationContext { Addressing = WsAddressingConstants.Addressing10 });
        stream.Position = 0;
        using var output = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        output.MoveToContent();
        Assert.Equal("yes", output.GetAttribute("flag", "urn:ext"));
        Assert.Contains("Extension", output.ReadOuterXml());
    }

    [Fact]
    public void TryReadWithUnknownNamespaceDoesNotConsumeElement()
    {
        const string xml = "<a:EndpointReference xmlns:a=\"urn:unknown\" />";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        Assert.False(new WsAddressingSerializer().TryReadEntity(reader, null, out _));
        Assert.Equal("EndpointReference", reader.LocalName);
    }

    [Theory]
    [InlineData("<a:Address>https://example.test/one</a:Address><a:Address>https://example.test/two</a:Address>")]
    [InlineData("<x:Extension xmlns:x=\"urn:ext\" /><a:Address>https://example.test/service</a:Address>")]
    public void RejectsAddressElementsThatAreDuplicatedOrNotFirst(string children)
    {
        const string ns = "http://www.w3.org/2005/08/addressing";
        var xml = $"<a:EndpointReference xmlns:a=\"{ns}\">{children}</a:EndpointReference>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);

        Assert.Throws<XmlException>(() => new WsAddressingSerializer().ReadEndpointReference(reader));
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

    [Fact]
    public void AddressingAssemblyDoesNotDependOnTrust()
    {
        Assert.DoesNotContain(typeof(WsAddressingSerializer).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name == "Solid.IdentityModel.Protocols.WsTrust");
    }
}
