# WS-SecureConversation protocol model and serialization

All features added to this project must comply with the applicable WS-SecureConversation specification for each version they affect:

- [WS-SecureConversation February 2005](https://specs.xmlsoap.org/ws/2005/02/sc/WS-SecureConversation.pdf) (`WsSecureConversationConstants.SecureConversationFeb2005`)
- [WS-SecureConversation 1.3](https://docs.oasis-open.org/ws-sx/ws-secureconversation/200512/ws-secureconversation-1.3-os.html) (`WsSecureConversationConstants.SecureConversation13`)

Before implementing or reviewing a feature, identify the affected version(s) and check the relevant security context, token, derived-key, XML namespace, and processing requirements. Verify serialization and parsing against the applicable specification and test affected versions.
