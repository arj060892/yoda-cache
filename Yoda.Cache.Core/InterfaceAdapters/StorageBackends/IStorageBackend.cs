namespace Yoda.Cache.Core.InterfaceAdapters.StorageBackends
{
    public interface IStorageBackend<TKey, TValue> where TKey : notnull
    {
        Task<TValue> GetAsync(TKey key);

        Task PutAsync(TKey key, TValue value);

        Task RemoveAsync(TKey key);

        Task<int> GetSizeAsync();
    }
}