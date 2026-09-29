# WS-Trust protocol model and serialization

Before making any change in this project, consult the relevant requirements in both WS-Trust specifications:

- [WS-Trust February 2005](https://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf)
- [WS-Trust 1.3 (December 2005)](https://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html)

Identify which version(s) and message elements the change affects. Check the applicable XML structure, namespaces, required and optional fields, processing rules, and version-specific differences before implementing or reviewing it. Ensure serialization and parsing follow the specifications, and cover affected versions in tests. If a requirement is intentionally unsupported, document the limitation rather than silently treating it as supported.
