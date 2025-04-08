using System.Collections.Generic;

namespace Solid.IdentityModel.Tokens.Saml2.Metadata
{
    public class ContactPerson
    {
        public string Company { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public ICollection<string> EmailAddresses { get; } = new List<string>();
        public ICollection<string> TelephoneNumbers { get; } = new List<string>();
        public ContactType ContactType { get; set; }
    }
}
