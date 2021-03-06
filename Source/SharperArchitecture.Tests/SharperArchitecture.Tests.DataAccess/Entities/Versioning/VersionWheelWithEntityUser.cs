﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.DataAccess.Entities.Versioning
{
    public class VersionWheelWithEntityUser : VersionedEntityWithUser<User>
    {
        public virtual int Dimension { get; set; }

        public virtual VersionCarWithEntityUser VersionCarWithEntityUser { get; set; }
    }
}
