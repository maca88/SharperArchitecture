using System;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.DataAccess.Specifications;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping;

namespace PowerArhitecture.DataAccess
{
    public class SessionFactoryInfo
    {
        public SessionFactoryInfo(ISessionFactory sessionFactory, Configuration configuration) : this()
        {
            SessionFactory = sessionFactory;
            Configuration = configuration;
            Initialize();
        }

        public SessionFactoryInfo()
        {
            OneToOneWithoutLazyLoading = new Dictionary<string, List<string>>();
        }

        public ISessionFactory SessionFactory { get; private set; }

        public Configuration Configuration { get; private set; }

        public Dictionary<string, List<string>> OneToOneWithoutLazyLoading { get; private set; } 

        public bool IsLazyLoadEnabled(Type type, string propertyName)
        {
            return !OneToOneWithoutLazyLoading.ContainsKey(type.FullName) ||
                   !OneToOneWithoutLazyLoading[type.FullName].Contains(propertyName);
        }

        public void ValidateSettings(IDatabaseSettings settings)
        {
            if (!settings.AllowOneToOneWithoutLazyLoading && OneToOneWithoutLazyLoading.Any())
                throw new HibernateException(
                    "One to one relation without lazy loading is not permitted. " +
                    "Lazy loading is disabled for: " +
                    string.Join(",\r\n", OneToOneWithoutLazyLoading.Select(o => String.Format("Type: {0} Properties: {1}", 
                        o.Key, string.Join(", ", o.Value)))) +
                    "\r\nHint: Use Constrained one to one or allow one to one without lazy loading in config");
        }

        private void Initialize()
        {
            foreach (var cls in Configuration.ClassMappings)
            {
                foreach (var prop in cls.PropertyIterator)
                {
                    var oneToOne = prop.Value as OneToOne;
                    if (prop.Value is OneToOne)
                    {
                        Process(cls, oneToOne);
                        continue;
                    }
                    var oneToMany = prop.Value as OneToMany;
                    if (oneToMany != null)
                    {
                        Process(cls, oneToMany);
                        continue;
                    }
                }
            }
        }

        private void Process(PersistentClass persistentClass, OneToOne oneToOne)
        {
            var clsFullName = persistentClass.MappedClass.FullName;
            if (clsFullName == null) return;
            if (!oneToOne.IsConstrained) //Lazy load is disabled
            {
                if (!OneToOneWithoutLazyLoading.ContainsKey(clsFullName))
                    OneToOneWithoutLazyLoading.Add(clsFullName, new List<string>());
                OneToOneWithoutLazyLoading[clsFullName].Add(oneToOne.PropertyName);
            }
        }

        private void Process(PersistentClass persistentClass, OneToMany oneToOne)
        {

        }
    }
}
