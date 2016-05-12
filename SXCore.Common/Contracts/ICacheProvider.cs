using System;

namespace SXCore.Common.Contracts
{
    public interface ICacheProvider
    {
        object this[string key] { get; }

        bool Contains(string key);

        object Get(string key);
        T Get<T>(string key) where T : class;

        void Set(string key, object value, TimeSpan timespan);
        void Set(string key, object value);
    }
}
