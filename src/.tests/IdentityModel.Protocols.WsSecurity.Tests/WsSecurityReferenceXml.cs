using System;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Logging;
using Solid.IdentityModel.Protocols.TestUtilities;

namespace Solid.IdentityModel.Protocols.WsSecurity.Tests;

public class WsSecurityReferenceXml
{
    public static XmlDictionaryReader GetSecurityHeaderReader(WsSecurityConstants securityConstants, WsSecurityUtilityConstants securityUtilityConstants, SecurityHeader header)
        => XmlUtilities.CreateDictionaryReader(CreateSecurityHeaderXml(securityConstants, securityUtilityConstants, header));
    
    public static string CreateSecurityHeaderXml(WsSecurityConstants securityConstants, WsSecurityUtilityConstants securityUtilityConstants, SecurityHeader header)
    {
        var builder = new StringBuilder();
        builder.Append($"<{securityConstants.DefaultPrefix}:Security");

        var xmlns = header
            .AdditionalXmlAttributes
            .Select(a => $@" xmlns:{a.Prefix}=""{a.NamespaceUri}""")
            .Concat([
                $@" xmlns:{securityConstants.DefaultPrefix}=""{securityConstants.Namespace}""",
                $@" xmlns:{securityUtilityConstants.DefaultPrefix}=""{securityUtilityConstants.Namespace}"""
            ])
            .Distinct();

        foreach (var ns in xmlns)
            builder.Append(ns);
        
        builder.AppendLine(">");
        
        builder.AppendLine(CreateTimestampXml(securityUtilityConstants, header.Timestamp));
        foreach (var element in header.AdditionalXmlElements)
            builder.AppendLine(element.OuterXml);

        builder.AppendLine($"</{securityConstants.DefaultPrefix}:Security>");
        
        return builder.ToString();
    }

    public static string CreateTimestampXml(WsSecurityUtilityConstants securityUtilityConstants, Timestamp timestamp, bool addXmlns = true)
    {
        var builder = new StringBuilder();
        builder.Append($"<{securityUtilityConstants.DefaultPrefix}:Timestamp");
        if (addXmlns)
            builder.Append($@" xmlns:{securityUtilityConstants.DefaultPrefix}=""{securityUtilityConstants.Namespace}""");
        builder.AppendLine(">");

        builder.AppendLine(
            $"<{securityUtilityConstants.DefaultPrefix}:Created>{XmlConvert.ToString(timestamp.Created, XmlDateTimeSerializationMode.Utc)}</{securityUtilityConstants.DefaultPrefix}:Created>");
        builder.AppendLine(
            $"<{securityUtilityConstants.DefaultPrefix}:Expires>{XmlConvert.ToString(timestamp.Expires, XmlDateTimeSerializationMode.Utc)}</{securityUtilityConstants.DefaultPrefix}:Expires>");
        
        builder.AppendLine($"</{securityUtilityConstants.DefaultPrefix}:Timestamp>");
        
        return builder.ToString();
    }


    public static XmlDictionaryReader GetSecurityTokenReferenceReader(WsSecurityConstants securityConstants, WsSecurityUtilityConstants securityUtilityConstants, SecurityTokenReference reference)
        => XmlUtilities.CreateDictionaryReader(CreateSecurityTokenReferenceXml(securityConstants, securityUtilityConstants, reference));

    public static string CreateSecurityTokenReferenceXml(WsSecurityConstants securityConstants, WsSecurityUtilityConstants securityUtilityConstants, SecurityTokenReference reference)
    {
        var builder = new StringBuilder();
        builder.Append($"<{securityConstants.DefaultPrefix}:SecurityTokenReference");
        if (reference.Id != null)
            builder.Append($@" {securityUtilityConstants.DefaultPrefix}:Id=""{reference.Id}""");

        if (reference.TokenType != null)
            builder.Append($@" TokenType=""{reference.TokenType}""");
        if (reference.Usage != null)
            builder.Append($@" Usage=""{reference.Usage}""");
        
        builder.Append($@" xmlns:{securityConstants.DefaultPrefix}=""{securityConstants.Namespace}""");
        builder.Append($@" xmlns:{securityUtilityConstants.DefaultPrefix}=""{securityUtilityConstants.Namespace}""");
        builder.AppendLine(">");
        
        if(reference.KeyIdentifier != null)
            builder.AppendLine(CreateKeyIdentifierXml(securityConstants, securityUtilityConstants, reference.KeyIdentifier));

        builder.AppendLine($"</{securityConstants.DefaultPrefix}:SecurityTokenReference>");
        
        return builder.ToString();
    }
    
    public static XmlDictionaryReader GetKeyIdentifierReader(WsSecurityConstants securityConstants, WsSecurityUtilityConstants securityUtilityConstants, KeyIdentifier identifier)
        => XmlUtilities.CreateDictionaryReader(CreateKeyIdentifierXml(securityConstants, securityUtilityConstants, identifier));

    public static string CreateKeyIdentifierXml(WsSecurityConstants securityConstants, WsSecurityUtilityConstants securityUtilityConstants, KeyIdentifier identifier)
    {
        //  <wsse:KeyIdentifier wsu:Id="..."
        //                      ValueType="..."
        //                      EncodingType="...">
        //      ...
        //  </wsse:KeyIdentifier>
        
        var builder = new StringBuilder();
        builder.Append(string.Format(@"<{0}:KeyIdentifier {1}:Id=""{2}"" ValueType=""{5}"" xmlns:{0}=""{3}"" xmlns:{1}=""{4}"">", 
            securityConstants.DefaultPrefix,
            securityUtilityConstants.DefaultPrefix,
            identifier.Id,
            securityConstants.Namespace,
            securityUtilityConstants.Namespace,
            identifier.ValueType
        ));
        builder.Append(identifier.Value);
        builder.Append(string.Format("</{0}:KeyIdentifier>", securityConstants.DefaultPrefix));
        
        return builder.ToString();
    }
    
    public static XmlDictionaryReader RandomElementReader => XmlUtilities.CreateDictionaryReader(@"<z:SomeRandomElement xmlns:z=""http://some.random.namespace.xsd"" >SomeRandomValue</z:SomeRandomElement>");

    public static XmlElement RandomElement
    {
        get
        {
            var reader = RandomElementReader;
            var doc = new XmlDocument();
            doc.LoadXml(reader.ReadOuterXml());
            return doc.DocumentElement;
        }
    } 
    
}