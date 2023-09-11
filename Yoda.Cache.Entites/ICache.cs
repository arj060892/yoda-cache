namespace Yoda.Cache.Entites
{
    public interface ICache<TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);
        Task PutAsync(TKey key, TValue value);
        int Capacity { get; }
        IEvictionStrategy<TKey, TValue> EvictionStrategy { get; set; }
        IStorageBackend<TKey, TValue> StorageBackend { get; set; }
        event EventHandler<ItemEvictedEventArgs<TKey, TValue>> ItemEvicted;
    }
}
