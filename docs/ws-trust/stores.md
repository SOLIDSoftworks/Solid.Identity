# Stores and claims

WS-Trust resolves incoming issuers through `IIdentityProviderStore` and token audiences through `IRelyingPartyStore`. The application that informed these examples uses factories to construct providers and parties on demand. The samples below use generic, application-supplied identifiers and keys.

## Identity-provider store

Implement all three lookup methods: by issuer ID, by signing key, and enumeration. The key lookup is useful when a token is identified by its signing key rather than an explicit issuer. This small example uses preconfigured providers with X.509 signing keys; for a dynamic store, replace the in-memory collection with your own source of trusted partners.

```csharp
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public sealed class ExampleIdentityProviderStore : IIdentityProviderStore
{
    private readonly IReadOnlyList<IIdentityProvider> _providers;

    public ExampleIdentityProviderStore(IEnumerable<IIdentityProvider> providers)
        => _providers = providers.ToList();

    public ValueTask<IEnumerable<IIdentityProvider>> GetIdentityProvidersAsync()
        => new ValueTask<IEnumerable<IIdentityProvider>>(_providers);

    public ValueTask<IIdentityProvider> GetIdentityProviderAsync(string id)
        => new ValueTask<IIdentityProvider>(
            _providers.FirstOrDefault(provider => provider.Enabled && provider.Id == id));

    public ValueTask<IIdentityProvider> GetIdentityProviderAsync(SecurityKey key)
        => new ValueTask<IIdentityProvider>(
            key is X509SecurityKey presentedKey
                ? _providers.FirstOrDefault(provider => provider.Enabled &&
                    provider.SecurityKeys.OfType<X509SecurityKey>().Any(trustedKey =>
                        trustedKey.Certificate.RawData.SequenceEqual(
                            presentedKey.Certificate.RawData)))
                : null);
}
```

The key lookup matches a presented X.509 certificate to a configured trusted certificate; for other key types, implement a suitable comparison of trusted key material rather than trusting an unverified key ID. `IdentityProvider` can be created with `new IdentityProvider(issuerId)` and populated with trusted public `SecurityKeys` obtained from a certificate provider. Set `AllowedRelyingParties` when `RestrictRelyingParties` is enabled.

## Relying-party store

The `AppliesTo` field of a request identifies the relying party. A store may construct parties at lookup time, as in the consumer application, or retrieve them from a repository:

```csharp
using Solid.Identity.Protocols.WsTrust.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class ExampleRelyingPartyStore : IRelyingPartyStore
{
    private readonly IReadOnlyDictionary<string, IRelyingParty> _parties;

    public ExampleRelyingPartyStore(IReadOnlyDictionary<string, IRelyingParty> parties)
        => _parties = parties;

    public ValueTask<IEnumerable<IRelyingParty>> GetRelyingPartiesAsync()
        => new ValueTask<IEnumerable<IRelyingParty>>(_parties.Values);

    public ValueTask<IRelyingParty> GetRelyingPartyAsync(string appliesTo)
        => new ValueTask<IRelyingParty>(
            appliesTo != null && _parties.TryGetValue(appliesTo, out var party) && party.Enabled
                ? party : null);
}
```

Build an `IRelyingParty` with `new RelyingParty(appliesTo)`; configure `RequiredClaims`, `SupportedTokenTypes`, `TokenLifetime`, and signing or encryption keys as needed. Return `null` for unknown audiences rather than creating a permissive default party.

Register the stores through the WS-Trust builder:

```csharp
services.AddWsTrust(builder => builder
    .AddIdentityProviderStore<ExampleIdentityProviderStore>()
    .AddRelyingPartyStore<ExampleRelyingPartyStore>()
    .AddWsTrust13AsyncContract());
```

The built-in options collections still work alongside custom stores: lookups check the store first, then the configured options. Supply the example stores' dependencies through dependency injection.

## Claims for a relying party

The application also uses relying-party claim stores to add audience-specific claims. Derive from `ClaimStore` and advertise the offered claim types:

```csharp
using Solid.Identity.Protocols.WsTrust;
using Solid.Identity.Protocols.WsTrust.Abstractions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

public sealed class ExamplePartyClaimStore : ClaimStore
{
    private const string Audience = "urn:example:service";

    public override bool CanGenerateClaims(string appliesTo) => appliesTo == Audience;

    public override IEnumerable<ClaimDescriptor> ClaimTypesOffered =>
        new[] { new ClaimDescriptor("urn:example:claim:category") };

    public override ValueTask<IEnumerable<Claim>> GetClaimsAsync(
        ClaimsIdentity identity, IRelyingParty party)
    {
        var category = identity.FindFirst("urn:example:incoming:category")?.Value;
        IEnumerable<Claim> claims = category == null
            ? new Claim[0]
            : new[] { new Claim("urn:example:claim:category", category) };
        return new ValueTask<IEnumerable<Claim>>(claims);
    }
}
```

Register it with `builder.AddRelyingPartyClaimStore<ExamplePartyClaimStore>()`. For claims that depend on the requested token format instead of the audience, implement `ITokenTypeClaimStore` with `CanGenerateClaims(string tokenType)` and `GetClaimsAsync(ClaimsIdentity, IRelyingParty, IEnumerable<Claim>)`, then register it with `AddTokenTypeClaimStore<T>()`.
