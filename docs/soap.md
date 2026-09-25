# SOAP services

`Solid.Extensions.AspNetCore.Soap` hosts SOAP services in ASP.NET Core using service contracts and endpoint routing.

```console
dotnet add package Solid.Extensions.AspNetCore.Soap
```

Define a contract and its implementation:

```csharp
[ServiceContract]
public interface IEchoServiceContract
{
    [OperationContract]
    Task<string> EchoAsync(string value);
}

public class EchoService : IEchoServiceContract
{
    public Task<string> EchoAsync(string value) => Task.FromResult(value);
}
```

Register the service and map the endpoint:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRouting()
        .AddSingletonSoapService<IEchoServiceContract, EchoService>();
}

public void Configure(IApplicationBuilder app)
{
    app.UseRouting();
    app.UseEndpoints(endpoints =>
        endpoints.MapSoapService<IEchoServiceContract>("/echo", MessageVersion.Soap11));
}
```

See the [SOAP package guide](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Utility/Extensions.AspNetCore.Soap/README.md) for middleware customization and client channel examples.
