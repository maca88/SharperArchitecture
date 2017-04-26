using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.DataAccess.MultiDatabase.BazModels
{
    public class BazModel : Entity
    {
        public virtual string Name { get; set; }
    }
}
