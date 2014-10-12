using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Notifications.Specifications;
using Ninject;
using Ninject.Syntax;

namespace PowerArhitecture.Notifications.Managers
{
    public static class RecipientSearchQueryManager
    {
        private static readonly Dictionary<string, Type> RecipientSearchQueries = new Dictionary<string, Type>();

        static RecipientSearchQueryManager()
        {
        }

        public static void RegisterSearchQueryManager<T>(this IBindingRoot bindingRoot, string name) where T : IRecipientSearchQuery
        {
            bindingRoot.Bind<IRecipientSearchQuery>().To<T>().Named(name);
            RecipientSearchQueries[name] = typeof (T);
        }

        public static void RegisterSearchQueryManager(this IBindingRoot bindingRoot, Type type, string name)
        {
            bindingRoot.Bind<IRecipientSearchQuery>().To(type).Named(name);
            RecipientSearchQueries[name] = type;
        }

        public static bool ContainsQueryName(string name)
        {
            return RecipientSearchQueries.ContainsKey(name);
        }

        public static IEnumerable<string> GetAllQueryNames()
        {
            return RecipientSearchQueries.Keys;
        }
    }
}
