using System;
using System.Linq;
using Solid.IdentityModel.Protocols.WsSecurity;
using Xunit;

namespace Solid.IdentityModel.Protocols.TestUtilities;

public class ProtocolTestFixture
{
    public void Compare(SecurityHeader? expected, SecurityHeader? value)
    {
        CompareNullability(expected, value);
        Compare(expected.Timestamp, value.Timestamp);
        CompareAdditionalNodes(expected, value);
    }
    
    public void Compare(Timestamp? expected, Timestamp? value)
    {
        CompareNullability(expected, value);
        
        Assert.Equal(expected!.Id, value.Id);
        Assert.Equal(expected.Created, value.Created);
        Assert.Equal(expected.Expires, value.Expires);
    }
    
    
    
    public void Compare(SecurityTokenReference? expected, SecurityTokenReference? value)
    {
        CompareNullability(expected, value);
        Assert.Equal(expected!.Id, value.Id);
        Assert.Equal(expected.TokenType, value.TokenType);
        Assert.Equal(expected.Usage, value.Usage);
        if(expected.KeyIdentifier != null)
            Compare(expected.KeyIdentifier, value.KeyIdentifier);
    }
    
    public void Compare(KeyIdentifier? expected, KeyIdentifier? value)
    {
        CompareNullability(expected, value);
        Assert.Equal(expected.Id, value.Id);
        Assert.Equal(expected.ValueType, value.ValueType);
        Assert.Equal(expected.Value, value.Value);
    }

    public void Compare(ProtocolTestData data, Exception exception)
    {
        if(data.ExpectedExceptionType == null)
            Assert.Null(exception);
        Assert.Equal(data.ExpectedExceptionType, exception.GetType());
        if (data.ExpectedError != null)
            Assert.StartsWith(data.ExpectedError, exception.Message);
    }
    
    private void CompareNullability(object? expected, object? value)
    {
        Assert.NotNull(expected);
        Assert.NotNull(value);
    }

    private void CompareAdditionalNodes(IXmlOpenItem expected, IXmlOpenItem value)
    {
        Assert.Equal(expected.AdditionalXmlElements.Count, value.AdditionalXmlElements.Count);
        Assert.Equal(expected.AdditionalXmlAttributes.Count, value.AdditionalXmlAttributes.Count);

        foreach(var element in value.AdditionalXmlElements)
            element.Normalize();
        
        foreach (var element in expected.AdditionalXmlElements)
        {
            element.Normalize();
            var match = value.AdditionalXmlElements.FirstOrDefault(x => x.OuterXml == element.OuterXml);
            Assert.NotNull(match);
        }

        foreach (var element in expected.AdditionalXmlAttributes)
        {
            Assert.Contains(value
                .AdditionalXmlAttributes, a => a.LocalName == element.LocalName && a.NamespaceUri == element.NamespaceUri && a.Value == element.Value);
        }
    }
}