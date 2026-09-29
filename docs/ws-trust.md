# Hosting a WS-Trust service

## RST/RSTR wire coverage

`WsTrustSerializer` reads and writes the following parameters in WS-Trust February 2005 and 1.3. The model is a wire-format capability; the default STS currently handles Issue requests only. SOAP contract availability depends on the registered binding.

| Parameter family | Feb 2005 RST / RSTR | 1.3 RST / RSTR | Notes |
| --- | --- | --- | --- |
| RequestType, TokenType, KeyType, KeySize, Context | yes / TokenType, KeyType, KeySize, Context | same | Context is on each RSTR, including responses in a collection. |
| Lifetime, Entropy (BinarySecret), AppliesTo | yes / yes | yes / yes | Encrypted entropy belongs to the token/key writer work (#9). |
| Claims, OnBehalfOf, UseKey, ProofEncryption, PolicyReference, AdditionalContext | yes / — | yes / — | OnBehalfOf and UseKey support is limited to their existing token/reference implementations; ProofEncryption writing is tracked by #6. |
| Algorithm choices (canonicalization, encryption, EncryptWith, SignWith, computed key) | yes / encryption algorithm | yes / encryption algorithm | SignatureAlgorithm and KeyWrapAlgorithm are also round-tripped in requests and responses. |
| AuthenticationType, Forwardable, Delegatable, AllowPostdating, Renewing | yes / — | yes / — | Renewing uses `Allow` and `OK` attributes. |
| RenewTarget, CancelTarget, ValidateTarget | yes / — | yes / — | Typed `TokenTarget` contains a token XML element or a WS-Security 1.0 KeyIdentifier reference. |
| Status, RequestedTokenCancelled | — / yes | — / yes | `Status` requires `Code` and permits `Reason`; cancellation is an empty marker. |
| RequestedSecurityToken, RequestedProofToken, attached/unattached references, Authenticator | — / yes | — / yes | Supported token/key formats are limited by registered handlers and related issues #5 and #9. |
| BinaryExchange | yes / yes | yes / yes | Base64 and hex encodings; the bounded 1.3 echo negotiation profile is described below. |
| SecondaryParameters | unavailable | yes / — | Secondary fields remain separate from primary fields; explicit primary parameters take precedence. Nested secondary blocks and secondary RequestType are rejected. |
| ActAs, DelegateTo, Issuer, Participants | unsupported | unsupported | Known unsupported trust parameters fail explicitly instead of being discarded. |

Other-namespace extension XML is retained in request and response `AdditionalXmlElements`. Known but unsupported WS-Trust elements fail on read. The 1.4 namespace is recognized by the serializer but does not represent a tested 1.4 binding or complete profile.

### Binary challenge exchange (WS-Trust 1.3)

The default STS supports the `urn:solid:identity:wstrust:binary-exchange:echo` ValueType for an authenticated, bounded four-message Issue exchange. A client sends a normal Issue RST with `Context` (at most 128 characters), `AppliesTo` (at most 2048 characters), and a nonempty `BinaryExchange` (at most 4096 bytes). Optional token/key type identifiers are limited to 512 characters each; other request parameters are not supported in this profile. The STS replies with an intermediate RSTR at the `RSTR/Issue` action carrying a random 32-byte challenge and the same Context. The client echoes those bytes in an RSTR at the `RSTR/Issue` action; the STS then issues a final RSTRC at `RSTRC/IssueFinal`. The response must contain exactly one RSTR with only Context and BinaryExchange, and the same authenticated credential identity must complete the exchange. Pending challenges expire after two minutes, are single-use, and are capped at 1024 per server process. Other ValueTypes and out-of-order responses fault. The exchange does not establish proof of possession of a secret; configure message/transport protection appropriate to the endpoint.

The single-RSTR intermediate response mode applies only to WS-Trust 1.3 challenge rounds. Ordinary February 2005 Issue responses retain their collection response shape.

`WsTrustBinaryExchangeProcessor` coordinates exchange validation and issuance. It selects an `IBinaryExchangeProcessor` by its case-sensitive `ValueType` and supports up to eight challenge rounds, with 4096-byte limits on decoded input, output, and processor state. Oversized BinaryExchange data is rejected during XML decoding for both WS-Trust versions and both supported encodings. Additional challenges use a single RSTR at `RSTR/Issue`; only completion uses the final RSTRC at `RSTRC/IssueFinal`. Implement `IBinaryExchangeProcessor` with `Begin` and `Continue` methods that return `BinaryExchangeStep.Challenge(data, state)` or `BinaryExchangeStep.Completed()`; use `builder.AddBinaryExchangeProcessor<YourProcessor>()` (or the factory overload) inside `AddWsTrust`. The echo processor is registered by default. Duplicate ValueTypes are rejected when the orchestrator is constructed. Processor state must be bounded and serializable for a shared store. Completion uses the original authenticated requestor for issuance; the echo exchange does not itself authenticate a new identity.

`IWsTrustExchangeStore` now handles pending state only: atomic admission, lookup, transition, and single-use removal. For multi-node hosting, register a shared implementation with `builder.AddWsTrustExchangeStore<YourExchangeStore>()` (or its factory overload). The default `WsTrustExchangeStore` is in-memory and local to one server process. A shared implementation must enforce bounded capacity, expiry, and atomic transitions/removal across nodes. Mechanisms that depend on a live, node-local security context additionally require appropriate routing.

`AddWsTrust` registers `TimeProvider.System` for exchange expiry by default; register an application `TimeProvider` before calling it to supply a different clock.

This guide describes an integration pattern used by an ASP.NET Core WS-Trust application: a SOAP endpoint issues tokens for configured relying parties, validates incoming credentials and tokens, and enriches outgoing claims. The examples use illustrative names and assume certificates, partner identifiers, and credentials are supplied by the application's configuration or secret provider.

## Packages and service registration

The service uses `Solid.Identity.Protocols.WsTrust` for WS-Trust and WS-Security, with SOAP hosting configured by the WS-Trust builder. An application can configure options and register the async WS-Trust 1.3 contract:

```csharp
services.AddRouting();

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

The incoming issuer and the requested `AppliesTo` value select the identity provider and relying party. Configure trusted signing keys for token validation and a signing key for issued tokens; do not put private keys or passwords in source code. Applications that construct providers and parties dynamically can register `AddIdentityProviderStore<T>()` and `AddRelyingPartyStore<T>()` to resolve them through custom stores. See [stores and claims](ws-trust/stores.md) for implementations. The sample validator above is an application-defined type; see [credential validators](ws-trust/validators.md).

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

See [stores and claims](ws-trust/stores.md) for identity-provider, relying-party, and claim-store examples, and [credential validators](ws-trust/validators.md) for username/password and X.509 validation.

## Integration testing

Integration tests can use `Solid.Testing.AspNetCore.Extensions.Https`, `Solid.Testing.AspNetCore.Extensions.XUnit.Soap`, and `Solid.Testing.Certificates` to host the service and generate test-only certificates. A `SoapTestingServerFixture<TStartup>` can configure the host and inject test providers and relying parties. Test clients can use `WsTrustChannelFactory` with `WsTrustUserNameBinding` or `WsTrustIssuedTokenBinding`, set `TrustVersion` to `WsTrustVersion.Trust13`, and send requests to the hosted endpoint. Supply credentials through test setup rather than embedding real account data.

See the [SOAP services](soap.md) guide for the underlying hosting primitives.
