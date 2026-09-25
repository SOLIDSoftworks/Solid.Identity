using System.IO;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Protocols.WsAddressing;
using Solid.IdentityModel.Protocols.WsSecurity;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsPolicy.Tests;

public class WsPolicySerializerTests
{
    private static readonly WsSerializationContext Context = new WsSerializationContext
    {
        Addressing = WsAddressingConstants.Addressing10,
        SecurityPolicy = WsSecurityPolicyConstants.SecurityPolicy12
    };

    [Fact]
    public void AppliesToRoundTripsEndpointReference()
    {
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsPolicySerializer.WriteAppliesTo(writer, Context, new AppliesTo(new EndpointReference("https://example.test/")));

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        Assert.Equal("https://example.test/", new WsPolicySerializer().ReadAppliesTo(reader, Context.SecurityPolicy.Namespace).EndpointReference.Uri);
    }

    [Fact]
    public void PolicyReferenceRoundTripsAttributes()
    {
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsPolicySerializer.WritePolicyReference(writer, Context, new PolicyReference("urn:policy", "digest", "urn:hash"));

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var reference = new WsPolicySerializer().ReadPolicyReference(reader, Context.SecurityPolicy.Namespace);
        Assert.Equal("urn:policy", reference.Uri);
        Assert.Equal("digest", reference.Digest);
        Assert.Equal("urn:hash", reference.DigestAlgorithm);
    }

    [Fact]
    public void PolicyReferenceMustUseKnownPolicyNamespace()
    {
        var xml = "<x:PolicyReference xmlns:x=\"urn:not-policy\" URI=\"urn:policy\" />";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        Assert.Throws<Microsoft.IdentityModel.Xml.XmlReadException>(() =>
            new WsPolicySerializer().ReadPolicyReference(reader, Context.SecurityPolicy.Namespace));
    }

    [Fact]
    public void PolicyAssemblyDoesNotDependOnTrust()
    {
        Assert.DoesNotContain(typeof(WsPolicySerializer).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name == "Solid.IdentityModel.Protocols.WsTrust");
    }
}
