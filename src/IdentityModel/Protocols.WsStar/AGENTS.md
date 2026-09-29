# Shared WS-* protocol constants and XML helpers

All features and version constants added to this project must comply with the specification for the affected protocol and version. Consult the corresponding project-level `AGENTS.md` in `Protocols.WsAddressing`, `Protocols.WsFederation`, `Protocols.WsPolicy`, `Protocols.WsSecurity`, `Protocols.WsSecureConversation`, `Protocols.WsTrust`, or `Protocols.XmlEnc` before changing shared protocol constants or XML handling.

Match namespace URIs and version-specific values to the cited specifications. When adding a version, update the owning project's `AGENTS.md` with its specification and verify affected versions in tests. The current `WsSecurityPolicyConstants.SecurityPolicy12` and `SecurityPolicy15` carry WS-Policy 1.2 and 1.5 namespaces; see `Protocols.WsPolicy/AGENTS.md` for their specifications.
