using System;
using System.IdentityModel.Tokens;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Xml;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsSecurity.Tests;

public class WsSecuritySerializerTests : IClassFixture<WsSecuritySerializerTestFixture>
{
    private readonly WsSecuritySerializerTestFixture _fixture;

    public WsSecuritySerializerTests(WsSecuritySerializerTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Theory, MemberData(nameof(ReadSecurityTokenReferenceTestCases))]
    public void ReadSecurityTokenReference(WsSecurityTestData data)
    {
        try
        {
            var result = _fixture.SecurityTokenReferenceSerializer.ReadEntity(data.Reader, _fixture.Serializer, data.SerializationContext);
            _fixture.Compare(data.SecurityTokenReference, result);
        }
        catch (Exception ex)
        {
            _fixture.Compare(data, ex);
        }
    }
    
    [Theory, MemberData(nameof(ReadSecurityHeaderTestCases))]
    public void ReadSecurityHeader(WsSecurityTestData data)
    {
        try
        {
            var result = _fixture.SecurityHeaderSerializer.ReadEntity(data.Reader, _fixture.Serializer, data.SerializationContext);
            _fixture.Compare(data.SecurityHeader, result);
        }
        catch (Exception ex)
        {
            _fixture.Compare(data, ex);
        }
    }

    [Theory]
    [InlineData("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public void ReadsKeyIdentifierWithoutContext(string ns)
    {
        var xml = $"<wsse:KeyIdentifier xmlns:wsse=\"{ns}\" ValueType=\"urn:type\">identifier</wsse:KeyIdentifier>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        var identifier = _fixture.Serializer.ReadEntity<KeyIdentifier>(reader);
        Assert.Equal("identifier", identifier.Value);
        Assert.Equal("urn:type", identifier.ValueType);
    }

    [Fact]
    public void ReadsTimestampWithoutContext()
    {
        var ns = WsSecurityUtilityConstants.SecurityUtility10.Namespace;
        var xml = $"<wsu:Timestamp xmlns:wsu=\"{ns}\" wsu:Id=\"timestamp\"><wsu:Created>2024-01-01T00:00:00Z</wsu:Created><wsu:Expires>2024-01-01T01:00:00Z</wsu:Expires></wsu:Timestamp>";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        var timestamp = _fixture.Serializer.ReadEntity<Timestamp>(reader);
        Assert.Equal("timestamp", timestamp.Id);
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), timestamp.Created);
    }

    [Fact]
    public void TryReadKeyIdentifierWithUnknownNamespaceDoesNotConsumeElement()
    {
        const string xml = "<wsse:KeyIdentifier xmlns:wsse=\"urn:unknown\" />";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        Assert.False(((IProtocolSerializer<KeyIdentifier>)new WsSecuritySerializer()).TryReadEntity(reader, _fixture.Serializer, out _));
        Assert.Equal("KeyIdentifier", reader.LocalName);
    }

    [Fact]
    public void TryReadTimestampWithUnknownNamespaceDoesNotConsumeElement()
    {
        const string xml = "<wsu:Timestamp xmlns:wsu=\"urn:unknown\" />";
        using var reader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), XmlDictionaryReaderQuotas.Max);
        Assert.False(((IProtocolSerializer<Timestamp>)new WsSecurityUtilitySerializer()).TryReadEntity(reader, _fixture.Serializer, out _));
        Assert.Equal("Timestamp", reader.LocalName);
    }

    public static TheoryData<WsSecurityTestData> ReadSecurityTokenReferenceTestCases
    {
        get
        {
            return new TheoryData<WsSecurityTestData>
            {
                new WsSecurityTestData(WsSecurityConstants.WsSecurity10, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    SecurityTokenReference = new SecurityTokenReference
                    {
                        Id = Guid.NewGuid().ToString(),
                        TokenType =  "type",
                        Usage = "usage"
                    },
                    TestId = "Read simple security token reference - WS-Security 1.0",
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    SecurityTokenReference = new SecurityTokenReference
                    {
                        Id = Guid.NewGuid().ToString(),
                        TokenType = "type",
                        Usage = "usage"
                    },
                    TestId = "Read simple security token reference - WS-Security 1.1",
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity10, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    SecurityTokenReference = new SecurityTokenReference
                    {
                        Id = Guid.NewGuid().ToString(),
                        KeyIdentifier = new KeyIdentifier
                        {
                            Id = Guid.NewGuid().ToString(),
                            Value =  Guid.NewGuid().ToString(),
                            ValueType = "test"
                        }
                    },
                    TestId = "Read security token reference with key identifier - WS-Security 1.0",
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    SecurityTokenReference = new SecurityTokenReference
                    {
                        Id = Guid.NewGuid().ToString(),
                        KeyIdentifier = new KeyIdentifier
                        {
                            Id = Guid.NewGuid().ToString(),
                            Value =  Guid.NewGuid().ToString(),
                            ValueType = "test"
                        }
                    },
                    TestId = "Read security token reference with key identifier - WS-Security 1.1",
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    TestId = "Reader null",
                    ExpectedExceptionType =  typeof(ArgumentNullException),
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    TestId = "Context null",
                    SerializationContext = null,
                    ExpectedExceptionType =  typeof(ArgumentNullException),
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    TestId = "Incorrect reader location",
                    Reader = WsSecurityReferenceXml.RandomElementReader,
                    ExpectedExceptionType =  typeof(XmlReadException),
                    ExpectedError = nameof(LogMessages.IDX15011)
                }
            };
        }
    }

    public static TheoryData<WsSecurityTestData> ReadSecurityHeaderTestCases
    {
        get
        {
            return new TheoryData<WsSecurityTestData>
            {
                new WsSecurityTestData(WsSecurityConstants.WsSecurity10, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    SecurityHeader = new SecurityHeader
                    {
                        Timestamp = new Timestamp
                        {
                            Created = DateTime.UtcNow,
                            Expires = DateTime.UtcNow.AddHours(1),
                        }
                    },
                    TestId = "Read simple security header - WS-Security 1.0",
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    SecurityHeader = new SecurityHeader
                    {
                        Timestamp = new Timestamp
                        {
                            Created = DateTime.UtcNow,
                            Expires = DateTime.UtcNow.AddHours(1),
                        }
                    },
                    TestId = "Read simple security header - WS-Security 1.1",
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    SecurityHeader = new SecurityHeader
                    {
                        Timestamp = new Timestamp
                        {
                            Created = DateTime.UtcNow,
                            Expires = DateTime.UtcNow.AddHours(1),
                        },
                        AdditionalXmlElements =
                        {
                            WsSecurityReferenceXml.RandomElement
                        }
                    },
                    TestId = "Read additional element xml - WS-Security 1.1",
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    TestId = "Reader null",
                    ExpectedExceptionType =  typeof(ArgumentNullException),
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    TestId = "Context null",
                    SerializationContext = null,
                    ExpectedExceptionType =  typeof(ArgumentNullException),
                },
                new WsSecurityTestData(WsSecurityConstants.WsSecurity11, WsSecurityUtilityConstants.SecurityUtility10)
                {
                    TestId = "Incorrect reader location",
                    Reader = WsSecurityReferenceXml.RandomElementReader,
                    ExpectedExceptionType =  typeof(XmlReadException),
                    ExpectedError = nameof(LogMessages.IDX15011)
                }
            };
        }
    }
}
