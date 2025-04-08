using Microsoft.IdentityModel.Tokens;

namespace Solid.IdentityModel.Tokens.Tests
{
    public class SignatureProviderTestData
    {
        public SecurityKey SigningKey { get; set; }
        public string Algorithm { get; set; }
        public string Data { get; set; }
        public string SignedData { get; set; }
        public bool Verify { get; set; }
        public bool Sign { get; internal set; }
    }
}