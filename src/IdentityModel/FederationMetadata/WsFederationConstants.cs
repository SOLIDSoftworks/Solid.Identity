using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata
{
    public static class WsFederationConstants
    {
        public static readonly string Namespace = "http://docs.oasis-open.org/wsfed/federation/200706";
        public static readonly string Prefix = "fed";
        public static class Elements
        {
            public static readonly string LogicalServiceNamesOffered = nameof(LogicalServiceNamesOffered);
            public static readonly string TokenTypesOffered = nameof(TokenTypesOffered);
            public static readonly string ClaimDialectsOffered = nameof(ClaimDialectsOffered);
            public static readonly string ClaimTypesOffered = nameof(ClaimTypesOffered);
            public static readonly string ClaimTypesRequested = nameof(ClaimTypesRequested);
            public static readonly string AutomaticPseudonyms = nameof(AutomaticPseudonyms);
            public static readonly string TargetScopes = nameof(TargetScopes);
            public static readonly string SecurityTokenServiceEndpoint = nameof(SecurityTokenServiceEndpoint);
            public static readonly string SingleSignOutSubscriptionEndpoint = nameof(SingleSignOutSubscriptionEndpoint);
            public static readonly string SingleSignOutNotificationEndpoint = nameof(SingleSignOutNotificationEndpoint);
            public static readonly string PassiveRequestorEndpoint = nameof(PassiveRequestorEndpoint);
            public static readonly string PseudonymServiceEndpoint = nameof(PseudonymServiceEndpoint);
            public static readonly string AttributeServiceEndpoint = nameof(AttributeServiceEndpoint);
            public static readonly string ApplicationServiceEndpoint = nameof(ApplicationServiceEndpoint);
            public static readonly string IssuerName = nameof(IssuerName);
            public static readonly string TokenType = nameof(TokenType);
            public static readonly string ClaimDialect = nameof(ClaimDialect);
        }

        public static class Attributes
        {
            public static readonly string ServiceDisplayName = nameof(ServiceDisplayName);
            public static readonly string ServiceDescription = nameof(ServiceDescription);
            public static readonly string Uri = nameof(Uri);
        }
    }
}
