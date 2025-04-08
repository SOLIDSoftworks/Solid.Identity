#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    public class SecurityTokenReference
    {
        public SecurityTokenReference()
        {
            
        }
        
        public SecurityTokenReference(KeyIdentifier keyIdentifier)
            : this()
        {
            KeyIdentifier = keyIdentifier;
        }

        public string Id { get; set; }

        public KeyIdentifier KeyIdentifier { get; set; }

        public string TokenType { get; set; }

        public string Usage { get; set; }
    }
}
