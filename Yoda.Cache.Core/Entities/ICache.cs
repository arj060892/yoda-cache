using Yoda.Cache.Core.Entities;

namespace Yoda.Cache.Entites
{
    public interface ICache<TKey, TValue> where TKey : notnull
    {
        Task<TValue> GetAsync(TKey key);

        Task PutAsync(TKey key, TValue value);

        int Capacity { get; }

        event EventHandler<ItemEvictedEventArgs<TKey, TValue>> ItemEvicted;
    }
}