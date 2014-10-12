using System.Collections.Generic;
using PowerArhitecture.DataCaching.Specifications;

namespace PowerArhitecture.DataCaching.DataCaches
{
    internal class SimpleDataCache : IDataCache
    {
        private readonly Dictionary<string, object> _objects;

        public SimpleDataCache()
        {
            _objects = new Dictionary<string, object>();
        }

        public void InsertOrUpdate(string key, object value)
        {
            if (_objects.ContainsKey(key))
                _objects[key] = value;
            else
                _objects.Add(key, value);
        }

        public object Get(string key)
        {
            return _objects.ContainsKey(key) ? _objects[key] : null;
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            return value is T ? (T) value : default(T);
        }

        public void Delete(string key)
        {
            if(!_objects.ContainsKey(key)) return;
            _objects.Remove(key);
        }
    }
}
