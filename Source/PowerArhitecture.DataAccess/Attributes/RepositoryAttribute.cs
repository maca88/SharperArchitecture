using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.DataAccess.Attributes
{
    public class RepositoryAttribute : Attribute
    {
        public RepositoryAttribute()
        {
            AutoBind = true;
        }

        /// <summary>
        /// Default is true
        /// </summary>
        public bool AutoBind { get; set; }
    }
}
