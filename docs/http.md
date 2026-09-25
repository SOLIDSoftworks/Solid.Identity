# HTTP client

`Solid.Http` provides a fluent asynchronous HTTP client. It includes `Solid.Http.Core` and `Solid.Http.Json`.

```console
dotnet add package Solid.Http
```

Register the client with dependency injection:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSolidHttp();
}
```

Inject `ISolidHttpClientFactory` and create a client with a base address. Requests using that client can use relative URLs:

```csharp
var client = factory.CreateWithBaseAddress("https://jsonplaceholder.typicode.com");
var posts = await client.GetAsync("posts").AsMany<Post>();
```

For requests with a body and named URL parameters:

```csharp
await client
    .PutAsync("posts/{id}")
    .WithNamedParameter("id", id)
    .WithJsonContent(post)
    .ExpectSuccess();
```

Add `Solid.Http.Xml` or `Solid.Http.Zip` when you need those formats. See the existing [HTTP overview](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Http/README.md) and the package guides for [Core](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Http/Http.Core/README.md), [JSON](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Http/Http.Json/README.md), [XML](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Http/Http.Xml/README.md), and [ZIP](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Http/Http.Zip/README.md) for configuration and additional examples.
