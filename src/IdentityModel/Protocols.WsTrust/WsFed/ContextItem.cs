#pragma warning disable 1591

using System;
using Microsoft.IdentityModel.Logging;

namespace Solid.IdentityModel.Protocols.WsFed
{
    /// <summary>
    /// This class is used to represent a ContextItem found in the WsFed specification: http://docs.oasis-open.org/wsfed/federation/v1.2/os/ws-federation-1.2-spec-os.html .
    /// </summary>
    public class ContextItem
    {
        private string _name;
        private string _scope;
        private string _value;

        internal ContextItem() { }

        public ContextItem(string name)
        {
            Name = name;
        }

        public ContextItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public ContextItem(string name, string value, string scope)
        {
            Name = name;
            Value = value;
            Scope = scope;
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Name 
        {
            get => _name;
            set => _name = (string.IsNullOrEmpty(value)) ? throw LogHelper.LogArgumentNullException(nameof(Name)) : value; 
        }

        /// <summary>
        /// Gets the Scope of the scope.
        /// </summary>
        public string Scope
        {
            get => _scope;
            set => _scope = (string.IsNullOrEmpty(value)) ? throw LogHelper.LogArgumentNullException(nameof(Scope)) : value;
        }

        /// <summary>
        /// Gets the value of the value.
        /// </summary>
        public string Value
        {
            get => _value;
            set => _value = (string.IsNullOrEmpty(value)) ? throw LogHelper.LogArgumentNullException(nameof(Value)) : value;
        }
    }
}
