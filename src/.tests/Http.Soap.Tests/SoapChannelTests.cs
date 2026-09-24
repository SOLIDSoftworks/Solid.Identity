using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solid.Http.Soap.Tests.Host;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Http.Soap.Tests;

public class SoapChannelTests : IClassFixture<TestingServerFixture<Startup>>
{
    private readonly TestingServerFixture<Startup> _fixture;

    public SoapChannelTests(TestingServerFixture<Startup> fixture, ITestOutputHelper output)
    {
        fixture.SetOutput(output);
        _fixture = fixture;
    }
    
    [Theory]
    [MemberData(nameof(ShouldCallEchoTestData))]
    public void ShouldCallEcho(ShouldCallEchoData data)
    {
        var services = new ServiceCollection()
            .AddSolidHttpCore()
            .BuildServiceProvider();
        var factory = services.GetService<ISolidHttpClientFactory>();
        var options = new ProxyOptions<IEchoServiceContract>
        {
            MessageVersion =  data.MessageVersion,
            Address = new EndpointAddress(new Uri(_fixture.TestingServer.BaseAddress, data.Path))
        };
        var channel = factory.CreateProxy<IEchoServiceContract>(new BasicHttpBinding(), options);

        var expected = Guid.NewGuid().ToString();
        var value = channel.Echo(expected);
        Assert.Equal(expected, value);
    }

    public class ShouldCallEchoData
    {
        public MessageVersion MessageVersion { get; set; }
        public string Path { get; set; }
    }

    public static readonly IEnumerable<object[]> ShouldCallEchoTestData =
    [
        [ new ShouldCallEchoData { MessageVersion = MessageVersion.Default, Path = "/echo"}],
        [ new ShouldCallEchoData { MessageVersion = MessageVersion.Soap11, Path = "/echo1"}],
        [ new ShouldCallEchoData { MessageVersion = MessageVersion.Soap11WSAddressingAugust2004, Path = "/echo2"}],
        [ new ShouldCallEchoData { MessageVersion = MessageVersion.Soap12WSAddressingAugust2004, Path = "/echo3"}],
        [ new ShouldCallEchoData { MessageVersion = MessageVersion.Soap12WSAddressing10, Path = "/echo4"}],
    ];
}