# Hosting a WS-Trust service

This guide describes an integration pattern used by an ASP.NET Core WS-Trust application: a SOAP endpoint issues tokens for configured relying parties, validates incoming credentials and tokens, and enriches outgoing claims. The examples use illustrative names and assume certificates, partner identifiers, and credentials are supplied by the application's configuration or secret provider.

## Packages and service registration

The service uses `Solid.Identity.Protocols.WsTrust` for WS-Trust and WS-Security, `Solid.Http` for outbound requests, and the SOAP hosting integration provided by the WS-Trust builder. An application can configure options and register the async WS-Trust 1.3 contract:

```csharp
services.AddRouting();
services.AddSolidHttp();

services.Configure<WsTrustOptions>(options =>
{
    // Obtain the signing key from your certificate provider at runtime.
    options.Issuer = issuerId;
    options.DefaultSigningKey = signingKey;

    options.AddIdentityProvider(issuerId, provider =>
    {
        provider.Name = "Example identity provider";
        provider.SecurityKeys.Add(trustedSigningKey);
    });

    options.AddRelyingParty(appliesTo, party =>
    {
        party.Name = "Example relying party";
        party.RequiredClaims.Add(ClaimTypes.NameIdentifier);
        party.ValidateRequestedTokenType = true;
        party.SupportedTokenTypes.Add(Saml2Constants.Saml2TokenProfile11);
    });
});

services.AddWsTrust(builder => builder
    .Configure(options => options.AddSaml2SecurityTokenHandler())
    .AddPasswordValidator<ExamplePasswordValidator>()
    .AddWsTrust13AsyncContract());
```

The incoming issuer and the requested `AppliesTo` value select the identity provider and relying party. Configure trusted signing keys for token validation and a signing key for issued tokens; do not put private keys or passwords in source code. Applications that construct providers and parties dynamically can register `AddIdentityProviderStore<T>()` and `AddRelyingPartyStore<T>()` to resolve them through custom stores. The application's sample options, store types, and validator types above are illustrative implementations, not built-in types.

## Map the SOAP endpoint

Map the contract after ASP.NET Core routing is enabled:

```csharp
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapWsTrust13AsyncService("/trust/13",
        WsTrustContractOptions.DefaultTrust13Contract);
});
```

`MapWsTrust13AsyncService` wires the SOAP endpoint to the WS-Trust 1.3 contract and WS-Security processing. Choose an application-specific path; if the app uses `UsePathBase`, take the path base into account when addressing the service.

## Extend token and claim processing

The WS-Trust builder accepts a custom `SecurityTokenService`, password validator, token handlers, incoming claim mappers, relying-party claim stores, and token-type claim stores. A service can compose these extensions as follows:

```csharp
services.AddWsTrust(builder => builder
    .Configure(options =>
    {
        options.AddSaml2SecurityTokenHandler();
        options.AddSecurityTokenHandler(serviceProvider =>
            serviceProvider.GetRequiredService<ExampleTokenHandler>());
    })
    .AddSecurityTokenService<ExampleSecurityTokenService>()
    .AddPasswordValidator<ExamplePasswordValidator>()
    .AddIdentityProviderStore<ExampleIdentityProviderStore>()
    .AddRelyingPartyStore<ExampleRelyingPartyStore>()
    .AddIncomingClaimMapper<ExampleClaimMapper>()
    .AddRelyingPartyClaimStore<ExamplePartyClaimStore>()
    .AddTokenTypeClaimStore<ExampleTokenTypeClaimStore>()
    .AddWsTrust13AsyncContract());
```

For example, a claim mapper can normalize claims from incoming tokens before issuance; a relying-party claim store adds claims for a specific audience, while a token-type claim store contributes claims appropriate to the issued token format. `AddSecurityTokenHandler` supports custom token formats alongside SAML. Register only the extensions your application needs.

## Outbound HTTP and integration testing

The service can inject `ISolidHttpClientFactory` after `AddSolidHttp()` to call a credential-validation service or health endpoint. Build request URLs from configuration and keep credentials out of logs and examples:

```csharp
var client = httpClientFactory.Create();
var response = await client.GetAsync(healthEndpoint, cancellationToken);
```

Integration tests can use `Solid.Testing.AspNetCore.Extensions.Https`, `Solid.Testing.AspNetCore.Extensions.XUnit.Soap`, and `Solid.Testing.Certificates` to host the service and generate test-only certificates. A `SoapTestingServerFixture<TStartup>` can configure the host and inject test providers and relying parties. Test clients can use `WsTrustChannelFactory` with `WsTrustUserNameBinding` or `WsTrustIssuedTokenBinding`, set `TrustVersion` to `WsTrustVersion.Trust13`, and send requests to the hosted endpoint. Supply credentials through test setup rather than embedding real account data.

See the [HTTP client](http.md) and [SOAP services](soap.md) guides for the underlying client and hosting primitives.
