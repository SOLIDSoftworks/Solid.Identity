using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsSecureConversation;

public class WsSecureConversationConstants : WsProtocolConstants
{
    public static WsSecureConversationConstants SecureConversationFeb2005 { get; } = new WsSecureConversationFeb2005();
    public static WsSecureConversationConstants SecureConversation13 { get; } = new WsSecureConversation13();
    
    public static IDictionary<string, WsSecureConversationConstants> KnownNamespaces { get; } = new Dictionary<string, WsSecureConversationConstants>
    {
        { SecureConversationFeb2005.Namespace,SecureConversationFeb2005 },
        { SecureConversation13.Namespace, SecureConversation13 }
    };
}

internal class WsSecureConversationFeb2005 : WsSecureConversationConstants
{
    public WsSecureConversationFeb2005()
    {
        DefaultPrefix = "wsc";
        Namespace = "http://schemas.xmlsoap.org/ws/2005/02/sc";
    }
}

internal class WsSecureConversation13 : WsSecureConversationConstants
{
    public WsSecureConversation13()
    {
        DefaultPrefix = "wsc";
        Namespace = "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512";
    }
}