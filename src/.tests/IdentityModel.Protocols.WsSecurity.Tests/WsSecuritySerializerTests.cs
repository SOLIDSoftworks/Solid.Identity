using System;
using System.IdentityModel.Tokens;
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