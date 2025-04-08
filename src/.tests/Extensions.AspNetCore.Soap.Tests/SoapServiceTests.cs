using Microsoft.Extensions.Logging;
using Solid.Extensions.AspNetCore.Soap.Tests.Host;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using Solid.Testing.AspNetCore.Extensions.XUnit.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Extensions.AspNetCore.Soap.Tests
{
    public class SoapServiceTests : IClassFixture<SoapTestingServerFixture<Startup>>
    {
        private SoapTestingServerFixture<Startup> _fixture;

        public SoapServiceTests(SoapTestingServerFixture<Startup> fixture, ITestOutputHelper output)
        {
            fixture.SetOutput(output);
            _fixture = fixture;
            _fixture.UpdateConfiguration(builder => builder.SetLogLevel("Solid", LogLevel.Trace), clear: true);
        }

        [Theory]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        public void ShouldDoNonDestructiveLogging(LogLevel level)
        {
            _fixture.UpdateConfiguration(builder => builder.SetDefaultLogLevel(level), clear: true);

            var expected = Guid.NewGuid().ToString();
            var channel = _fixture.CreateChannel<IEchoServiceContract>(path: "echo");
            // mostly just testing that this doesn't explode
            var value = channel.Echo(expected);

            Assert.Equal(expected, value);
        }

        [Theory]
        [MemberData(nameof(GenerateEchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldEcho(TestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var value = channel.Echo(data.Value);

            Assert.Equal(data.Value, value);
        }

        [Theory]
        [MemberData(nameof(GenerateEchoTestData), DisableDiscoveryEnumeration = false)]
        public async Task ShouldAsynchronouslyEcho(TestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var value = await channel.AsynchronousEchoAsync(data.Value);

            Assert.Equal(data.Value, value);
        }

        [Theory]
        [MemberData(nameof(GenerateEchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldOutEcho(TestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            channel.OutEcho(data.Value, out var value);

            Assert.Equal(data.Value, value);
        }

        [Theory]
        [MemberData(nameof(GenerateEchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldWrappedEcho(TestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var wrapper = new EchoWrapper { Value = data.Value };
            var response = channel.WrappedEcho(wrapper);

            Assert.Equal(data.Value, response.Value);
        }

        [Theory]
        [MemberData(nameof(GenerateEchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldWrappedAndOutEcho(TestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var wrapper = new EchoWrapper { Value = data.Value };
            var response = channel.WrappedAndOutEcho(wrapper, out var value);

            Assert.Equal(data.Value, response.Value);
            Assert.Equal(data.Value, value);
        }

        [Theory]
        [MemberData(nameof(GenerateFaultTestDataWithoutNull), DisableDiscoveryEnumeration = false)]
        public void ShouldGetFaultException(TestData data)
        {
            var channel = _fixture.CreateChannel<IFaultServiceContract>(version: data.MessageVersion, path: data.Path);
            var exception = null as Exception;
            try
            {
                channel.ThrowsException(data.Value);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsType<FaultException>(exception);
            var fault = exception as FaultException;
            Assert.NotEqual(data.Value, fault.Message);
        }

        [Theory]
        [MemberData(nameof(GenerateFaultTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldGetFaultContract(TestData data)
        {
            var channel = _fixture.CreateChannel<IFaultServiceContract>(version: data.MessageVersion, path: data.Path);

            var exception = null as Exception;
            try
            {
                channel.ThrowsContractFault(data.Value);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsType<FaultException<TestDetail>>(exception);
            var fault = exception as FaultException<TestDetail>;
            var detail = fault.Detail;
            Assert.Equal(data.Value, detail.Message);
        }

        [Theory]
        [MemberData(nameof(GenerateDetailedFaultTestDataWithoutNull), DisableDiscoveryEnumeration = false)]
        public void ShouldGetFaultExceptionWithExceptionDetails(TestData data)
        {
            var channel = _fixture.CreateChannel<IDetailedFaultServiceContract>(version: data.MessageVersion, path: data.Path);
            var exception = null as Exception;
            try
            {
                channel.ThrowsException(data.Value);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsType<FaultException<ExceptionDetail>>(exception);
            var fault = exception as FaultException<ExceptionDetail>;
            var detail = fault.Detail;
            Assert.Equal(data.Value, fault.Message);
            Assert.Equal(data.Value, detail.Message);
        }

        public static TheoryData<TestData> GenerateEchoTestData() => GenerateTestData("echo");

        public static TheoryData<TestData> GenerateFaultTestData() => GenerateTestData("faults");

        public static TheoryData<TestData> GenerateFaultTestDataWithoutNull() => GenerateTestData("faults", includeNullValues: false);

        public static TheoryData<TestData> GenerateDetailedFaultTestDataWithoutNull() => GenerateTestData("detailedfaults", includeNullValues: false);

        private static TheoryData<TestData> GenerateTestData(string pathPrefix, bool includeNullValues = true)
        {
            var data = new TheoryData<TestData>();
            foreach (var pair in _messageVersions)
            {
                var version = pair.Value;
                var path = $"{pathPrefix}{pair.Key}";
                foreach (var value in _values.Where(v => includeNullValues || v != null))
                {
                    data.Add(new TestData { MessageVersion = version, Path = path, Value = value });
                }
            }
            return data;
        }

        private static IEnumerable<string> _values = new[] { null, "", "expected" };
        private static IDictionary<int, MessageVersion> _messageVersions = new Dictionary<int, MessageVersion>
        {
            { 1, MessageVersion.Soap11 },
            { 2, MessageVersion.Soap11WSAddressingAugust2004 },
            { 3, MessageVersion.Soap12WSAddressingAugust2004  },
            { 4, MessageVersion.Soap12WSAddressing10 },
        };
    }

    public class TestData
    {
        public string Value { get; set; }
        public MessageVersion MessageVersion { get; set; }
        public string Path { get; set; }
    }
}
