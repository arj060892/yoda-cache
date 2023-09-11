namespace Yoda.Cache.Core.Entities
{
    public class ItemEvictedEventArgs<TKey, TValue> : EventArgs where TKey : notnull
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public ItemEvictedEventArgs(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}