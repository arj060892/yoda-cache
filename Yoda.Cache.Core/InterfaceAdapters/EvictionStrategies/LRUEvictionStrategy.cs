using Yoda.Cache.Entites;

namespace Yoda.Cache.Core.InterfaceAdapters.EvictionStrategies
{
    public class LRUEvictionStrategy<TKey, TValue> : IEvictionStrategy<TKey, TValue>
    {
        private readonly object _lock = new();
        private readonly LinkedList<CacheNode<TKey, TValue>> _accessOrder = new();

        public Task<CacheNode<TKey, TValue>> GetNodeToEvictAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_accessOrder.Last?.Value);
            }
        }

        public Task NotifyAccessAsync(CacheNode<TKey, TValue> node)
        {
            lock (_lock)
            {
                _accessOrder.Remove(node);
                _accessOrder.AddFirst(node);
                return Task.CompletedTask;
            }
        }

        public Task NotifyAdditionAsync(CacheNode<TKey, TValue> node)
        {
            lock (_lock)
            {
                _accessOrder.AddFirst(node);
                return Task.CompletedTask;
            }
        }
    }
}