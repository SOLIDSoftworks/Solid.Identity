# SAML 2.0

`Solid.Identity.Protocols.Saml2p` integrates SAML 2.0 single sign-on with ASP.NET Core. It supports service-provider (SP) and identity-provider (IdP) scenarios.

```console
dotnet add package Solid.Identity.Protocols.Saml2p
```

For an SP, register your partner IdP and add the SAML authentication handler:

```csharp
services.AddSaml2p(options =>
{
    options.DefaultIssuer = "https://myhost/saml";
    options.AddIdentityProvider("https://identityproviderhost/saml", idp =>
    {
        idp.BaseUrl = new Uri("https://identityproviderhost");
        idp.AcceptSsoEndpoint = "/saml/sso";
        idp.CanInitiateSso = true;
        // Configure the partner's assertion signing keys here.
    });
});

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Saml2pAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddSaml2p("https://identityproviderhost/saml");
```

The [SAML 2.0 package guide](https://github.com/SOLIDSoftworks/Solid.Identity/blob/main/src/Protocols/Saml2p/README.md) also covers IdP setup, partner configuration, and authentication-context handling.
