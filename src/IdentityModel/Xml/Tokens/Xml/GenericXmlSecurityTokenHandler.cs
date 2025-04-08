using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Solid.IdentityModel.Tokens.Xml
{
    /// <summary>
    /// An implementation of <see cref="SecurityTokenHandler"/> that can read and write generic xml security tokens.
    /// </summary>
    public class GenericXmlSecurityTokenHandler : SecurityTokenHandler
    {
        private readonly Encoding _encoding = new UTF8Encoding(false);

        /// <summary>
        /// The type of <see cref="SecurityToken"/> that this handler can read and write.
        /// </summary>
        public override Type TokenType => typeof(GenericXmlSecurityToken);

        /// <summary>
        /// Gets a value indicating whether this handler supports validation of tokens handled by this instance.
        /// </summary>
        /// <value>false</value>
        public override bool CanValidateToken => false;

        /// <summary>
        /// Gets a value indicating whether the class provides serialization functionality to serialize token handled by this instance.
        /// </summary>
        /// <value>true</value>
        public override bool CanWriteToken => true;

        /// <summary>
        /// Gets a value indicating whether this handler can read a token from the provided <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> instance.</param>
        /// <returns>true is start element</returns>
        public override bool CanReadToken(XmlReader reader) => reader.IsStartElement();

        /// <summary>
        /// Reads an xml element from the provided <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> instance.</param>
        /// <returns>A <see cref="GenericXmlSecurityToken"/> instance.</returns>
        public override SecurityToken ReadToken(XmlReader reader)
        {
            if (!reader.IsStartElement())
                reader.MoveToContent();
            using var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false, Encoding = _encoding, OmitXmlDeclaration = true }))
                writer.WriteNode(reader, true);
            stream.Position = 0;
            var document = new XmlDocument();
            document.Load(stream);
            return new GenericXmlSecurityToken(document.DocumentElement, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(1));
        }

        /// <summary>
        /// Reads an xml element from the provided <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> instance.</param>
        /// <param name="_">Not used.</param>
        /// <returns>A <see cref="GenericXmlSecurityToken"/> instance.</returns>
        public override SecurityToken ReadToken(XmlReader reader, TokenValidationParameters _)
            => ReadToken(reader);

        /// <summary>
        /// Writes an xml element from the provided <paramref name="token"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">An <see cref="XmlWriter"/> instance.</param>
        /// <param name="token">A <see cref="GenericXmlSecurityToken"/> instance.</param>
        public override void WriteToken(XmlWriter writer, SecurityToken token)
        {
            if (!(token is GenericXmlSecurityToken generic))
                throw new ArgumentException("Can only write GenericXmlSecurityTokens.", nameof(token));

            writer.WriteRaw(generic.Element.OuterXml);
        }

        /// <summary>
        /// Writes an xml element from the provided <paramref name="token"/> as a <see cref="string"/>.
        /// </summary>
        /// <param name="token">A <see cref="GenericXmlSecurityToken"/> instance.</param>
        /// <returns>The <see cref="string"/> representation of the <see cref="GenericXmlSecurityToken"/>.</returns>
        public override string WriteToken(SecurityToken token)
        {
            using var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false, OmitXmlDeclaration = true, Encoding = _encoding }))
                WriteToken(writer, token);

            return _encoding.GetString(stream.ToArray());
        }
    }
}
