namespace Yoda.Cache.Entites
{
    public class CacheNode<TKey, TValue> where TKey : notnull
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public CacheNode(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }

            Key = key;
            Value = value;
        }
    }
}