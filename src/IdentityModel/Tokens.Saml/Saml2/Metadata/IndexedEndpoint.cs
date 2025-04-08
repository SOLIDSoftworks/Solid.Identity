namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class IndexedEndpoint : Endpoint
    {
        public ushort Index { get; set; }
        public bool? IsDefault { get; set; }
    }
}
