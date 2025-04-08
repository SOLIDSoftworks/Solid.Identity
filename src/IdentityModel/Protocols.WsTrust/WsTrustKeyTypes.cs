namespace Solid.IdentityModel.Protocols.WsTrust
{
    /// <summary>
    /// Abstract class for singleton pattern for multipule WsTrust versions for KeyTypes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WsTrustKeyTypes<T> : WsTrustKeyTypes where T : new()
    {
        private static T _instance;

        /// <summary>
        /// Implements singleton pattern.
        /// </summary>
        internal static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new T();

                return _instance;
            }
        }
    }

    /// <summary>
    /// Values for KeyTypes for WsTrust Feb2005, 1.3 and 1.4.
    /// </summary>
    public abstract class WsTrustKeyTypes
    {
        /// <summary>
        /// Gets the an instance of WsTrust Feb2005 KeyTypes.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrustFeb2005KeyTypes TrustFeb2005 => WsTrustFeb2005KeyTypes.Instance;

        /// <summary>
        /// Gets the an instance of WsTrust 1.3 KeyTypes.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrust13KeyTypes Trust13 => WsTrust13KeyTypes.Instance;

        /// <summary>
        /// Gets the an instance of WsTrust 1.4 KeyTypes.
        /// <para>see: http://specs.xmlsoap.org/ws/2005/02/trust/WS-Trust.pdf </para>
        /// </summary>
        public static WsTrust14KeyTypes Trust14 => WsTrust14KeyTypes.Instance;

        /// <summary>
        /// Gets the Bearer KeyType.
        /// </summary>
        public string Bearer { get; protected set; }

        /// <summary>
        /// Gets the PublicKey KeyType.
        /// </summary>
        public string PublicKey { get; protected set; }

        /// <summary>
        /// Gets the PSHA1 KeyType.
        /// </summary>
        public string PSHA1 { get; protected set; }

        /// <summary>
        /// Gets the Symmetric KeyType.
        /// </summary>
        public string Symmetric { get; protected set; }
    }

    /// <summary>
    /// Values for KeyTypes for WsTrust Feb2005.
    /// </summary>
    public class WsTrustFeb2005KeyTypes : WsTrustKeyTypes<WsTrustFeb2005KeyTypes>
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrustFeb2005KeyTypes"/>.
        /// <para>The property <see cref="WsTrustKeyTypes.TrustFeb2005"/>  maintains a singleton instance of KeyTypes for WsTrust Feb2005.</para>
        /// </summary>
        public WsTrustFeb2005KeyTypes()
        {
            Bearer = "http://schemas.xmlsoap.org/ws/2005/02/trust/Bearer";
            PSHA1 = "http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1";
            PublicKey = "http://schemas.xmlsoap.org/ws/2005/02/trust/PublicKey";
            Symmetric = "http://schemas.xmlsoap.org/ws/2005/02/trust/SymmetricKey";
        }
    }

    /// <summary>
    /// Values for KeyTypes for WsTrust 1.3.
    /// </summary>
    public class WsTrust13KeyTypes : WsTrustKeyTypes<WsTrust13KeyTypes>
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrustFeb2005KeyTypes"/>.
        /// <para>The property <see cref="WsTrustKeyTypes.TrustFeb2005"/>  maintains a singleton instance of KeyTypes for WsTrust 1.3.</para>
        /// </summary>
        public WsTrust13KeyTypes()
        {
            Bearer = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer";
            PSHA1 = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/CK/PSHA1";
            PublicKey = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/PublicKey";
            Symmetric = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey"; ;
        }
    }

    /// <summary>
    /// Values for KeyTypes for WsTrust 1.4.
    /// </summary>
    public class WsTrust14KeyTypes : WsTrustKeyTypes<WsTrust14KeyTypes>
    {
        /// <summary>
        /// Creates an instance of <see cref="WsTrust14KeyTypes"/>.
        /// <para>The property <see cref="WsTrustKeyTypes.Trust14"/>  maintains a singleton instance of KeyTypes for WsTrust 1.4.</para>
        /// </summary>
        public WsTrust14KeyTypes()
        {
            Bearer = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer";
            PSHA1 = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/CK/PSHA1";
            PublicKey = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/PublicKey";
            Symmetric = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey";
        }
    }
}
