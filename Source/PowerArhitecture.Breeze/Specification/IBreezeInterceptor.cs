using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;

namespace PowerArhitecture.Breeze.Specification
{
    public interface IBreezeInterceptor<TType> : IBreezeInterceptor
    {

    }

    public interface IBreezeInterceptor
    {
        Task BeforeSave(List<EntityInfo> entitiesToPersist);

        Task AfterFlush(List<EntityInfo> entitiesToPersist);

        Task BeforeFlush(List<EntityInfo> entitiesToPersist);
    }
}
