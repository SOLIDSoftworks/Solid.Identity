using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "WS-Policy defines a URI-valued attribute", Scope = "member", Target = "~P:Solid.IdentityModel.Protocols.WsPolicy.PolicyReference.Uri")]
[assembly: SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "WS-Policy defines a URI-valued attribute", Scope = "member", Target = "~M:Solid.IdentityModel.Protocols.WsPolicy.PolicyReference.#ctor(System.String,System.String,System.String)")]
