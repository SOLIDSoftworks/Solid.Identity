using Solid.IdentityModel.Tokens.Saml2.Metadata;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Xunit;

namespace Solid.IdentityModel.FederationMetadata.Tests
{
    public class Saml2MetadataSerializerTests
    {
        private TestMetadataSerializer _serializer;

        public Saml2MetadataSerializerTests()
        {
            _serializer = new TestMetadataSerializer();
        }

        [Theory]
        [InlineData(ExampleMetadata.SignedExampleSecurityTokenService, true, 1)]
        [InlineData(ExampleMetadata.UnsignedExampleSpSso, false, 1)]
        [InlineData(ExampleMetadata.UnsignedExample2Roles, false, 2)]
        public void ShouldReadExample(string xml, bool signed, int roleCount)
        {
            var reader = CreateReader(xml);
            var metadata = _serializer.ReadMetadata(reader);
            Assert.IsType<EntityDescriptor>(metadata);
            Assert.True(reader.EOF);
            if (signed) Assert.NotNull(metadata.Signature);
            else Assert.Null(metadata.Signature);
            Assert.Equal(roleCount, metadata.Items.Count);
        }

        [Fact]
        public void ShouldReadEntityDescriptor()
        {
            var xml = @"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></EntityDescriptor>";
            
            var reader = CreateReader(xml);
            var metadata = _serializer.ReadMetadata(reader);
            Assert.IsType<EntityDescriptor>(metadata);
        }

        [Fact]
        public void ShouldReadEntitiesDescriptor()
        {
            var xml = @"<EntitiesDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></EntitiesDescriptor>";

            var reader = CreateReader(xml);
            var metadata = _serializer.ReadMetadata(reader);
            Assert.IsType<EntitiesDescriptor>(metadata);
        }

        [Fact]
        public void ShouldReadId()
        {
            var guid = Guid.NewGuid();
            var expected = $"_{guid}";
            var xml = @$"<EntityDescriptor ID=""{expected}"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></EntityDescriptor>";

            var reader = CreateReader(xml);
            var metadata = _serializer.ReadMetadata(reader);
            Assert.Equal(expected, metadata.Id);
        }

        [Fact]
        public void ShouldReadEntityId()
        {
            var guid = Guid.NewGuid();
            var expected = new Uri($"urn:{guid}");
            var xml = @$"<EntityDescriptor entityID=""{expected}"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></EntityDescriptor>";


            var reader = CreateReader(xml);
            var metadata = _serializer.ReadMetadata(reader);
            var entity = metadata as EntityDescriptor;
            Assert.Equal(expected, entity.EntityId);
        }

        [Fact]
        public void ShouldReadCacheDuration()
        {
            var expected = TimeSpan.FromMinutes(144);
            var serialized = XmlConvert.ToString(expected);
            var xml = @$"<EntityDescriptor cacheDuration=""{serialized}"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></EntityDescriptor>";

            var reader = CreateReader(xml);
            var metadata = _serializer.ReadMetadata(reader);
            Assert.NotNull(metadata.CacheDuration);
            Assert.Equal(expected, metadata.CacheDuration.Value);
        }

        [Fact]
        public void ShouldNotCrashOnInvalidCacheDuration()
        {
            var xml = @$"<EntityDescriptor cacheDuration=""invalid"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></EntityDescriptor>";

            var reader = CreateReader(xml);
            _ = _serializer.ReadMetadata(reader);
        }

        [Fact]
        public void ShouldReadValidUntil()
        {
            var expected = DateTime.UtcNow.AddDays(1);
            var serialized = XmlConvert.ToString(expected, XmlDateTimeSerializationMode.Utc);
            var xml = @$"<EntityDescriptor validUntil=""{serialized}"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></EntityDescriptor>";

            var reader = CreateReader(xml);
            var metadata = _serializer.ReadMetadata(reader);
            Assert.NotNull(metadata.ValidUntil);
            Assert.Equal(expected, metadata.ValidUntil.Value);
        }

        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:1.1:protocol")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:protocol")]
        [InlineData("urn:oasis:names:tc:SAML:1.1:protocol urn:oasis:names:tc:SAML:2.0:protocol")]
        public void ShouldReadProtocolEnumeration(string protocolSupportEnumeration)
        {
            var expected = protocolSupportEnumeration.Split(' ').Select(protocol => new Uri(protocol));
            var xml = @$"<RoleDescriptor protocolSupportEnumeration=""{protocolSupportEnumeration}"" xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""></RoleDescriptor>";

            var reader = CreateReader(xml);
            var role = _serializer.ReadRoleDescriptor(reader);
            Assert.Equal(expected, role.ProtocolsSupported);
        }

        private XmlDictionaryReader CreateReader(string xml)
        {
            var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(xml));
            var reader = XmlReader.Create(stream);
            reader.MoveToContent();
            return XmlDictionaryReader.CreateDictionaryReader(reader);
        }

        class TestMetadataSerializer : Saml2MetadataSerializer
        {
            public RoleDescriptor ReadRoleDescriptor(XmlReader reader)
            {
                if (TryReadRoleDescriptor(CreateXmlDictionaryReader(reader), out var role))
                    return role;
                return null;
            }

            private XmlDictionaryReader CreateXmlDictionaryReader(XmlReader reader) => XmlDictionaryReader.CreateDictionaryReader(reader);
        }
    }
}
