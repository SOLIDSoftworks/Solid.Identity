using System.Collections.Generic;

namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// Values for constants for WsTrust Feb2005, 1.3 and 1.4.
    /// </summary>
    public class WsTrustConstants : WsProtocolConstants
    {
        /// <summary>
        /// Gets an instance of WsTrust Feb2005 Constants.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrustConstants TrustFeb2005 { get; } = new WsTrustFeb2005();

        /// <summary>
        /// Gets an instance of WsTrust 1.3 Constants.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrustConstants Trust13 { get; } = new WsTrust13();

        // /// <summary>
        // /// Gets an instance of WsTrust 1.4 Constants.
        // /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        // /// </summary>
        // public static WsTrustConstants Trust14 { get; } = new WsTrust14();
        
        /// <summary>
        /// Gets a list of all known namespaces
        /// </summary>
        public static IDictionary<string, WsTrustConstants> KnownNamespaces { get; } = new Dictionary<string, WsTrustConstants>
        {
            { TrustFeb2005.Namespace, TrustFeb2005 },
            { Trust13.Namespace, Trust13 }
        };

        /// <summary>
        /// Gets version specific WsTrust Actions.
        /// </summary>
        public WsTrustActions Actions { get; protected init; }

        /// <summary>
        /// Gets version specific WsTrust KeyTypes.
        /// </summary>
        public WsTrustKeyTypes KeyTypes { get; protected init; }

        /// <summary>
        /// Gets version specific WsTrust KeyTypes.
        /// </summary>
        public WsTrustBinarySecretTypes BinarySecretTypes { get; protected init; }

    }
    
    internal class WsTrustFeb2005 : WsTrustConstants
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrustFeb2005"/>.
        /// <para>The property <see cref="WsTrustConstants.TrustFeb2005"/> maintains a singleton instance of constants for WsTrust Feb2005.</para>
        /// </summary>
        public WsTrustFeb2005()
        {
            Namespace = "http://schemas.xmlsoap.org/ws/2005/02/trust";
            DefaultPrefix = "t";
            Actions = WsTrustActions.TrustFeb2005;
            BinarySecretTypes = WsTrustBinarySecretTypes.TrustFeb2005;
            KeyTypes = WsTrustKeyTypes.TrustFeb2005;
        }
    }
    
    internal class WsTrust13 : WsTrustConstants
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrust13"/>.
        /// <para>The property <see cref="WsTrustConstants.Trust13"/> maintains a singleton instance of constants for WsTrust 1.3.</para>
        /// </summary>
        public WsTrust13()
        {
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200512";
            DefaultPrefix = "trust";
            Actions = WsTrustActions.Trust13;
            BinarySecretTypes = WsTrustBinarySecretTypes.Trust13;
            KeyTypes = WsTrustKeyTypes.Trust13;
        }
    }
    
    internal class WsTrust14 : WsTrustConstants
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrust14"/>.
        /// <para>The property <see cref="WsTrustConstants.Trust14"/> maintains a singleton instance of constants for WsTrust 1.4.</para>
        /// </summary>
        public WsTrust14()
        {
            Namespace = "http://docs.oasis-open.org/ws-sx/ws-trust/200802";
            DefaultPrefix = "tr";
            Actions = WsTrustActions.Trust14;
            BinarySecretTypes = WsTrustBinarySecretTypes.Trust14;
            KeyTypes = WsTrustKeyTypes.Trust14;
        }
    }
}
