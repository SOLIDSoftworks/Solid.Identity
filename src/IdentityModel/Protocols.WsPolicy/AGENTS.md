# WS-Policy model and serialization

All features added to this project must comply with the applicable WS-Policy specification for each version they affect. The policy-version constants in `Protocols.WsStar/WsSecurity/WsSecurityPolicyConstants.cs` use WS-Policy namespaces:

- [WS-Policy 1.2 — Framework](https://www.w3.org/Submission/2006/SUBM-WS-Policy-20060425/) (`WsSecurityPolicyConstants.SecurityPolicy12`)
- [WS-Policy 1.5 — Framework](https://www.w3.org/TR/ws-policy/) (`WsSecurityPolicyConstants.SecurityPolicy15`)

Before implementing or reviewing a feature, identify the affected version(s) and check the relevant policy expression syntax, namespace, references, and processing rules. Verify serialization and parsing against the applicable specification and test affected versions.
