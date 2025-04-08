using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.FederationMetadata.Tests
{
    public static class ExampleMetadata
    {
        public const string SignedExampleSecurityTokenService = @"<md:EntityDescriptor xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata""
                     entityID=""http://localhost:5000"">
   <md:RoleDescriptor xmlns:fed=""http://docs.oasis-open.org/wsfed/federation/200706""
                      xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                      xsi:type=""fed:SecurityTokenServiceType""
                      protocolSupportEnumeration=""http://docs.oasis-open.org/wsfed/federation/200706"">
      <md:KeyDescriptor use=""signing"">
         <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
            <X509Data>
               <X509Certificate>
                  MIIDBTCCAfGgAwIBAgIQNQb+T2ncIrNA6cKvUA1GWTAJBgUrDgMCHQUAMBIxEDAOBgNVBAMTB0RldlJvb3QwHhcNMTAwMTIwMjIwMDAwWhcNMjAwMTIwMjIwMDAwWjAVMRMwEQYDVQQDEwppZHNydjN0ZXN0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqnTksBdxOiOlsmRNd+mMS2M3o1IDpK4uAr0T4/YqO3zYHAGAWTwsq4ms+NWynqY5HaB4EThNxuq2GWC5JKpO1YirOrwS97B5x9LJyHXPsdJcSikEI9BxOkl6WLQ0UzPxHdYTLpR4/O+0ILAlXw8NU4+jB4AP8Sn9YGYJ5w0fLw5YmWioXeWvocz1wHrZdJPxS8XnqHXwMUozVzQj+x6daOv5FmrHU1r9/bbp0a1GLv4BbTtSh4kMyz1hXylho0EvPg5p9YIKStbNAW9eNWvv5R8HN7PPei21AsUqxekK0oW9jnEdHewckToX7x5zULWKwwZIksll0XnVczVgy7fCFwIDAQABo1wwWjATBgNVHSUEDDAKBggrBgEFBQcDATBDBgNVHQEEPDA6gBDSFgDaV+Q2d2191r6A38tBoRQwEjEQMA4GA1UEAxMHRGV2Um9vdIIQLFk7exPNg41NRNaeNu0I9jAJBgUrDgMCHQUAA4IBAQBUnMSZxY5xosMEW6Mz4WEAjNoNv2QvqNmk23RMZGMgr516ROeWS5D3RlTNyU8FkstNCC4maDM3E0Bi4bbzW3AwrpbluqtcyMN3Pivqdxx+zKWKiORJqqLIvN8CT1fVPxxXb/e9GOdaR8eXSmB0PgNUhM4IjgNkwBbvWC9F/lzvwjlQgciR7d4GfXPYsE1vf8tmdQaY8/PtdAkExmbrb9MihdggSoGXlELrPA91Yce+fiRcKY3rQlNWVd4DOoJ/cPXsXwry8pWjNCo5JD8Q+RQ5yZEy7YPoifwemLhTdsBz3hlZr28oCGJ3kbnpW0xGvQb3VHSTVVbeei0CfXoW6iz1
                </X509Certificate>
            </X509Data>
         </KeyInfo>
      </md:KeyDescriptor>
      <fed:PassiveRequestorEndpoint>
         <wsa:EndpointReference xmlns:wsa=""http://www.w3.org/2005/08/addressing"">
            <wsa:Address>http://localhost:5000/wsfed</wsa:Address>
         </wsa:EndpointReference>
      </fed:PassiveRequestorEndpoint>
   </md:RoleDescriptor>
   <Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
      <SignedInfo>
         <CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
         <SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" />
         <Reference URI=""#id"">
            <Transforms>
               <Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" />
               <Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" />
            </Transforms>
            <DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" />
            <DigestValue>+zDVB+ZyWfMhQKTUSBsEVhTOZ3F/nHBofGwXHNnHBB4=</DigestValue>
         </Reference>
      </SignedInfo>
      <SignatureValue>bW0tpiape4eW9WaTGA8fKgEmXFbzsnMmpH95WEYLK4itByLequvDl603FrSUOFo2m0juZvp6B8pxDckV2xpp6igIUEh1ViXHWZU24BKBl7CbuiZ6SGKyLCdZJy4n4/udqpOXp6F759HoszN1mQYfOe/+hODDR0SZKUta6VDsMBZHRYBJcU24H1Fl8WY2JtNyfSG94Ok3T8CPgeN9zRXKoBo32p2RO/Qbki8528+r6kletWNBFw7k3U2YOgYiCh6CPcSGkTTUeDJjg11Y8UPvqveUorjTwIUe3Jvwsb3izfTqUDNzeQDbzZlbsfPCTUYYSqT3Dq0jHsXhQaphq71i2A==</SignatureValue>
      <KeyInfo>
         <X509Data>
            <X509Certificate>
               MIIDBTCCAfGgAwIBAgIQNQb+T2ncIrNA6cKvUA1GWTAJBgUrDgMCHQUAMBIxEDAOBgNVBAMTB0RldlJvb3QwHhcNMTAwMTIwMjIwMDAwWhcNMjAwMTIwMjIwMDAwWjAVMRMwEQYDVQQDEwppZHNydjN0ZXN0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqnTksBdxOiOlsmRNd+mMS2M3o1IDpK4uAr0T4/YqO3zYHAGAWTwsq4ms+NWynqY5HaB4EThNxuq2GWC5JKpO1YirOrwS97B5x9LJyHXPsdJcSikEI9BxOkl6WLQ0UzPxHdYTLpR4/O+0ILAlXw8NU4+jB4AP8Sn9YGYJ5w0fLw5YmWioXeWvocz1wHrZdJPxS8XnqHXwMUozVzQj+x6daOv5FmrHU1r9/bbp0a1GLv4BbTtSh4kMyz1hXylho0EvPg5p9YIKStbNAW9eNWvv5R8HN7PPei21AsUqxekK0oW9jnEdHewckToX7x5zULWKwwZIksll0XnVczVgy7fCFwIDAQABo1wwWjATBgNVHSUEDDAKBggrBgEFBQcDATBDBgNVHQEEPDA6gBDSFgDaV+Q2d2191r6A38tBoRQwEjEQMA4GA1UEAxMHRGV2Um9vdIIQLFk7exPNg41NRNaeNu0I9jAJBgUrDgMCHQUAA4IBAQBUnMSZxY5xosMEW6Mz4WEAjNoNv2QvqNmk23RMZGMgr516ROeWS5D3RlTNyU8FkstNCC4maDM3E0Bi4bbzW3AwrpbluqtcyMN3Pivqdxx+zKWKiORJqqLIvN8CT1fVPxxXb/e9GOdaR8eXSmB0PgNUhM4IjgNkwBbvWC9F/lzvwjlQgciR7d4GfXPYsE1vf8tmdQaY8/PtdAkExmbrb9MihdggSoGXlELrPA91Yce+fiRcKY3rQlNWVd4DOoJ/cPXsXwry8pWjNCo5JD8Q+RQ5yZEy7YPoifwemLhTdsBz3hlZr28oCGJ3kbnpW0xGvQb3VHSTVVbeei0CfXoW6iz1
            </X509Certificate>
         </X509Data>
      </KeyInfo>
   </Signature>
</md:EntityDescriptor>";

        public const string UnsignedExampleSpSso = @"<EntityDescriptor
    xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""loadbalancer-9.siroe.com"">
    <SPSSODescriptor
        AuthnRequestsSigned=""false""
        WantAssertionsSigned=""false""
        protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
        <KeyDescriptor use=""signing"">
            <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                <X509Data>
                    <X509Certificate>
MIICYDCCAgqgAwIBAgICBoowDQYJKoZIhvcNAQEEBQAwgZIxCzAJBgNVBAYTAlVTMRMwEQYDVQQI
EwpDYWxpZm9ybmlhMRQwEgYDVQQHEwtTYW50YSBDbGFyYTEeMBwGA1UEChMVU3VuIE1pY3Jvc3lz
dGVtcyBJbmMuMRowGAYDVQQLExFJZGVudGl0eSBTZXJ2aWNlczEcMBoGA1UEAxMTQ2VydGlmaWNh
dGUgTWFuYWdlcjAeFw0wNjExMDIxOTExMzRaFw0xMDA3MjkxOTExMzRaMDcxEjAQBgNVBAoTCXNp
cm9lLmNvbTEhMB8GA1UEAxMYbG9hZGJhbGFuY2VyLTkuc2lyb2UuY29tMIGfMA0GCSqGSIb3DQEB
AQUAA4GNADCBiQKBgQCjOwa5qoaUuVnknqf5pdgAJSEoWlvx/jnUYbkSDpXLzraEiy2UhvwpoBgB
EeTSUaPPBvboCItchakPI6Z/aFdH3Wmjuij9XD8r1C+q//7sUO0IGn0ORycddHhoo0aSdnnxGf9V
tREaqKm9dJ7Yn7kQHjo2eryMgYxtr/Z5Il5F+wIDAQABo2AwXjARBglghkgBhvhCAQEEBAMCBkAw
DgYDVR0PAQH/BAQDAgTwMB8GA1UdIwQYMBaAFDugITflTCfsWyNLTXDl7cMDUKuuMBgGA1UdEQQR
MA+BDW1hbGxhQHN1bi5jb20wDQYJKoZIhvcNAQEEBQADQQB/6DOB6sRqCZu2OenM9eQR0gube85e
nTTxU4a7x1naFxzYXK1iQ1vMARKMjDb19QEJIEJKZlDK4uS7yMlf1nFS
                    </X509Certificate>
                </X509Data>
            </KeyInfo>
        </KeyDescriptor>
        <KeyDescriptor use=""encryption"">
            <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
                <X509Data>
                    <X509Certificate>
MIICTDCCAfagAwIBAgICBo8wDQYJKoZIhvcNAQEEBQAwgZIxCzAJBgNVBAYTAlVTMRMwEQYDVQQI
EwpDYWxpZm9ybmlhMRQwEgYDVQQHEwtTYW50YSBDbGFyYTEeMBwGA1UEChMVU3VuIE1pY3Jvc3lz
dGVtcyBJbmMuMRowGAYDVQQLExFJZGVudGl0eSBTZXJ2aWNlczEcMBoGA1UEAxMTQ2VydGlmaWNh
dGUgTWFuYWdlcjAeFw0wNjExMDcyMzU2MTdaFw0xMDA4MDMyMzU2MTdaMCMxITAfBgNVBAMTGGxv
YWRiYWxhbmNlci05LnNpcm9lLmNvbTCBnzANBgkqhkiG9w0BAQEFAAOBjQAwgYkCgYEAw574iRU6
HsSO4LXW/OGTXyfsbGv6XRVOoy3v+J1pZ51KKejcDjDJXNkKGn3/356AwIaqbcymWd59T0zSqYfR
Hn+45uyjYxRBmVJseLpVnOXLub9jsjULfGx0yjH4w+KsZSZCXatoCHbj/RJtkzuZY6V9to/hkH3S
InQB4a3UAgMCAwEAAaNgMF4wEQYJYIZIAYb4QgEBBAQDAgZAMA4GA1UdDwEB/wQEAwIE8DAfBgNV
HSMEGDAWgBQ7oCE35Uwn7FsjS01w5e3DA1CrrjAYBgNVHREEETAPgQ1tYWxsYUBzdW4uY29tMA0G
CSqGSIb3DQEBBAUAA0EAMlbfBg/ff0Xkv4DOR5LEqmfTZKqgdlD81cXynfzlF7XfnOqI6hPIA90I
x5Ql0ejivIJAYcMGUyA+/YwJg2FGoA==
                    </X509Certificate>
                </X509Data>
            </KeyInfo>
            <EncryptionMethod Algorithm=
                ""https://www.w3.org/2001/04/xmlenc#aes128-cbc"">
                <KeySize xmlns=""https://www.w3.org/2001/04/xmlenc#"">128</KeySize>
            </EncryptionMethod>
        </KeyDescriptor>
        <SingleLogoutService
            Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
            Location=""https://LoadBalancer-9.siroe.com:3443/federation/
            SPSloRedirect/metaAlias/sp""
            ResponseLocation=""https://LoadBalancer-9.siroe.com:3443/
            federation/SPSloRedirect/metaAlias/sp""/>
        <SingleLogoutService
            Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP""
            Location=""https://LoadBalancer-9.siroe.com:3443/
            federation/SPSloSoap/metaAlias/sp""/>
       <ManageNameIDService
            Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
            Location=""https://LoadBalancer-9.siroe.com:3443/federation/
            SPMniRedirect/metaAlias/sp""
            ResponseLocation=""https://LoadBalancer-9.siroe.com:3443/
            federation/SPMniRedirect/metaAlias/sp""/>
        <ManageNameIDService
            Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP""
            Location=""https://LoadBalancer-9.siroe.com:3443/
            federation/SPMniSoap/metaAlias/sp""
            ResponseLocation=""https://LoadBalancer-9.siroe.com:3443/
            federation/SPMniSoap/metaAlias/sp""/>
        <NameIDFormat>
            urn:oasis:names:tc:SAML:2.0:nameid-format:persistent
        </NameIDFormat>
        <NameIDFormat>
            urn:oasis:names:tc:SAML:2.0:nameid-format:transient
        </NameIDFormat>
        <AssertionConsumerService
            isDefault=""true""
            index=""0""
            Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact""
            Location=""https://LoadBalancer-9.siroe.com:3443/
            federation/Consumer/metaAlias/sp""/>
        <AssertionConsumerService
            index=""1""
            Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
            Location=""https://LoadBalancer-9.siroe.com:3443/
            federation/Consumer/metaAlias/sp""/>
    </SPSSODescriptor>
</EntityDescriptor>";

        public const string UnsignedExample2Roles = @"<md:EntityDescriptor entityID=""https://idp.example.org/idp/shibboleth"" validUntil=""2010-01-01T00:00:00Z"" xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata"">
 
  <md:IDPSSODescriptor protocolSupportEnumeration=""urn:mace:shibboleth:1.0 urn:oasis:names:tc:SAML:1.1:protocol urn:oasis:names:tc:SAML:2.0:protocol"">
  
    <md:ArtifactResolutionService Location=""https://idp.example.org:8443/idp/profile/SAML1/SOAP/ArtifactResolution""
      Binding=""urn:oasis:names:tc:SAML:1.0:bindings:SOAP-binding"" index=""1""/>
    <md:ArtifactResolutionService Location=""https://idp.example.org:8443/idp/profile/SAML2/SOAP/ArtifactResolution""
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP"" index=""2""/>
 
    <md:NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</md:NameIDFormat>
    <md:NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</md:NameIDFormat>
 
    <md:SingleSignOnService Location=""https://idp.example.org/idp/profile/Shibboleth/SSO""
      Binding=""urn:mace:shibboleth:1.0:profiles:AuthnRequest""/>
    <md:SingleSignOnService Location=""https://idp.example.org/idp/profile/SAML2/POST/SSO""
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""/>
    <md:SingleSignOnService Location=""https://idp.example.org/idp/profile/SAML2/POST-SimpleSign/SSO""
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST-SimpleSign""/>
    <md:SingleSignOnService Location=""https://idp.example.org/idp/profile/SAML2/Redirect/SSO""
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""/>
    <md:SingleSignOnService Location=""https://idp.example.org/idp/profile/SAML2/SOAP/ECP""
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP""/>
 </md:IDPSSODescriptor>
 
  <md:AttributeAuthorityDescriptor protocolSupportEnumeration=""urn:oasis:names:tc:SAML:1.1:protocol urn:oasis:names:tc:SAML:2.0:protocol"">
  
    <md:AttributeService Location=""https://idp.example.org:8443/idp/profile/SAML1/SOAP/AttributeQuery""
      Binding=""urn:oasis:names:tc:SAML:1.0:bindings:SOAP-binding""/>
    <md:AttributeService Location=""https://idp.example.org:8443/idp/profile/SAML2/SOAP/AttributeQuery""
      Binding=""urn:oasis:names:tc:SAML:2.0:bindings:SOAP""/>
 
    <md:NameIDFormat>urn:mace:shibboleth:1.0:nameIdentifier</md:NameIDFormat>
    <md:NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</md:NameIDFormat>
 
  </md:AttributeAuthorityDescriptor>
 
  <md:Organization>
    <md:OrganizationName xml:lang=""en"">Example Organization, Ltd.</md:OrganizationName>
    <md:OrganizationDisplayName xml:lang=""en"">Example Organization</md:OrganizationDisplayName>
    <md:OrganizationURL xml:lang=""en"">http://www.example.org/</md:OrganizationURL>
  </md:Organization>
 
</md:EntityDescriptor>";
    }
}
