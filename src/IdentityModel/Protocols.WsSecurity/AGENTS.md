# WS-Security model and serialization

All features added to this project must comply with the applicable WS-Security specification for each version they affect:

- [WS-Security 1.0 — SOAP Message Security](https://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0.pdf) (`WsSecurityConstants.WsSecurity10`)
- [WS-Security 1.1 — SOAP Message Security](https://docs.oasis-open.org/wss/v1.1/wss-v1.1-spec-errata-os-SOAPMessageSecurity.htm) (`WsSecurityConstants.WsSecurity11`)

The `WsSecurityUtilityConstants.SecurityUtility10` namespace is defined by the WS-Security utility schema referenced in these specifications. Before implementing or reviewing a feature, check the applicable security token, reference, timestamp, XML namespace, and processing requirements. Verify serialization and parsing against the applicable specification and test affected versions.
