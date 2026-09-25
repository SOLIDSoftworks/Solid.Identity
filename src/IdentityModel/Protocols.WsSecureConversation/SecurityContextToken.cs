namespace Solid.IdentityModel.Protocols.WsSecureConversation;

public class SecurityContextToken : XmlOpenItem
{
    public string Id { get; set; }
    public Identifier Identifier { get; set; }
    public string Instance { get; set; }
}
