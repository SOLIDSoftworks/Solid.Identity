
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens
{
    public class RequestedSecurityTokenDescriptor : SecurityTokenDescriptor
    {
        private bool _useSingleEncryptingCredentials = true;
        private EncryptingCredentials _proofKeyEncryptingCredentials;
        public SecurityKey ProofKey { get; set; }
        public EncryptingCredentials ProofKeyEncryptingCredentials
        {
            get
            {
                if (_useSingleEncryptingCredentials) return EncryptingCredentials;
                return _proofKeyEncryptingCredentials;
            }
            set
            {
                _useSingleEncryptingCredentials = false;
                _proofKeyEncryptingCredentials = value;
            }
        }
    }
}
