using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Breeze.Metadata
{
    public class FkMap : MetadataDictionary<string>
    {
        public FkMap(Dictionary<string, string> dict)
            : base(dict)
        {
        }
    }
}
