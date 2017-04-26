using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using SharperArchitecture.Domain;

namespace SharperArchitecture.DataAccess.Interceptors
{
    public class NhibernateInterceptor : EmptyInterceptor
    {
        public override bool? IsTransient(object obj)
        {
            var entity = obj as IEntity;
            if (entity == null) return null;
            return entity.IsTransient();
        }
    }
}
