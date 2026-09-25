using System;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace Solid.IdentityModel.Protocols.WsTrust.Tests;

public class BinarySecretWireTests
{
    [Theory]
    [InlineData("http://schemas.xmlsoap.org/ws/2005/02/trust")]
    [InlineData("http://docs.oasis-open.org/ws-sx/ws-trust/200512")]
    [InlineData("http://docs.oasis-open.org/ws-sx/ws-trust/200802")]
    public void UntypedSecretIsBase64Encoded(string ns)
    {
        var version = WsTrustConstants.KnownNamespaces[ns];
        var data = new byte[] { 0, 1, 2, 253, 254, 255 };
        using var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            WsTrustSerializer.WriteBinarySecret(writer, new WsSerializationContext(version), new BinarySecret(data));

        stream.Position = 0;
        using var reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max);
        reader.MoveToContent();
        Assert.Equal(ns, reader.NamespaceURI);
        Assert.Equal(Convert.ToBase64String(data), reader.ReadElementContentAsString());
    }
}
