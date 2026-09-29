# WS-Federation protocol model and serialization

All features added to this project must comply with [WS-Federation 1.2](https://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html), including its federation, authorization, and privacy provisions (`WsFederationConstants.Federation12`, `WsFederationAuthorizationConstants.FederationAuthorization12`, and `WsFederationPrivacyConstants.FederationPrivacy12`).

Before implementing or reviewing a feature, check the applicable message semantics, XML namespaces, required and optional elements, and processing rules. Verify serialization and parsing against the specification and test affected features.
