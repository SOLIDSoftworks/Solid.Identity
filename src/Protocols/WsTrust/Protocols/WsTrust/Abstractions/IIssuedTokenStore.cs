using Solid.Identity.Protocols.WsTrust;

namespace Solid.Identity.Protocols.WsTrust.Abstractions
{
    /// <summary>Storage for issued-token lifecycle entries. Implementations must atomically compare revisions on updates and removals.</summary>
    public interface IIssuedTokenStore
    {
        /// <summary>Stores an entry, rejecting expired entries and enforcing the implementation's capacity policy.</summary>
        void Register(string key, IssuedTokenRegistry.Entry entry);

        /// <summary>Looks up an unexpired entry by its token digest key.</summary>
        bool TryGet(string key, out IssuedTokenRegistry.Entry entry);

        /// <summary>Atomically updates an entry only if its revision still matches.</summary>
        bool TryUpdate(string key, IssuedTokenRegistry.Entry expected, IssuedTokenRegistry.Entry updated);

        /// <summary>Atomically removes an entry only if its revision still matches.</summary>
        bool TryRemove(string key, IssuedTokenRegistry.Entry expected);
    }
}
