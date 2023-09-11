using Yoda.Cache.Core.Entities;
using Yoda.Cache.Core.InterfaceAdapters.EvictionStrategies;
using Yoda.Cache.Core.InterfaceAdapters.StorageBackends;
using Yoda.Cache.Entites;

namespace Yoda.Cache.Core.UseCases
{
    public class CacheOperations<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
    {
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly IEvictionStrategy<TKey, TValue> _evictionStrategy;
        private readonly IStorageBackend<TKey, TValue> _storageBackend;

        public int Capacity { get; }

        public event EventHandler<ItemEvictedEventArgs<TKey, TValue>> ItemEvicted;

        public CacheOperations(int capacity, IEvictionStrategy<TKey, TValue> evictionStrategy, IStorageBackend<TKey, TValue> storageBackend)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be a positive number.", nameof(capacity));
            }

            Capacity = capacity;
            _evictionStrategy = evictionStrategy;
            _storageBackend = storageBackend;
        }

        public async Task<TValue> GetAsync(TKey key)
        {
            await _lock.WaitAsync();
            try
            {
                TValue value = await _storageBackend.GetAsync(key);
                if (value != null)
                {
                    CacheNode<TKey, TValue> node = new CacheNode<TKey, TValue>(key, value);
                    await _evictionStrategy.NotifyAccessAsync(node);
                }
                return value;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task PutAsync(TKey key, TValue value)
        {
            await _lock.WaitAsync();
            try
            {
                if (_storageBackend.GetSizeAsync().Result >= Capacity)
                {
                    CacheNode<TKey, TValue> nodeToEvict = await _evictionStrategy.GetNodeToEvictAsync();
                    if (nodeToEvict != null)
                    {
                        await _storageBackend.RemoveAsync(nodeToEvict.Key);
                        OnItemEvicted(nodeToEvict.Key, nodeToEvict.Value);
                    }
                }

                await _storageBackend.PutAsync(key, value);
                CacheNode<TKey, TValue> newNode = new CacheNode<TKey, TValue>(key, value);
                await _evictionStrategy.NotifyAdditionAsync(newNode);
            }
            finally
            {
                _lock.Release();
            }
        }

        protected virtual void OnItemEvicted(TKey key, TValue value)
        {
            ItemEvicted?.Invoke(this, new ItemEvictedEventArgs<TKey, TValue>(key, value));
        }
    }
}