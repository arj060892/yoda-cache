using Yoda.Cache.Entites;

namespace Yoda.Cache.Core.InterfaceAdapters.EvictionStrategies
{
    public interface IEvictionStrategy<TKey, TValue> where TKey : notnull
    {
        Task<CacheNode<TKey, TValue>> GetNodeToEvictAsync();
        Task NotifyAccessAsync(CacheNode<TKey, TValue> node);
        Task NotifyAdditionAsync(CacheNode<TKey, TValue> node);
    }
}
