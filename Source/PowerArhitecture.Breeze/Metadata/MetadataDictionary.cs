using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Breeze.Metadata
{
    public class MetadataDictionary : IDictionary<string, object>, IDictionary
    {
        public MetadataDictionary()
        {
            OriginalDictionary = new Dictionary<string, object>();
        }

        public MetadataDictionary(Dictionary<string, object> dict)
        {
            OriginalDictionary = dict;
        }

        public Dictionary<string, object> OriginalDictionary { get; private set; }

        public object this[string key]
        {
            get { return OriginalDictionary[key]; }
            set { OriginalDictionary[key] = value; }
        }

        #region Interface implementations

        void IDictionary.Clear()
        {
            OriginalDictionary.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return OriginalDictionary.GetEnumerator();
        }

        public void Remove(object key)
        {
            ((IDictionary)OriginalDictionary).Remove(key);
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary)OriginalDictionary)[key]; }
            set { ((IDictionary) OriginalDictionary)[key] = value; }
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return OriginalDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return OriginalDictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)OriginalDictionary).Add(item);
        }

        public bool Contains(object key)
        {
            return ((IDictionary) OriginalDictionary).Contains(key);
        }

        public void Add(object key, object value)
        {
            ((IDictionary)OriginalDictionary).Add(key, value);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            ((ICollection<KeyValuePair<string, object>>)OriginalDictionary).Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
           return ((ICollection<KeyValuePair<string, object>>) OriginalDictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>) OriginalDictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>) OriginalDictionary).Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection) OriginalDictionary).CopyTo(array, index);
        }

        int ICollection.Count { get { return OriginalDictionary.Count; } }
        public object SyncRoot { get { return ((ICollection) OriginalDictionary).SyncRoot; } }
        public bool IsSynchronized { get { return ((ICollection)OriginalDictionary).IsSynchronized; } }
        int ICollection<KeyValuePair<string, object>>.Count 
        { 
            get
            {
                return ((ICollection<KeyValuePair<string, object>>) OriginalDictionary).Count;
            } 
        }
        ICollection IDictionary.Values { get { return ((IDictionary) OriginalDictionary).Values; } }
        bool IDictionary.IsReadOnly { get { return ((IDictionary)OriginalDictionary).IsReadOnly; } }
        public bool IsFixedSize { get { return ((IDictionary)OriginalDictionary).IsFixedSize; } }
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly { get { return ((IDictionary)OriginalDictionary).IsReadOnly; } }

        public bool ContainsKey(string key)
        {
            return OriginalDictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            OriginalDictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return OriginalDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return OriginalDictionary.TryGetValue(key, out value);
        }

        object IDictionary<string, object>.this[string key]
        {
            get { return OriginalDictionary[key]; }
            set { OriginalDictionary[key] = value; }
        }

        ICollection<string> IDictionary<string, object>.Keys { get { return OriginalDictionary.Keys; } }
        ICollection IDictionary.Keys { get { return ((IDictionary) OriginalDictionary).Keys; } }
        ICollection<object> IDictionary<string, object>.Values { get { return OriginalDictionary.Values; } }

        #endregion
    }
}
