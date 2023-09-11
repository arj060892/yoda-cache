namespace Yoda.Cache.API.Helpers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CacheKeyTypeAttribute : Attribute
    {
        public Type KeyType { get; }

        public CacheKeyTypeAttribute(Type keyType)
        {
            KeyType = keyType;
        }
    }
}