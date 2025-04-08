using Solid.IdentityModel.FederationMetadata.WsAddressing;
using Solid.IdentityModel.Tokens.Saml2.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Xunit;

namespace Solid.IdentityModel.FederationMetadata.Tests
{
    public class FederationMetadataSerializerTests
    {
        private TestMetadataSerializer _serializer;

        public FederationMetadataSerializerTests()
        {
            _serializer = new TestMetadataSerializer();
        }

        [Fact]
        public void ShouldReadExampleWithSecurityTokenService()
        {
            var reader = _serializer.CreateXmlDictionaryReader(ExampleMetadata.UnsignedExampleSecurityTokenService);
            var metadata = _serializer.ReadMetadata(reader);
            Assert.IsType<EntityDescriptor>(metadata);
            Assert.True(reader.EOF);
            Assert.Equal(1, metadata.Items.Count);
            Assert.IsType<SecurityTokenServiceDescriptor>(metadata.Items.First());
        }

        [Theory]
        [InlineData(typeof(AttributeServiceDescriptor))]
        [InlineData(typeof(ApplicationServiceDescriptor))]
        [InlineData(typeof(PseudonymServiceDescriptor))]
        [InlineData(typeof(SecurityTokenServiceDescriptor))]
        public void ShouldReadCorrectRoleType(Type descriptorType)
        {
            var typeName = $"{descriptorType.Name.Replace("Descriptor", string.Empty)}Type";
            var xml = @$"
<RoleDescriptor 
  xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
  xmlns:fed=""http://docs.oasis-open.org/wsfed/federation/200706""
  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
  xsi:type=""fed:{typeName}""
>
</RoleDescriptor>";

            var metadata = _serializer.ReadRoleDescriptor(xml);
            Assert.IsType(descriptorType, metadata);
        }

        [Fact]
        public void ShouldRoundTripSecurityTokenService()
        {
            var descriptor = new SecurityTokenServiceDescriptor();
            var securityTokenServiceEndpoint = new EndpointReferenceCollection();
            securityTokenServiceEndpoint.Add(new EndpointReference { Address = new Uri("https://notused") });
            descriptor.SecurityTokenServiceEndpoint.Add(securityTokenServiceEndpoint);
            descriptor.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            var xml = _serializer.WriteRoleDescriptor(descriptor);

            var deserialized = _serializer.ReadRoleDescriptor(xml);

            Assert.IsType<SecurityTokenServiceDescriptor>(deserialized);
            AssertEqual(descriptor, deserialized);
        }

        [Fact]
        public void ShouldRoundTripApplicationService()
        {
            var descriptor = new ApplicationServiceDescriptor();
            var applicationServiceEndpoint = new EndpointReferenceCollection();
            applicationServiceEndpoint.Add(new EndpointReference { Address = new Uri("https://notused") });
            descriptor.ApplicationServiceEndpoint.Add(applicationServiceEndpoint);
            descriptor.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            var xml = _serializer.WriteRoleDescriptor(descriptor);

            var deserialized = _serializer.ReadRoleDescriptor(xml);

            Assert.IsType<ApplicationServiceDescriptor>(deserialized);
            AssertEqual(descriptor, deserialized);
        }

        [Fact]
        public void ShouldRoundTripAttributeService()
        {
            var descriptor = new AttributeServiceDescriptor();
            var attributeServiceEndpoint = new EndpointReferenceCollection();
            attributeServiceEndpoint.Add(new EndpointReference { Address = new Uri("https://notused") });
            descriptor.AttributeServiceEndpoint.Add(attributeServiceEndpoint);
            descriptor.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            var xml = _serializer.WriteRoleDescriptor(descriptor);

            var deserialized = _serializer.ReadRoleDescriptor(xml);

            Assert.IsType<AttributeServiceDescriptor>(deserialized);
            AssertEqual(descriptor, deserialized);
        }

        [Fact]
        public void ShouldRoundTripPseudonymService()
        {
            var descriptor = new PseudonymServiceDescriptor();
            var pseudonymServiceEndpoint = new EndpointReferenceCollection();
            pseudonymServiceEndpoint.Add(new EndpointReference { Address = new Uri("https://notused") });
            descriptor.PseudonymServiceEndpoint.Add(pseudonymServiceEndpoint);
            descriptor.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            var xml = _serializer.WriteRoleDescriptor(descriptor);

            var deserialized = _serializer.ReadRoleDescriptor(xml);

            Assert.IsType<PseudonymServiceDescriptor>(deserialized);
            AssertEqual(descriptor, deserialized);
        }

        private void AssertEqual(object expected, object actual)
        {
            if (expected == null) return;
            Assert.NotNull(actual);
            var type = expected.GetType();
            Assert.IsType(type, actual);

            if(type.IsValueType || type.Equals(typeof(string)))
            {
                Assert.Equal(expected, actual);
            }
            else if (actual is IEnumerable actualEnumerable)
            {
                var actualObjectEnumerable = actualEnumerable.Cast<object>();
                var expectedObjectEnumerable = (expected as IEnumerable).Cast<object>();

                AssertEqual(expectedObjectEnumerable.Count(), actualObjectEnumerable.Count());

                for (var i = 0; i < expectedObjectEnumerable.Count(); i++)
                {
                    var expectedValue = expectedObjectEnumerable.ElementAt(i);
                    var actualValue = actualObjectEnumerable.ElementAt(i);
                    AssertEqual(expectedValue, actualValue);
                }
            }
            else
            {
                foreach (var property in type.GetProperties())
                {
                    var expectedValue = property.GetValue(expected);
                    var actualValue = property.GetValue(actual);
                    AssertEqual(expectedValue, actualValue);
                }

                foreach (var field in type.GetFields())
                {
                    var expectedValue = field.GetValue(expected);
                    var actualValue = field.GetValue(actual);
                    AssertEqual(expectedValue, actualValue);
                }
            }
        }

        class TestMetadataSerializer : FederationMetadataSerializer
        {
            public RoleDescriptor ReadRoleDescriptor(string xml)
            {
                using (var reader = CreateXmlDictionaryReader(xml))
                    return ReadRoleDescriptor(reader);
            }

            public RoleDescriptor ReadRoleDescriptor(XmlDictionaryReader reader)
            {
                if (TryReadRoleDescriptor(reader, out var role))
                    return role;
                return null;
            }

            public string WriteRoleDescriptor(RoleDescriptor roleDescriptor)
                => WriteElement(writer => WriteRoleDescriptor(writer, roleDescriptor));

            private string WriteElement(Action<XmlDictionaryWriter> action)
            {
                using (var stream = new MemoryStream())
                {
                    using (var inner = XmlWriter.Create(stream, new XmlWriterSettings { OmitXmlDeclaration = true, CloseOutput = false }))
                    using (var writer = XmlDictionaryWriter.CreateDictionaryWriter(inner))
                    {
                        action(writer);
                    }
                    var buffer = stream.GetBuffer();
                    return Encoding.UTF8.GetString(buffer);
                }
            }

            public XmlDictionaryReader CreateXmlDictionaryReader(string xml)
            {
                var stream = new MemoryStream(new UTF8Encoding(false).GetBytes(xml));
                var reader = XmlReader.Create(stream);
                return XmlDictionaryReader.CreateDictionaryReader(reader);
            }
        }
    }
}
