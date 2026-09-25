using Xunit;

namespace Solid.IdentityModel.Protocols.XmlEnc.Tests;

public class XmlEncryptionTests
{
    [Fact]
    public void EncryptionTypesUseXmlEncryptionNamespace()
    {
        var constants = XmlEncryptionConstants.XmlEnc11;
        Assert.Equal("http://www.w3.org/2001/04/xmlenc#", constants.Namespace);
        Assert.Equal(constants.Namespace + "Element", XmlEncryptionDataTypes.XmlEnc11.Element);
        Assert.Equal(constants.Namespace + "Content", XmlEncryptionDataTypes.XmlEnc11.Content);
    }

    [Fact]
    public void EncryptionAssemblyDoesNotDependOnTrust()
    {
        Assert.DoesNotContain(typeof(EncryptedKey).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name == "Solid.IdentityModel.Protocols.WsTrust");
    }
}
