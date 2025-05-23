using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// Values for constants for WsTrust Feb2005, 1.3 and 1.4.
    /// </summary>
    public abstract class WsTrustConstants : WsConstantsBase
    {
        /// <summary>
        /// Gets a list of all known namespaces
        /// </summary>
        public static IList<string> KnownNamespaces { get; } = new List<string> { "http://schemas.xmlsoap.org/ws/2005/02/trust", "http://docs.oasis-open.org/ws-sx/ws-trust/200512", "http://docs.oasis-open.org/ws-sx/ws-trust/200802" };

        /// <summary>
        /// Gets version specific WsTrust Actions.
        /// </summary>
        public WsTrustActions WsTrustActions { get; protected set; }

        /// <summary>
        /// Gets version specific WsTrust KeyTypes.
        /// </summary>
        public WsTrustKeyTypes WsTrustKeyTypes { get; protected set; }

        /// <summary>
        /// Gets version specific WsTrust KeyTypes.
        /// </summary>
        public WsTrustBinarySecretTypes WsTrustBinarySecretTypes { get; protected set; }

        /// <summary>
        /// Gets the an instance of WsTrust Feb2005 Constants.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrustFeb2005Constants TrustFeb2005 { get; } = new WsTrustFeb2005Constants();

        /// <summary>
        /// Gets the an instance of WsTrust 1.3 Constants.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrust13Constants Trust13 { get; } = new WsTrust13Constants();

        /// <summary>
        /// Gets the an instance of WsTrust 1.4 Constants.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrust14Constants Trust14 { get; } = new WsTrust14Constants();
    }

    /// <summary>
    /// Provides constants for WsTrust Feb2005.
    /// </summary>
    public class WsTrustFeb2005Constants : WsTrustConstants
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrustFeb2005Constants"/>.
        /// <para>The property <see cref="WsTrustConstants.TrustFeb2005"/> maintains a singleton instance of constants for WsTrust Feb2005.</para>
        /// </summary>
        public WsTrustFeb2005Constants()
        {
            Namespace = "http://schemas.xmlsoap.org/ws/2005/02/trust";
            Prefix = "t";
            WsTrustActions = WsTrustActions.TrustFeb2005;
            WsTrustBinarySecretTypes = WsTrustBinarySecretTypes.TrustFeb2005;
            WsTrustKeyTypes = WsTrustKeyTypes.TrustFeb2005;
        }
    }

    /// <summary>
    /// Provides constants for WsTrust 1.3.
    /// </summary>
    public class WsTrust13Constants : WsTrustConstants
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrust13Constants"/>.
        /// <para>The property <see cref="WsTrustConstants.Trust13"/> maintains a singleton instance of constants for WsTrust 1.3.</para>
        /// </summary>
        public WsTrust13Constants()
        {
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512";
            Prefix = "trust";
            WsTrustActions = WsTrustActions.Trust13;
            WsTrustBinarySecretTypes = WsTrustBinarySecretTypes.Trust13;
            WsTrustKeyTypes = WsTrustKeyTypes.Trust13;
        }
    }

    /// <summary>
    /// Provides constants for WsTrust 1.3.
    /// </summary>
    public class WsTrust14Constants : WsTrustConstants
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrust14Constants"/>.
        /// <para>The property <see cref="WsTrustConstants.Trust14"/> maintains a singleton instance of constants for WsTrust 1.4.</para>
        /// </summary>
        public WsTrust14Constants()
        {
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200802";
            Prefix = "tr";
            WsTrustActions = WsTrustActions.Trust14;
            WsTrustBinarySecretTypes = WsTrustBinarySecretTypes.Trust14;
            WsTrustKeyTypes = WsTrustKeyTypes.Trust14;
        }
    }
}
