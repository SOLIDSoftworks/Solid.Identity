# Testing web APIs

`Solid.Testing.AspNetCore` hosts an ASP.NET Core application for integration tests and uses the fluent HTTP client for requests and assertions.

```console
dotnet add package Solid.Testing.AspNetCore
```

Build a testing server with your application startup class:

```csharp
var server = new TestingServerBuilder()
    .AddAspNetCoreHostFactory()
    .AddStartup<Startup>()
    .Build();
```

Use the server to perform a request and assert its response:

```csharp
await server
    .GetAsync("values")
    .ShouldRespondSuccessfully()
    .Should(async response =>
    {
        var content = await response.ReadAsStringAsync();
        Assert.NotEmpty(content);
    });
```

For an HTTPS host, install `Solid.Testing.AspNetCore.Extensions.Https` and use `AddAspNetCoreHttpsHostFactory()`. See the [testing package guide](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Testing/README.md) for host customization and more examples.
