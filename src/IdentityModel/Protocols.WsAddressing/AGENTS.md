# WS-Addressing protocol model and serialization

All features added to this project must comply with the applicable WS-Addressing specification for each version they affect:

- [WS-Addressing August 2004](https://www.w3.org/Submission/2004/SUBM-ws-addressing-20040810/) (`WsAddressingConstants.Addressing200408`)
- [WS-Addressing 1.0 — Core](https://www.w3.org/TR/ws-addr-core/) (`WsAddressingConstants.Addressing10`)

Before implementing or reviewing a feature, identify the affected version(s) and check the relevant XML namespaces, endpoint-reference structure, addressing rules, and version-specific differences. Verify serialization and parsing against the applicable specification and test affected versions.
