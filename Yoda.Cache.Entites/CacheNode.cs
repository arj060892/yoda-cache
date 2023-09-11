namespace Yoda.Cache.Entites
{
    public class CacheNode<TKey, TValue> where TKey : notnull
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public CacheNode<TKey, TValue> Previous { get; set; }
        public CacheNode<TKey, TValue> Next { get; set; }
    }
}