using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.DataAccess.Entities.Versioning
{
    public class VersionCarWithEntityUser : VersionedEntityWithUser<User>
    {
        public VersionCarWithEntityUser()
        {
            Wheels = new HashSet<VersionWheelWithEntityUser>();
        }

        public virtual string Model { get; set; }

        public virtual VersionCarWithEntityUser Child { get; set; }

        public virtual ISet<VersionWheelWithEntityUser> Wheels { get; set; }

        public virtual void AddWheel(VersionWheelWithEntityUser wheel)
        {
            wheel.VersionCarWithEntityUser = this;
            Wheels.Add(wheel);
        }
    }
}
