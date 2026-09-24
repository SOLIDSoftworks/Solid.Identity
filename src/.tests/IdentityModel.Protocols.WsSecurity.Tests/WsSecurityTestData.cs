using Solid.IdentityModel.Protocols.TestUtilities;

namespace Solid.IdentityModel.Protocols.WsSecurity.Tests;

public class WsSecurityTestData : ProtocolTestData
{
    private SecurityTokenReference? _securityTokenReference;
    private SecurityHeader? _securityHeader;

    public WsSecurityTestData(WsSecurityConstants constants, WsSecurityUtilityConstants utilityConstants)
    {
        SerializationContext = new WsSerializationContext
        {
            Security = constants,
            SecurityUtility = utilityConstants
        };
    }
    
    public WsSerializationContext? SerializationContext { get; set; }

    public SecurityTokenReference? SecurityTokenReference
    {
        get => _securityTokenReference;
        set
        {
            if (value != null)
                Reader = WsSecurityReferenceXml.GetSecurityTokenReferenceReader(SerializationContext!.Security, SerializationContext!.SecurityUtility, value);
            else
                Reader = null;
            _securityTokenReference = value;
        }
    }

    public KeyIdentifier? KeyIdentifier { get; set; }

    public SecurityHeader? SecurityHeader
    {
        get => _securityHeader;
        set 
        {
            if (value != null)
                Reader = WsSecurityReferenceXml.GetSecurityHeaderReader(SerializationContext!.Security, SerializationContext!.SecurityUtility, value);
            else
                Reader = null;
            _securityHeader = value;
        }
    }
}