using Solid.IdentityModel.Protocols.TestUtilities;

namespace Solid.IdentityModel.Protocols.WsSecurity.Tests;

public class WsSecuritySerializerTestFixture : ProtocolTestFixture
{
    public WsSecuritySerializerTestFixture()
    {
        var securitySerializer = new WsSecuritySerializer();
        SecurityTokenReferenceSerializer = securitySerializer;
        SecurityHeaderSerializer = securitySerializer;
        
        var securityUtilitySerializer = new WsSecurityUtilitySerializer();
        
        Serializer = new WsSerializer([
            securitySerializer,
            securityUtilitySerializer
        ]);
    }

    public WsSerializer Serializer { get; }

    public IProtocolSerializer<SecurityTokenReference> SecurityTokenReferenceSerializer { get; }
    public IProtocolSerializer<SecurityHeader> SecurityHeaderSerializer { get; }
}