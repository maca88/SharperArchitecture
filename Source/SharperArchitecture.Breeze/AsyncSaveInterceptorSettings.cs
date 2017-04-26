using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;

namespace SharperArchitecture.Breeze
{
    public class AsyncSaveInterceptorSettings
    {
        public Func<List<EntityInfo>, Task> BeforeSave { get; set; }

        public Action<Dictionary<Type, List<EntityInfo>>, List<KeyMapping>> AfterSave { get; set; }

        public Func<List<EntityInfo>, Task> AfterFlush { get; set; }

        public Func<List<EntityInfo>, Task> BeforeFlush { get; set; }
    }
}
