using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Tests.DataAccess.Entities.Versioning
{
    public class VersionCar : VersionedEntity
    {
        public virtual string Model { get; set; }

        public virtual VersionCar Child { get; set; }

        private ISet<VersionWheel> _wheels;

        public virtual ISet<VersionWheel> Wheels { 
            get { return _wheels ?? (_wheels = new HashSet<VersionWheel>()); } 
            set {  _wheels = value; }
        }

        public virtual void AddWheel(VersionWheel wheel)
        {
            wheel.VersionCar = this;
            Wheels.Add(wheel);
        }
    }
}
