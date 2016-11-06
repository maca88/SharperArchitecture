using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;

namespace PowerArhitecture.Breeze.Specification
{
    public interface IBreezeInterceptor
    {
        void BeforeSave(List<EntityInfo> entitiesToPersist);

        Task BeforeSaveAsync(List<EntityInfo> entitiesToPersist);

        void AfterFlush(List<EntityInfo> entitiesToPersist);

        Task AfterFlushAsync(List<EntityInfo> entitiesToPersist);

        void BeforeFlush(List<EntityInfo> entitiesToPersist);

        Task BeforeFlushAsync(List<EntityInfo> entitiesToPersist);
    }
}
