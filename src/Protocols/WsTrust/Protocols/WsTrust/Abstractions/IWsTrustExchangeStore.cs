namespace Solid.Identity.Protocols.WsTrust.Abstractions
{
    /// <summary>Stores pending exchange state with atomic admission, transition and consumption.</summary>
    public interface IWsTrustExchangeStore
    {
        bool TryAdd(string context, WsTrustPendingExchange exchange);
        bool TryGet(string context, out WsTrustPendingExchange exchange);
        bool TryUpdate(string context, WsTrustPendingExchange current, WsTrustPendingExchange next);
        bool TryRemove(string context, WsTrustPendingExchange current);
    }
}
