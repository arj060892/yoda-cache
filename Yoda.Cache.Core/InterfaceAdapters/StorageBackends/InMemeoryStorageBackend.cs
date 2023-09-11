using System.Collections.Concurrent;
using Yoda.Cache.Entites;

namespace Yoda.Cache.Core.InterfaceAdapters.StorageBackends
{
    public class InMemoryStorageBackend<TKey, TValue> : IStorageBackend<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, CacheNode<TKey, TValue>> _storage = new();

        public Task<TValue> GetAsync(TKey key)
        {
            if (_storage.TryGetValue(key, out CacheNode<TKey, TValue> node))
            {
                return Task.FromResult(node.Value);
            }
            return Task.FromResult(default(TValue));
        }

        public Task PutAsync(TKey key, TValue value)
        {
            _storage[key] = new CacheNode<TKey, TValue>(key, value); ;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(TKey key)
        {
            _storage.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task<int> GetSizeAsync()
        {
            return Task.FromResult(_storage.Count);
        }
    }
}