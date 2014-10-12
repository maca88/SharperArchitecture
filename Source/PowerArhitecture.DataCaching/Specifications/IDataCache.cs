namespace PowerArhitecture.DataCaching.Specifications
{
    public interface IDataCache
    {
        void InsertOrUpdate(string key, object value);

        object Get(string key);

        T Get<T>(string key);

        void Delete(string key);
    }
}
