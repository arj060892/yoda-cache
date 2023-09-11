using Yoda.Cache.Entites;

namespace Yoda.Cache.InterfaceAdapters.EvictionStrategies
{
    public interface IEvictionStrategy<TKey, TValue>
    {
        Task<CacheNode<TKey, TValue>> GetNodeToEvictAsync();
        Task NotifyAccessAsync(CacheNode<TKey, TValue> node);
        Task NotifyAdditionAsync(CacheNode<TKey, TValue> node);
    }
}
