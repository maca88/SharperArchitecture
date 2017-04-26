using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Specialized
{
    public static class NameValueCollectionExtensions
    {
        private static readonly PropertyInfo IsReadOnlyProp;

        static NameValueCollectionExtensions()
        {
            IsReadOnlyProp = typeof(NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static void SetReadOnly(this NameValueCollection coll, bool value)
        {
            IsReadOnlyProp.SetValue(coll, value);
        }
    }
}
