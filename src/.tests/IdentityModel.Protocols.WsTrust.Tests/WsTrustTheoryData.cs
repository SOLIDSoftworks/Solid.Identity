using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Protocols.WsSecurity;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Solid.IdentityModel.Protocols.WsTrust.Tests
{
    public class WsTrustTheoryData
    {
        public WsTrustTheoryData() { }

        public WsTrustTheoryData(WsTrustVersion trustVersion)
        {
            WsSerializationContext = new WsSerializationContext(trustVersion);
            WsTrustVersion = trustVersion;
        }

        public WsTrustTheoryData(XmlDictionaryReader reader)
        {
            Reader = reader;
        }

        public WsTrustTheoryData(MemoryStream memoryStream)
        {
            MemoryStream = memoryStream;
            Writer = XmlDictionaryWriter.CreateTextWriter(memoryStream, Encoding.UTF8);
        }

        public WsTrustTheoryData(MemoryStream memoryStream, WsTrustVersion trustVersion)
        {
            MemoryStream = memoryStream;
            Writer = XmlDictionaryWriter.CreateTextWriter(memoryStream, Encoding.UTF8);
            WsSerializationContext = new WsSerializationContext(trustVersion);
            WsTrustVersion = trustVersion;
        }

        public BinarySecret BinarySecret { get; set; }

        public Claims Claims { get; set; }

        public Entropy Entropy { get; set; }

        public Lifetime Lifetime { get; set; }

        public MemoryStream MemoryStream { get; set; }

        public SecurityTokenElement OnBehalfOf { get; set; }

        public SecurityTokenElement ProofEncryption { get; set; }

        public XmlDictionaryReader Reader { get; set; }

        public SecurityTokenReference Reference { get; set; }

        public SecurityTokenReference RequestedAttachedReference { get; set; }

        public RequestedProofToken RequestedProofToken { get; set; }

        public RequestedSecurityToken RequestedSecurityToken { get; set; }

        public SecurityTokenReference RequestedUnattachedReference { get; set; }

        public RequestSecurityTokenResponse RequestSecurityTokenResponse { get; set; }

        public SecurityTokenHandler SecurityTokenHandler { get; set; }

        public TokenValidationParameters TokenValidationParameters { get; set; }

        public UseKey UseKey { get; set; }

        public XmlDictionaryWriter Writer { get; set; }

        public WsSerializationContext WsSerializationContext { get; set; }

        public WsTrustRequest WsTrustRequest { get; set; }

        public WsTrustResponse WsTrustResponse { get; set; }

        public WsTrustSerializer WsTrustSerializer { get; set; } = new WsTrustSerializer();

        public WsTrustVersion WsTrustVersion { get; set; }
    }
}
