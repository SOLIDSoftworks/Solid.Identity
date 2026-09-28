# Credential validators

WS-Security can authenticate a username/password token using `IPasswordValidator` or an X.509 token using `IX509Validator`. The consumer application registers a custom password validator and leaves X.509 validation unregistered; the X.509 example below shows how to enable it when needed.

## Username and password

Derive from `PasswordValidator` to reuse its principal and claim creation, and supply credential checking through an application-defined service. The validator returns no principal for invalid credentials:

```csharp
using Solid.Identity.Protocols.WsSecurity.Abstractions;
using System.Threading.Tasks;

public interface ICredentialVerifier
{
    ValueTask<bool> VerifyAsync(string userName, string password);
}

public sealed class ExamplePasswordValidator : PasswordValidator
{
    private readonly ICredentialVerifier _verifier;

    public ExamplePasswordValidator(ICredentialVerifier verifier) => _verifier = verifier;

    protected override ValueTask<bool> IsValidAsync(string userName, string password)
        => _verifier.VerifyAsync(userName, password);
}
```

The base class creates a `ClaimsPrincipal` containing a subject identifier, username, authentication method, and authentication instant. If an application needs to validate an external token and map its claims itself, it can instead implement `IPasswordValidator.ValidatePasswordAsync` directly and return `null` for rejected credentials. Resolve verifier credentials and configuration at runtime, and do not log incoming passwords.

Register the validator with `builder.AddPasswordValidator<ExamplePasswordValidator>()` and register `ICredentialVerifier` with dependency injection. The WS-Trust builder uses a singleton validator, so its dependencies must be compatible with that lifetime.

## X.509 certificates

`X509Validator` creates a principal from a certificate after `IsValidAsync` succeeds. This example delegates trust checks to a separately configured certificate policy:

```csharp
using Solid.Identity.Protocols.WsSecurity.Abstractions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public interface ICertificateTrustPolicy
{
    ValueTask<bool> IsTrustedAsync(X509Certificate2 certificate);
}

public sealed class ExampleX509Validator : X509Validator
{
    private readonly ICertificateTrustPolicy _policy;

    public ExampleX509Validator(ICertificateTrustPolicy policy) => _policy = policy;

    protected override ValueTask<bool> IsValidAsync(X509Certificate2 certificate)
        => _policy.IsTrustedAsync(certificate);
}
```

Implement `ICertificateTrustPolicy` to verify the certificate chain, validity period, and required partner identity according to your trust policy. The base validator creates claims for the certificate subject, thumbprint, and authentication method. Register a policy implementation with dependency injection and opt in to certificate validation using `builder.AddX509Validator<ExampleX509Validator>()`. This validator is also registered as a singleton, so its policy dependency must have a compatible lifetime.
