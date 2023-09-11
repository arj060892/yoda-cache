using Yoda.Cache.API.Helpers;

namespace Yoda.Cache.API.Models
{
    [CacheKeyType(typeof(int))]
    public class User : ICachableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}