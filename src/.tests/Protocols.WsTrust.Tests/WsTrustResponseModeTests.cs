using System;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Solid.Extensions.AspNetCore.Soap;
using Solid.IdentityModel.Protocols.WsTrust;
using Xunit;

namespace Solid.Identity.Protocols.WsTrust.Tests;

public class WsTrustResponseModeTests
{
    [Fact]
    public async Task February2005IssueKeepsCollectionResponse()
    {
        var registrations = new ServiceCollection();
        registrations.ConfigureWsTrust(options => options.Issuer = "urn:test:issuer");
        registrations.AddSingleton<ISoapContextAccessor>(_ => null);
        registrations.AddLogging();
        using var services = registrations.AddWsTrust(_ => { }).BuildServiceProvider();
        var service = new ResponseModeService(services.GetRequiredService<WsTrustSerializerFactory>(),
            services.GetRequiredService<IOptionsMonitor<WsTrustOptions>>());
        var version = WsTrustConstants.TrustFeb2005;
        using var request = Message.CreateMessage(MessageVersion.Default, version.Actions.IssueRequest);

        using var response = await service.TrustFeb2005IssueAsync(request);

        Assert.Equal(version.Actions.IssueResponse, response.Headers.Action);
        using var reader = response.GetReaderAtBodyContents();
        Assert.Equal(WsTrustElements.RequestSecurityTokenResponseCollection, reader.LocalName);
        Assert.Equal(2, new WsTrustSerializer().ReadResponse(reader).RequestSecurityTokenResponseCollection.Count);
    }

    private sealed class ResponseModeService : WsTrustService
    {
        public ResponseModeService(WsTrustSerializerFactory serializerFactory, IOptionsMonitor<WsTrustOptions> options)
            : base(NullLogger<WsTrustService>.Instance, null, null, serializerFactory, options) { }

        protected override ValueTask<DispatchContext> CreateDispatchContextAsync(Message requestMessage, string requestAction,
            string responseAction, WsTrustConstants constants)
        {
            var response = new WsTrustResponse();
            response.RequestSecurityTokenResponseCollection.Add(new RequestSecurityTokenResponse { Context = "urn:first" });
            response.RequestSecurityTokenResponseCollection.Add(new RequestSecurityTokenResponse { Context = "urn:second" });
            return new(new DispatchContext
            {
                MessageVersion = requestMessage.Version,
                ResponseAction = responseAction,
                ResponseMessage = response
            });
        }

        protected override ValueTask DispatchRequestAsync(DispatchContext context, WsTrustConstants constants)
            => ValueTask.CompletedTask;
    }
}
