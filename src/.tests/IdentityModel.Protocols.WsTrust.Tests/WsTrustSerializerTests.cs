using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Xml;
using Solid.IdentityModel.Protocols.WsFed;
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust.Tests.Utilities;
using Xunit;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Solid.IdentityModel.Protocols.WsTrust.Tests
{
    public class WsTrustSerializerTests
    {
        [Fact]
        public void Constructors()
        {
            var serializer = new WsTrustSerializer();

            Assert.Equal(2, serializer.SecurityTokenHandlers.Count);
        }

        [Theory, MemberData(nameof(ReadBinarySecretTestCases))]
        public void ReadBinarySecret(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadBinarySecret", theoryData);
            try
            {
                var binarySecret = WsTrustSerializer.ReadBinarySecret(theoryData.Reader, theoryData.WsSerializationContext);
                IdentityComparer.AreEqual(binarySecret, theoryData.BinarySecret, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadBinarySecretTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        TestId = "ReaderNull",
                        WsSerializationContext = new WsSerializationContext(WsTrustConstants.Trust13)
                    },
                    new WsTrustTheoryData(WsTrustConstants.TrustFeb2005)
                    {
                        BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.TrustFeb2005.BinarySecretTypes.AsymmetricKey),
                        Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.TrustFeb2005, WsTrustConstants.TrustFeb2005.BinarySecretTypes.AsymmetricKey, KeyingMaterial.SelfSigned2048_SHA256),
                        TestId = "TrustFeb2005"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey),
                        Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust13, WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey, KeyingMaterial.SelfSigned2048_SHA256),
                        TestId = "Trust13"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust14)
                    {
                        BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust14.BinarySecretTypes.AsymmetricKey),
                        Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust14, WsTrustConstants.Trust14.BinarySecretTypes.AsymmetricKey, KeyingMaterial.SelfSigned2048_SHA256),
                        TestId = "Trust14"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX15017:", typeof(System.Xml.XmlException)),
                        BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey),
                        Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust13, WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey, "xxx"),
                        TestId = "EncodingError"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust14)
                    {
                        ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX15011:"),
                        BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey),
                        Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust13, WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey, KeyingMaterial.SelfSigned2048_SHA256),
                        TestId = "Trust13_14"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException("IDX15011:"),
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement"
                    }
                };
            }
        }
        

        [Theory, MemberData(nameof(ReadBinaryExchangeTestCases))]
        public void ReadBinaryExchange(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadBinaryExchange", theoryData);
            try
            {
                var binaryExchange = WsTrustSerializer.ReadBinaryExchange(theoryData.Reader, theoryData.WsSerializationContext);
                IdentityComparer.AreEqual(binaryExchange, theoryData.BinaryExchange, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadBinaryExchangeTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        TestId = "ReaderNull",
                        WsSerializationContext = new WsSerializationContext(WsTrustConstants.Trust13)
                    },
                    new WsTrustTheoryData(WsTrustConstants.TrustFeb2005)
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity10.EncodingTypes.Base64),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.TrustFeb2005, WsSecurityConstants.WsSecurity10.EncodingTypes.Base64, "guid", Convert.ToBase64String(KeyingMaterial.SharedKey)),
                        TestId = "TrustFeb2005_Base64"
                    },
                    new WsTrustTheoryData(WsTrustConstants.TrustFeb2005)
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity10.EncodingTypes.HexBinary),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.TrustFeb2005, WsSecurityConstants.WsSecurity10.EncodingTypes.HexBinary, "guid", Convert.ToHexString(KeyingMaterial.SharedKey)),
                        TestId = "TrustFeb2005_HexBinary"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.Base64),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.Trust13, WsSecurityConstants.WsSecurity11.EncodingTypes.Base64, "guid", Convert.ToBase64String(KeyingMaterial.SharedKey)),
                        TestId = "Trust13_Base64"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.HexBinary),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.Trust13, WsSecurityConstants.WsSecurity11.EncodingTypes.HexBinary, "guid", Convert.ToHexString(KeyingMaterial.SharedKey)),
                        TestId = "Trust13_HexBinary"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust14)
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.Base64),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.Trust14, WsSecurityConstants.WsSecurity11.EncodingTypes.Base64, "guid", Convert.ToBase64String(KeyingMaterial.SharedKey)),
                        TestId = "Trust14_Base64"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust14)
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.HexBinary),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.Trust14, WsSecurityConstants.WsSecurity11.EncodingTypes.HexBinary, "guid", Convert.ToHexString(KeyingMaterial.SharedKey)),
                        TestId = "Trust14_HexBinary"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX15017:", typeof(System.Xml.XmlException)),
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.HexBinary),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.Trust13, WsSecurityConstants.WsSecurity11.EncodingTypes.HexBinary, "guid", "xxx"),
                        TestId = "EncodingError"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust14)
                    {
                        ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX15011:"),
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.Base64),
                        Reader = ReferenceXml.GetBinaryExchangeReader(WsTrustConstants.Trust13, WsSecurityConstants.WsSecurity11.EncodingTypes.Base64, "guid", Convert.ToBase64String(KeyingMaterial.SharedKey)),
                        TestId = "Trust13_14"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException("IDX15011:"),
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(ReadClaimsTestCases))]
        public void ReadClaims(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadClaims", theoryData);

            try
            {
                var claims = theoryData.WsTrustSerializer.ReadClaims(theoryData.Reader, theoryData.WsSerializationContext);
                theoryData.ExpectedException.ProcessNoException(context);
                IdentityComparer.AreEqual(claims, theoryData.Claims, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadClaimsTestCases
        {
            get
            {
                var theoryData = new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull",
                        WsSerializationContext = null
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        Reader = null,
                        TestId = "ReaderNull",
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException(),
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement",
                    }
                };

                return theoryData;
            }
        }

        [Theory, MemberData(nameof(ReadLifetimeTestCases))]
        public void ReadLifetime(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadLifetime", theoryData);
            try
            {
                var lifetime = WsTrustSerializer.ReadLifetime(theoryData.Reader, theoryData.WsSerializationContext);
                IdentityComparer.AreEqual(lifetime, theoryData.Lifetime, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadLifetimeTestCases
        {
            get
            {
                DateTime created = DateTime.UtcNow;
                DateTime expires = created + TimeSpan.FromDays(1);
                Lifetime lifetime = new Lifetime(created, expires);

                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        TestId = "ReaderNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.TrustFeb2005)
                    {
                        Lifetime = lifetime,
                        Reader = ReferenceXml.GetLifeTimeReader(WsTrustConstants.TrustFeb2005, created, expires),
                        TestId = "TrustFeb2005"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        Lifetime = lifetime,
                        Reader = ReferenceXml.GetLifeTimeReader(WsTrustConstants.Trust13, created, expires),
                        TestId = "Trust13"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust14)
                    {
                        Lifetime = lifetime,
                        Reader = ReferenceXml.GetLifeTimeReader(WsTrustConstants.Trust14, created, expires),
                        TestId = "Trust14"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException("IDX15011:"),
                        Lifetime = lifetime,
                        Reader = ReferenceXml.GetLifeTimeReader(WsTrustConstants.Trust14, created, expires),
                        TestId = "Trust14_13"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException("IDX15017:", typeof(FormatException)),
                        Lifetime = lifetime,
                        Reader = ReferenceXml.GetLifeTimeReader(WsTrustConstants.Trust13, XmlConvert.ToString(created, XmlDateTimeSerializationMode.Utc), "xxx"),
                        TestId = "CreateParseError"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException("IDX15017:", typeof(FormatException)),
                        Lifetime = lifetime,
                        Reader = ReferenceXml.GetLifeTimeReader(WsTrustConstants.Trust13, "xxx", XmlConvert.ToString(expires, XmlDateTimeSerializationMode.Utc)),
                        TestId = "ExpireParseError"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException("IDX15011:"),
                        Lifetime = lifetime,
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(ReadOnBehalfOfTestCases))]
        public void ReadOnBehalfOf(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadOnBehalfOf", theoryData);

            try
            {
                var onBehalfOf = theoryData.WsTrustSerializer.ReadOnBehalfOf(theoryData.Reader, theoryData.WsSerializationContext);
                theoryData.ExpectedException.ProcessNoException(context);
                IdentityComparer.AreEqual(onBehalfOf, theoryData.OnBehalfOf, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadOnBehalfOfTestCases
        {
            get
            {
                var theoryData = new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        Reader = ReferenceXml.GetRequestSecurityTokenReader(WsTrustConstants.Trust13, ReferenceXml.Saml2Valid),
                        TestId = "SerializationContextNull",
                        WsSerializationContext = null
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        Reader = null,
                        TestId = "ReaderNull",
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException(),
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement",
                    }
                };

                return theoryData;
            }
        }

        [Theory, MemberData(nameof(ReadRequestedAttachedReferenceTestCases))]
        public void ReadRequestedAttachedReference(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadRequestedAttachedReference", theoryData);

            try
            {
                var attachedReference = WsTrustSerializer.ReadRequestedAttachedReference(theoryData.Reader, theoryData.WsSerializationContext);
                theoryData.ExpectedException.ProcessNoException(context);
                IdentityComparer.AreEqual(attachedReference, theoryData.Reference, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadRequestedAttachedReferenceTestCases
        {
            get
            {
                var theoryData = new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        Reader = ReferenceXml.GetRequestSecurityTokenReader(WsTrustConstants.Trust13, ReferenceXml.Saml2Valid),
                        TestId = "SerializationContextNull",
                        WsSerializationContext = null
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        Reader = null,
                        TestId = "ReaderNull",
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException(),
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement",
                    }
                };

                return theoryData;
            }
        }

        [Theory, MemberData(nameof(ReadRequestedSecurityTokenTestCases))]
        public void ReadRequestedSecurityToken(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadRequestedSecurityToken", theoryData);

            try
            {
                var requestedSecurityToken = WsTrustSerializer.ReadRequestedSecurityToken(theoryData.Reader, theoryData.WsSerializationContext);
                theoryData.ExpectedException.ProcessNoException(context);
                IdentityComparer.AreEqual(requestedSecurityToken, theoryData.RequestedSecurityToken, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadRequestedSecurityTokenTestCases
        {
            get
            {
                var theoryData = new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull",
                        WsSerializationContext = null
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        Reader = null,
                        TestId = "ReaderNull",
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException(),
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement",
                    }
                };

                return theoryData;
            }
        }

        [Theory, MemberData(nameof(ReadUnattachedReferenceTestCases))]
        public void ReadUnattachedReference(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.ReadUnattachedReference", theoryData);

            try
            {
                var unattachedReference = WsTrustSerializer.ReadRequestedUnattachedReference(theoryData.Reader, theoryData.WsSerializationContext);
                theoryData.ExpectedException.ProcessNoException(context);
                IdentityComparer.AreEqual(unattachedReference, theoryData.Reference, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> ReadUnattachedReferenceTestCases
        {
            get
            {
                var theoryData = new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(ReferenceXml.RandomElementReader)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull",
                        WsSerializationContext = null
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("reader"),
                        Reader = null,
                        TestId = "ReaderNull",
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.XmlReadException(),
                        Reader = ReferenceXml.RandomElementReader,
                        TestId = "ReaderNotOnCorrectElement",
                    }
                };

                return theoryData;
            }
        }

        [Theory, MemberData(nameof(WriteBinarySecretTestCases))]
        public void WriteBinarySecret(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteBinarySecret", theoryData);
            try
            {
                WsTrustSerializer.WriteBinarySecret(theoryData.Writer, theoryData.WsSerializationContext, theoryData.BinarySecret);
                //IdentityComparer.AreEqual(binarySecret, theoryData.BinarySecret, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteBinarySecretTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey),
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.BinarySecretTypes.AsymmetricKey),
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("binarySecret"),
                        TestId = "BinarySecretNull"
                    },
                    // new WsTrustTheoryData(WsTrustVersion.Trust13)
                    // {
                    //     BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.WsTrustBinarySecretTypes.AsymmetricKey),
                    //     Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust13, WsTrustConstants.Trust13.WsTrustBinarySecretTypes.AsymmetricKey, KeyingMaterial.SelfSigned2048_SHA256),
                    //     TestId = "Trust13"
                    // },
                    // new WsTrustTheoryData(WsTrustVersion.Trust14)
                    // {
                    //     BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust14.WsTrustBinarySecretTypes.AsymmetricKey),
                    //     Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust14, WsTrustConstants.Trust14.WsTrustBinarySecretTypes.AsymmetricKey, KeyingMaterial.SelfSigned2048_SHA256),
                    //     TestId = "Trust14"
                    // },
                    // new WsTrustTheoryData(WsTrustVersion.Trust13)
                    // {
                    //     ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX30017:", typeof(System.Xml.XmlException)),
                    //     BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.WsTrustBinarySecretTypes.AsymmetricKey),
                    //     Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust13, WsTrustConstants.Trust13.WsTrustBinarySecretTypes.AsymmetricKey, "xxx"),
                    //     TestId = "EncodingError"
                    // },
                    // new WsTrustTheoryData(WsTrustVersion.Trust14)
                    // {
                    //     ExpectedException = new ExpectedException(typeof(XmlReadException), "IDX30011:"),
                    //     BinarySecret = new BinarySecret(Convert.FromBase64String(KeyingMaterial.SelfSigned2048_SHA256), WsTrustConstants.Trust13.WsTrustBinarySecretTypes.AsymmetricKey),
                    //     Reader = ReferenceXml.GetBinarySecretReader(WsTrustConstants.Trust13, WsTrustConstants.Trust13.WsTrustBinarySecretTypes.AsymmetricKey, KeyingMaterial.SelfSigned2048_SHA256),
                    //     TestId = "Trust13_14"
                    // },
                    // new WsTrustTheoryData(WsTrustVersion.Trust13)
                    // {
                    //     ExpectedException = ExpectedException.XmlReadException("IDX30011:"),
                    //     Reader = ReferenceXml.RandomElementReader,
                    //     TestId = "ReaderNotOnCorrectElement"
                    // }
                };
            }
        }

        [Theory, MemberData(nameof(WriteBinaryExchangeTestCases))]
        public void WriteBinaryExchange(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteBinaryExchange", theoryData);
            try
            {
                WsTrustSerializer.WriteBinaryExchange(theoryData.Writer, theoryData.WsSerializationContext, theoryData.BinaryExchange);
                //IdentityComparer.AreEqual(binarySecret, theoryData.BinarySecret, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteBinaryExchangeTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.Base64),
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        BinaryExchange = new BinaryExchange(KeyingMaterial.SharedKey, "guid", WsSecurityConstants.WsSecurity11.EncodingTypes.Base64),
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("binaryExchange"),
                        TestId = "BinaryExchangeNull"
                    },
                };
            }
        }

        [Theory, MemberData(nameof(WriteClaimsTestCases))]
        public void WriteClaims(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteClaims", theoryData);
            try
            {
                WsTrustSerializer.WriteClaims(theoryData.Writer, theoryData.WsSerializationContext, theoryData.Claims);
                //IdentityComparer.AreEqual(claims, theoryData.Claims, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteClaimsTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        Claims = new Claims("http://ClaimsDialect", new List<ClaimType>()),
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        Claims = new Claims("http://ClaimsDialect", new List<ClaimType>()),
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("claims"),
                        TestId = "ClaimsNull"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(WriteLifetimeTestCases))]
        public void WriteLifetime(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteLifetime", theoryData);
            try
            {
                WsTrustSerializer.WriteLifetime(theoryData.Writer, theoryData.WsSerializationContext, theoryData.Lifetime);
                //IdentityComparer.AreEqual(lifetime, theoryData.Lifetime, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteLifetimeTestCases
        {
            get
            {
                DateTime created = DateTime.UtcNow;
                DateTime expires = created + TimeSpan.FromDays(1);
                Lifetime lifetime = new Lifetime(created, expires);

                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        Lifetime = lifetime,
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        Lifetime = lifetime,
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("lifetime"),
                        TestId = "LifetimeNull"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(WriteOnBehalfOfTestCases))]
        public void WriteOnBehalfOf(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteOnBehalfOf", theoryData);
            try
            {
                theoryData.WsTrustSerializer.WriteOnBehalfOf(theoryData.Writer, theoryData.WsSerializationContext, theoryData.OnBehalfOf);
                //IdentityComparer.AreEqual(lifetime, theoryData.Lifetime, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteOnBehalfOfTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        OnBehalfOf = new SecurityTokenElement(new SecurityTokenReference()),
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        OnBehalfOf = new SecurityTokenElement(new SecurityTokenReference()),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("onBehalfOf"),
                        TestId = "OnBehalfOfNull"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(WriteProofEncryptionTestCases))]
        public void WriteProofEncryption(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteProofEncryption", theoryData);
            try
            {
                WsTrustSerializer.WriteProofEncryption(theoryData.Writer, theoryData.WsSerializationContext, theoryData.ProofEncryption);
                //IdentityComparer.AreEqual(lifetime, theoryData.Lifetime, context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteProofEncryptionTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        ProofEncryption = new SecurityTokenElement(new SecurityTokenReference()),
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        ProofEncryption = new SecurityTokenElement(new SecurityTokenReference()),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("proofEncryption"),
                        TestId = "ProofEncryptionNull"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(WriteRequestedAttachedReferenceTestCases))]
        public void WriteRequestedAttachedReference(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteRequestedAttachedReference", theoryData);
            try
            {
                WsTrustSerializer.WriteRequestedAttachedReference(theoryData.Writer, theoryData.WsSerializationContext, theoryData.RequestedAttachedReference);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteRequestedAttachedReferenceTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        RequestedAttachedReference = new SecurityTokenReference(),
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        RequestedAttachedReference = new SecurityTokenReference(),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("securityTokenReference"),
                        TestId = "RequestedAttachedReferenceNull"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(WriteRequestedSecurityTokenTestCases))]
        public void WriteRequestedSecurityToken(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteRequestedSecurityToken", theoryData);
            try
            {
                theoryData.WsTrustSerializer.WriteRequestedSecurityToken(theoryData.Writer, theoryData.WsSerializationContext, theoryData.RequestedSecurityToken);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteRequestedSecurityTokenTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        RequestedSecurityToken = new RequestedSecurityToken(),
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        RequestedSecurityToken = new RequestedSecurityToken(),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("requestedSecurityToken"),
                        TestId = "RequestedSecurityTokenNull"
                    }
                };
            }
        }

        [Theory, MemberData(nameof(WriteRequestedUnattachedReferenceTestCases))]
        public void WriteRequestedUnattachedReference(WsTrustTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.WriteRequestedUnattachedReference", theoryData);
            try
            {
                WsTrustSerializer.WriteRequestedUnattachedReference(theoryData.Writer, theoryData.WsSerializationContext, theoryData.RequestedUnattachedReference);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<WsTrustTheoryData> WriteRequestedUnattachedReferenceTestCases
        {
            get
            {
                return new TheoryData<WsTrustTheoryData>
                {
                    new WsTrustTheoryData(new MemoryStream())
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("serializationContext"),
                        First = true,
                        RequestedAttachedReference = new SecurityTokenReference(),
                        TestId = "SerializationContextNull"
                    },
                    new WsTrustTheoryData(WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("writer"),
                        RequestedAttachedReference = new SecurityTokenReference(),
                        TestId = "WriterNull",
                    },
                    new WsTrustTheoryData(new MemoryStream(), WsTrustConstants.Trust13)
                    {
                        ExpectedException = ExpectedException.ArgumentNullException("securityTokenReference"),
                        TestId = "RequestedUnattachedReferenceNull"
                    }
                };
            }
        }
    }
}
