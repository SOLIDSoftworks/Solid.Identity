using System;
using System.IO;
using System.Text;
using System.Xml;
using Solid.IdentityModel.Protocols;
using Solid.IdentityModel.Protocols.WsFed;
using Solid.IdentityModel.Protocols.WsFederation;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsFederation.Tests;

public class WsFederationSerializerTests
{
    private static readonly WsSerializationContext Context = new WsSerializationContext
    {
        Federation = WsFederationConstants.Federation12
    };

    [Fact]
    public void AdditionalContextRoundTrips()
    {
        var expected = new AdditionalContext();
        expected.Items.Add(new ContextItem("urn:claim", "test-value", "urn:scope"));
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsFedSerializer.WriteAdditionalContext(writer, Context, expected);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var actual = new WsFedSerializer().ReadAdditionalContext(reader, Context.Federation.Authorization.Namespace);
        var item = Assert.Single(actual.Items);
        Assert.Equal("urn:claim", item.Name);
        Assert.Equal("urn:scope", item.Scope);
        Assert.Equal("test-value", item.Value);
    }

    [Fact]
    public void ClaimTypeRoundTrips()
    {
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsFedSerializer.WriteClaimType(writer, Context, new ClaimType { Uri = "urn:claim", IsOptional = true, Value = "value" });

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var actual = new WsFedSerializer().ReadClaimType(reader, Context.Federation.Authorization.Namespace);
        Assert.Equal("urn:claim", actual.Uri);
        Assert.True(actual.IsOptional);
        Assert.Equal("value", actual.Value);
    }

    [Fact]
    public void OptionalClaimWithoutValueRoundTrips()
    {
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsFedSerializer.WriteClaimType(writer, Context, new ClaimType { Uri = "urn:optional", IsOptional = false });

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var actual = new WsFedSerializer().ReadClaimType(reader, Context.Federation.Authorization.Namespace);
        Assert.Equal("urn:optional", actual.Uri);
        Assert.False(actual.IsOptional);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void ContextItemWithoutScopeRoundTrips()
    {
        var expected = new AdditionalContext();
        expected.Items.Add(new ContextItem("urn:claim", "value"));
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsFedSerializer.WriteAdditionalContext(writer, Context, expected);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var item = Assert.Single(new WsFedSerializer().ReadAdditionalContext(reader, Context.Federation.Authorization.Namespace).Items);
        Assert.Equal("urn:claim", item.Name);
        Assert.Equal("value", item.Value);
        Assert.Null(item.Scope);
    }

    [Fact]
    public void ContextItemWithoutValueRoundTrips()
    {
        var expected = new AdditionalContext();
        expected.Items.Add(new ContextItem("urn:claim"));
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsFedSerializer.WriteAdditionalContext(writer, Context, expected);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        var item = Assert.Single(new WsFedSerializer().ReadAdditionalContext(reader, Context.Federation.Authorization.Namespace).Items);
        Assert.Equal("urn:claim", item.Name);
        Assert.Null(item.Value);
    }

    [Fact]
    public void AdditionalContextMustUseAuthorizationNamespace()
    {
        var xml = "<x:AdditionalContext xmlns:x=\"urn:not-federation\" />";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        Assert.Throws<Microsoft.IdentityModel.Xml.XmlReadException>(() =>
            new WsFedSerializer().ReadAdditionalContext(reader, Context.Federation.Authorization.Namespace));
    }

    [Fact]
    public void FederationAssemblyDoesNotDependOnTrust()
    {
        Assert.DoesNotContain(typeof(WsFedSerializer).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name == "Solid.IdentityModel.Protocols.WsTrust");
    }

    [Fact]
    public void FederationNamespacesAndSchemasMatchVersion12()
    {
        Assert.Equal("http://docs.oasis-open.org/wsfed/federation/200706", Context.Federation.Namespace);
        Assert.Equal("http://docs.oasis-open.org/wsfed/federation/v1.2/federation.xsd", Context.Federation.SchemaLocation);
        Assert.Equal("http://docs.oasis-open.org/wsfed/authorization/200706", Context.Federation.Authorization.Namespace);
        Assert.Equal("http://docs.oasis-open.org/wsfed/authorization/v1.2/authorization.xsd", Context.Federation.Authorization.SchemaLocation);
        Assert.Equal("http://docs.oasis-open.org/wsfed/privacy/v1.2/privacy.xsd", Context.Federation.Privacy.SchemaLocation);
    }
}
