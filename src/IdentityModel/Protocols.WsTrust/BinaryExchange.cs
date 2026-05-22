using System;
using Microsoft.IdentityModel.Logging;
using Solid.IdentityModel.Protocols.WsSecurity;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// Represents the contents of the BinaryExchange element.
    /// see: http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html
    /// </summary>
    public class BinaryExchange
    {
        private byte[] _data;
        private string _encodingType;
        private string _valueType;

        internal BinaryExchange()
        {
        }
        
        /// <summary>
        /// Creates an instance of <see cref="BinaryExchange"/>
        /// </summary>
        /// <param name="data">Binary data exchanged.</param>
        /// <param name="valueType">Uri representing the value type of the binary data.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="data"/> or <paramref name="valueType"/>.</exception>
        /// <remarks>Default encoding type is: "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1/#Base64Binary".
        /// for possible values see: "http://docs.oasis-open.org/ws-sx/ws-trust/200512/ws-trust-1.3-os.html#wssecurity".</remarks>
        public BinaryExchange(byte[] data, string valueType)
            : this(data, valueType, WsSecurityEncodingTypes.WsSecurity11.Base64)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="BinaryExchange"/>
        /// </summary>
        /// <param name="data">Binary data exchanged.</param>
        /// <param name="valueType">Uri representing the value type of the binary data.</param>
        /// <param name="encodingType">Encoding type to be used for encoding the binary data </param>
        /// <exception cref="ArgumentNullException">if <paramref name="data"/>, <paramref name="valueType"/> or <paramref name="encodingType"/> is null.</exception>
        public BinaryExchange(byte[] data, string valueType, string encodingType)
        {
            Data = data ?? throw LogHelper.LogArgumentNullException(nameof(data));
            ValueType = valueType ?? throw LogHelper.LogArgumentNullException(nameof(valueType)); ;
            EncodingType = encodingType ?? throw LogHelper.LogArgumentNullException(nameof(encodingType)); ;
        }

        /// <summary>
        /// Gets the Binary Data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                byte[] copy = new byte[_data.Length];
                Array.Copy(_data, copy, _data.Length);
                return copy;
            }

            internal set => _data = value;
        }

        /// <summary>
        /// Gets the ValueType Uri.
        /// </summary>
        public string ValueType
        {
            get => _valueType;
            set => _valueType = string.IsNullOrEmpty(value) ? throw LogHelper.LogArgumentNullException(nameof(ValueType)) : value;
        }


        /// <summary>
        /// Gets or sets the encoding type.
        /// </summary>
        public string EncodingType
        {
            get => _encodingType;
            set => _encodingType = string.IsNullOrEmpty(value) ? throw LogHelper.LogArgumentNullException(nameof(EncodingType)) : value;
        }
    }
}
