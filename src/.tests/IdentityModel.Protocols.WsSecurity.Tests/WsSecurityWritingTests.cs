using System;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsSecurity.Tests;

public class WsSecurityWritingTests
{
    private static readonly WsSerializer Serializer = new WsSerializer([
        new WsSecuritySerializer(), new WsSecurityUtilitySerializer()
    ]);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SecurityHeaderRoundTripsTimestampAndExtensions(bool withExtensions)
    {
        var context = new WsSerializationContext
        {
            Security = WsSecurityConstants.WsSecurity10,
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10
        };
        var header = new SecurityHeader
        {
            Timestamp = new Timestamp
            {
                Id = "timestamp-id",
                Created = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc),
                Expires = new DateTime(2026, 1, 2, 4, 4, 5, DateTimeKind.Utc)
            }
        };
        if (withExtensions)
            header.AdditionalXmlElements.Add(WsSecurityReferenceXml.RandomElement);

        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            Serializer.WriteEntity(writer, header, context);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        var actual = Serializer.ReadEntity<SecurityHeader>(reader, context);
        Assert.Equal(header.Timestamp.Id, actual.Timestamp.Id);
        Assert.Equal(header.Timestamp.Created, actual.Timestamp.Created);
        Assert.Equal(header.Timestamp.Expires, actual.Timestamp.Expires);
        Assert.Equal(header.AdditionalXmlElements.Count, actual.AdditionalXmlElements.Count);
    }

    [Fact]
    public void KeyIdentifierUsesUtilityNamespaceForId()
    {
        var context = new WsSerializationContext
        {
            Security = WsSecurityConstants.WsSecurity10,
            SecurityUtility = WsSecurityUtilityConstants.SecurityUtility10
        };
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            Serializer.WriteEntity(writer, new KeyIdentifier { Id = "key-id", Value = "value", ValueType = "type", EncodingType = "encoding" }, context);

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        var result = Serializer.ReadEntity<KeyIdentifier>(reader, context);
        Assert.Equal("key-id", result.Id);
        Assert.Equal("value", result.Value);
        Assert.Equal("type", result.ValueType);
        Assert.Equal("encoding", result.EncodingType);
    }

    [Fact]
    public void SecurityAssemblyDoesNotDependOnTrust()
    {
        Assert.DoesNotContain(typeof(WsSecuritySerializer).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name == "Solid.IdentityModel.Protocols.WsTrust");
    }
}
