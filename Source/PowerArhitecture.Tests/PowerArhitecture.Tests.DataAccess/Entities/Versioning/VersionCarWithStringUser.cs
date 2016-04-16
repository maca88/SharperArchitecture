using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Tests.DataAccess.Entities.Versioning
{
    public class VersionCarWithStringUser : VersionedEntityWithUser<string>
    {
        public VersionCarWithStringUser()
        {
            Wheels = new HashSet<VersionWheelWithStringUser>();
        }

        public virtual string Model { get; set; }

        public virtual VersionCarWithStringUser Child { get; set; }

        public virtual ISet<VersionWheelWithStringUser> Wheels { get; set; }

        public virtual void AddWheel(VersionWheelWithStringUser wheel)
        {
            wheel.VersionCarWithStringUser = this;
            Wheels.Add(wheel);
        }
    }
}
