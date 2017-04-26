using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.DataAccess.Entities.Versioning
{
    public class VersionWheelWithStringUser : VersionedEntityWithUser<string>
    {
        public virtual int Dimension { get; set; }

        public virtual VersionCarWithStringUser VersionCarWithStringUser { get; set; }
    }
}
