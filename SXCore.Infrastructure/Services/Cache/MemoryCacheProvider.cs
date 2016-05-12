using SXCore.Common.Contracts;
using System;
using System.Runtime.Caching;

namespace SXCore.Infrastructure.Services.Cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        protected MemoryCache _cache;
        protected TimeSpan _defaultTimeSpan = new TimeSpan(24, 0, 0);

        public MemoryCacheProvider()
        {
            _cache = new MemoryCache("_");
        }

        public MemoryCacheProvider(string name)
        {
            _cache = new MemoryCache(name);
        }

        public object this[string key]
        { get { return this.Get(key); } }

        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public object Get(string key)
        {
            return _cache.Contains(key) ? _cache.Get(key) : null;
        }

        public T Get<T>(string key) where T : class
        {
            var item = this.Get(key);
            return item == null ? default(T) : item as T;
        }

        public void Set(string key, object value, TimeSpan timespan)
        {
            if (this.Contains(key))
            {
                _cache[key] = value;
            }
            else
            {
                _cache.Set(new CacheItem(key, value), new CacheItemPolicy() { SlidingExpiration = timespan });
            }
        }

        public void Set(string key, object value)
        {
            this.Set(key, value, _defaultTimeSpan);
        }
    }
}
