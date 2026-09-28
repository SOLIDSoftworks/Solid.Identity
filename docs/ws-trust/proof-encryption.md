# ProofEncryption in WS-Trust requests

`wst:ProofEncryption` identifies the key to use when encrypting proof material returned by a security token service. `Solid.IdentityModel.Protocols.WsTrust.WsTrustSerializer` reads and writes this element on `WsTrustRequest.ProofEncryption` for the WS-Trust February 2005 and 1.3 namespaces.

The property is a `SecurityTokenElement` containing **one** of:

- A `wsse:SecurityTokenReference` with a nonempty `wsse:KeyIdentifier` value. Reference serialization is delegated to the WS-Security serializer.
- An embedded security token supported by a handler in the serializer's `SecurityTokenHandlers` collection. The same handler must be available when reading the request back.

For example, a key-identifier reference can be supplied to the request serializer:

```csharp
using Solid.IdentityModel.Protocols.WsSecurity;
using Solid.IdentityModel.Protocols.WsTrust;

var serializer = new WsTrustSerializer();
var request = new WsTrustRequest(WsTrustConstants.Trust13.Actions.Issue)
{
    ProofEncryption = new SecurityTokenElement(new SecurityTokenReference
    {
        KeyIdentifier = new KeyIdentifier
        {
            Value = keyIdentifier, // Resolve from the requestor's key configuration.
            ValueType = keyIdentifierType
        }
    })
};

serializer.WriteRequest(writer, WsTrustConstants.Trust13, request);
```

Here `writer` is an `XmlDictionaryWriter`. The resulting request contains a `wst:ProofEncryption` wrapper around `wsse:SecurityTokenReference` and its `wsse:KeyIdentifier`. When reading XML with `serializer.ReadRequest(reader)`, the payload is restored to `request.ProofEncryption.SecurityTokenReference` (or `.SecurityToken` for an embedded token). For embedded tokens, register a compatible `SecurityTokenHandler` with `serializer.SecurityTokenHandlers.Add(handler)` before reading or writing. The static `WsTrustSerializer.WriteProofEncryption` method writes key-identifier references; use the instance `WriteRequest` method for embedded tokens so it can access the registered handlers.

An empty wrapper, missing/empty key identifier, unrecognized reference form, unsupported token, or more than one child is rejected rather than silently dropping the key. Writing an embedded token without a matching writable handler also fails. This is XML serialization support only: supplying `ProofEncryption` does not by itself make the service encrypt a returned proof key. See the separate [proof-key negotiation issue](https://github.com/SOLIDSoftworks/Solid.Identity/issues/5) for issuance and encrypted-proof behavior.
