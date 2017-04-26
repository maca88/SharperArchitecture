using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharperArchitecture.Domain;

namespace SharperArchitecture.Tests.WebApi.Server.Entities
{
    public class TestEntity : Entity
    {
        public virtual string Name { get; set; }
    }
}