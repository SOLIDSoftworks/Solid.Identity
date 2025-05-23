﻿#pragma warning disable 1591

namespace Solid.IdentityModel.Protocols.WsSecurity
{
    public class KeyIdentifier
    {
        public KeyIdentifier()
        {
        }

        public KeyIdentifier(string value)
            : this()
        {
            Value = value;
        }

        public string EncodingType { get; set; }

        public string Id { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }
    }
}
