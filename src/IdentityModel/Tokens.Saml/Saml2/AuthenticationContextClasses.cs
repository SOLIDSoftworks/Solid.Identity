using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.IdentityModel.Tokens.Saml2
{
    /// <summary>
    /// Authentication context classes for use with SAML2 tokens.
    /// </summary>
    public static class AuthenticationContextClasses
    {
        /// <summary>
        /// The base namespace for SAML2 authentication context classes.
        /// </summary>
        public const string Namespace = "urn:oasis:names:tc:SAML:2.0:ac:classes:";

        /// <summary>
        /// The Internet Protocol class is applicable when a principal is authenticated through
        /// the use of a provided IP address.
        /// </summary>
        public const string InternetProtocol = Namespace + nameof(InternetProtocol);

        /// <summary>
        /// The Internet Protocol Password class is applicable when a principal is authenticated
        /// through the use of a provided IP address, in addition to a username/password.
        /// </summary>
        public const string InternetProtocolPassword = Namespace + nameof(InternetProtocolPassword);

        /// <summary>
        /// This class is applicable when the principal has authenticated using a password to a
        /// local authentication authority, in order to acquire a Kerberos ticket.That Kerberos
        /// ticket is then used for subsequent network authentication.
        /// </summary>
        public const string Kerberos = Namespace + nameof(Kerberos);

        /// <summary>
        /// Reflects no mobile customer registration procedures and an authentication of the mobile device without
        /// requiring explicit end-user interaction. This context class authenticates only the device and never the user;
        /// it is useful when services other than the mobile operator want to add a secure device authentication to
        /// their authentication process.
        /// </summary>
        public const string MobileOneFactorUnregistered = Namespace + nameof(MobileOneFactorUnregistered);

        /// <summary>
        /// Reflects no mobile customer registration procedures and a two-factor based authentication, such as
        /// secure device and user PIN.This context class is useful when a service other than the mobile operator
        /// wants to link their customer ID to a mobile supplied two-factor authentication service by capturing mobile
        /// phone data at enrollment.
        /// </summary>
        public const string MobileTwoFactorUnregistered = Namespace + nameof(MobileTwoFactorUnregistered);

        /// <summary>
        /// Reflects mobile contract customer registration procedures and a single factor authentication. For example,
        /// a digital signing device with tamper resistant memory for key storage, such as the mobile MSISDN, but no
        /// required PIN or biometric for real-time user authentication.
        /// </summary>
        public const string MobileOneFactorContract = Namespace + nameof(MobileOneFactorContract);

        /// <summary>
        /// Reflects mobile contract customer registration procedures and a two-factor based authentication. For
        /// example, a digital signing device with tamper resistant memory for key storage, such as a GSM SIM, that
        /// requires explicit proof of user identity and intent, such as a PIN or biometric.
        /// </summary>
        public const string MobileTwoFactorContract = Namespace + nameof(MobileTwoFactorContract);

        /// <summary>
        /// The Password class is applicable when a principal authenticates to an authentication authority through the
        /// presentation of a password over an unprotected HTTP session.
        /// </summary>
        public const string Password = Namespace + nameof(Password);

        /// <summary>
        /// The PasswordProtectedTransport class is applicable when a principal authenticates to an authentication
        /// authority through the presentation of a password over a protected session.
        /// </summary>
        public const string PasswordProtectedTransport = Namespace + nameof(PasswordProtectedTransport);

        /// <summary>
        /// The PreviousSession class is applicable when a principal had authenticated to an authentication authority
        /// at some point in the past using any authentication context supported by that authentication authority.
        /// Consequently, a subsequent authentication event that the authentication authority will assert to the relying
        /// party may be significantly separated in time from the principal's current resource access request.
        /// <para>The context for the previously authenticated session is explicitly not included in this context class because
        /// the user has not authenticated during this session, and so the mechanism that the user employed to
        /// authenticate in a previous session should not be used as part of a decision on whether to now allow
        /// access to a resource.</para>
        /// </summary>
        public const string PreviousSession = Namespace + nameof(PreviousSession);

        /// <summary>
        /// The X509 context class indicates that the principal authenticated by means of a digital signature where the
        /// key was validated as part of an X.509 Public Key Infrastructure.
        /// </summary>
        public const string X509 = Namespace + nameof(X509);

        /// <summary>
        /// The PGP context class indicates that the principal authenticated by means of a digital signature where the
        /// key was validated as part of a PGP Public Key Infrastructure.
        /// </summary>
        public const string Pgp = Namespace + "PGP";

        /// <summary>
        /// Note that this URI is also used as the target namespace in the corresponding authentication context class
        /// schema document[SAMLAC - SPKI].
        /// </summary>
        public const string Spki = Namespace + "SPKI";

        /// <summary>
        /// This context class indicates that the principal authenticated by means of a digital signature according to
        /// the processing rules specified in the XML Digital Signature specification[XMLSig].
        /// </summary>
        public const string XmlDigitalSignature = Namespace + "XMLDSig";

        /// <summary>
        /// The Smartcard class is identified when a principal authenticates to an authentication authority using a
        /// smartcard.
        /// </summary>
        public const string Smartcard = Namespace + nameof(Smartcard);

        /// <summary>
        /// The SmartcardPKI class is applicable when a principal authenticates to an authentication authority through
        /// a two-factor authentication mechanism using a smartcard with enclosed private key and a PIN.
        /// </summary>
        public const string SmartcardPki = Namespace + "SmartcardPKI";

        /// <summary>
        /// The Software-PKI class is applicable when a principal uses an X.509 certificate stored in software to
        /// authenticate to the authentication authority.
        /// </summary>
        public const string SoftwarePki = Namespace + "SoftwarePKI";

        /// <summary>
        /// This class is used to indicate that the principal authenticated via the provision of a fixed-line telephone
        /// number, transported via a telephony protocol such as ADSL.
        /// </summary>
        public const string Telephony = Namespace + nameof(Telephony);

        /// <summary>
        /// Indicates that the principal is "roaming" (perhaps using a phone card) and authenticates via the means of
        /// the line number, a user suffix, and a password element.
        /// </summary>
        public const string NomadTelephony = Namespace + nameof(NomadTelephony);

        /// <summary>
        /// This class is used to indicate that the principal authenticated via the provision of a fixed-line telephone
        /// number and a user suffix, transported via a telephony protocol such as ADSL.
        /// </summary>
        public const string PersonalTelephony = Namespace + nameof(PersonalTelephony);

        /// <summary>
        /// Indicates that the principal authenticated via the means of the line number, a user suffix, and a password
        /// element.
        /// </summary>
        public const string AuthenticatedTelephony = Namespace + nameof(AuthenticatedTelephony);

        /// <summary>
        /// The Secure Remote Password class is applicable when the authentication was performed by means of
        /// Secure Remote Password as specified in [RFC 2945].
        /// </summary>
        public const string SecureRemotePassword = Namespace + nameof(SecureRemotePassword);

        /// <summary>
        /// This class indicates that the principal authenticated by means of a client certificate, secured with the
        /// SSL/TLS transport.
        /// </summary>
        public const string TlsClient = Namespace + "TLSClient";

        /// <summary>
        /// The TimeSyncToken class is applicable when a principal authenticates through a time synchronization
        /// token.
        /// </summary>
        public const string TimeSyncToken = Namespace + nameof(TimeSyncToken);

        /// <summary>
        /// The Unspecified class indicates that the authentication was performed by unspecified means
        /// </summary>
        public const string Unspecified = Namespace + nameof(Unspecified);
    }
}
