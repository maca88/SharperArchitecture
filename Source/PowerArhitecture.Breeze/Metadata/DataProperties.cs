using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerArhitecture.Breeze.Metadata
{
    public class DataProperties : MetadataList<DataProperty>
    {
        public DataProperties()
        {
        }

        public DataProperties(List<Dictionary<string, object>> listOfDict) : base(listOfDict)
        {
        }

        protected override DataProperty Convert(Dictionary<string, object> item)
        {
            return new DataProperty(item);
        }
    }
}
