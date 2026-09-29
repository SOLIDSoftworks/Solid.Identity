# XML Encryption model and serialization

All features added to this project must comply with [XML Encryption Syntax and Processing 1.1](https://www.w3.org/TR/xmlenc-core1/) (`XmlEncryptionConstants.XmlEnc11` in `Protocols.WsStar`). This specification retains the `http://www.w3.org/2001/04/xmlenc#` namespace used by the constant and also defines `http://www.w3.org/2009/xmlenc11#` for 1.1 additions.

Before implementing or reviewing a feature, check the applicable syntax, namespaces, algorithm identifiers, and processing rules. Verify serialization and parsing against the specification and test affected behavior.
