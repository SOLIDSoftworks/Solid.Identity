﻿using System.Text;

namespace Solid.IdentityModel.FederationMetadata.Tests
{
    public static class ExampleMetadata
    {
        public const string UnsignedExampleSecurityTokenService = @"<md:EntityDescriptor xmlns:md=""urn:oasis:names:tc:SAML:2.0:metadata""
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
</md:EntityDescriptor>";
    }
}
