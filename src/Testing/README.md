# Solid.Testing [![License](https://img.shields.io/github/license/mashape/apistatus.svg)](https://en.wikipedia.org/wiki/MIT_License) ![Solid.Testing on nuget](https://img.shields.io/nuget/vpre/Solid.Testing.Core)

Solid.Testing is a library used for integration testing and system testing of web apis. It's been designed to be used with AspNetCore and ASP.Net OWIN. It uses Solid.Http internally to perform HTTP requests to an in memory host.

## Packages
* [Solid.Testing.AspNetCore](https://www.nuget.org/packages/Solid.Testing.AspNetCore)
* [Solid.Testing.AspNetCore.Extensions.Https](https://www.nuget.org/packages/Solid.Testing.AspNetCore.Extensions.Https)
* [Solid.Testing.Core](https://www.nuget.org/packages/Solid.Testing.Core)

## Usage
TestingServer is a class that wraps an in-memory instance of your api, whether it's AspNetCore 3.1 or ASP.Net OWIN. A random port is chosen for the in-memory host and a client set up internally for communication. Once you have an instance of the TestingServer, you can perform requests and assert them using a fluid interface.

``` csharp

[Fact]
public async Task ShouldRespondWithTwoValues()
{
    // This '_server' member is an instance of TestingServer
    await _server
        // This is the fluent interface from Solid.Http 
        .GetAsync("values")
        .WithHeader("my-header", "my-header-value")

        // This is the fluent interface from Solid.Testing
        .ShouldRespondSuccessfully()
        .Should(async response =>
        {
            using(var stream = await response.ReadAsStreamAsync())
            {
                var values = JsonSerializer.Deserialize<IEnumerable<string>>(stream);
                // We use xUnit internally, so we use it for our examples. However, any unit test framework can work.
                Assert.Collection(
                  values,
                  value => Assert.Equal("value1", value),
                  value => Assert.Equal("value2", value)
                );
            }
        })
    ;
}
```

### Assertion methods

Methods that extend _Solid.Testing.Models.Assertion_ or _Solid.Http.ISolidHttpRequest_ start with the __Should__ prefix. For convenience, they are all in the _Solid.Http_ namespace.

### Building the TestingServer
To build a TestingServer, you need to use the TestingServerBuilder. There are different extension methods for AspNetCore and ASP.Net OWIN. It's also possible to extend this to another self-hosted http framework.

#### AspNetCore
The basic TestingServer builder method is pretty simple.

```cli
> dotnet add package Solid.Testing.AspNetCore
```

```csharp
private TestingServer BuildTestingServer()
{
    return new TestingServerBuilder()
        .AddAspNetCoreHostFactory()
        // This is the startup class for your AspNetCore application
        .AddStartup<Startup>()
        .Build()
    ;
}
```

#### AspNetCore with https
You can also have a TestingServer which hosts the service with https.

```cli
> dotnet add package Solid.Testing.AspNetCore.Extensions.Https
```

```csharp
private TestingServer BuildTestingServer()
{
    return new TestingServerBuilder()
        .AddAspNetCoreHttpsHostFactory()
        // This is the startup class for your AspNetCore application
        .AddStartup<Startup>()
        .Build()
    ;
}
```

#### ASP.Net OWIN
If you use ASP.Net OWIN, there is an extension for the TestingServerBuilder that will host your service.

```cli
> dotnet add package Solid.Testing.Owin
```

```csharp
private TestingServer BuildTestingServer()
{
    return new TestingServerBuilder()
        .AddOwinHostFactory()
        // This is the startup class for your ASP.Net OWIN application
        .AddStartup<Startup>()
        .Build()
    ;
}
```

#### Adding more advanced customization
There are multiple things that you can do to change the TestingServer. You can add services which it uses internally. These could, for example, be the services that Solid.Http is using for communication.

```csharp
private TestingServer BuildTestingServer()
{
    return new TestingServerBuilder()
        .AddAspNetCoreHostFactory(webHostBuilder =>
        {
            webHostBuilder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                var configuration = new Dictionary<string, string>()
                {
                    { "My__Configuration__Key", "myvalue"}
                };
                // Add custom configuration for your AspNetCore application.
                configurationBuilder.AddInMemoryCollection(configuration);
            });
        })
        .AddTestingServices(services => 
        {
            services.AddSingleton<IHttpClientFactory, MyCustomHttpClientFactory>();
            services.ConfigureSolidHttp(builder =>
            {
                // Use Newtonsoft.Json instead of System.Text.Json
                // This is in the Solid.Http.NewtonsoftJson package
                builder.AddNewtonsoftJson();
            });
        })
        // This is the startup class for your AspNetCore application
        .AddStartup<Startup>()
        .Build()
    ;
}
```