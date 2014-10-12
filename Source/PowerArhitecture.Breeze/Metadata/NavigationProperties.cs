using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerArhitecture.Breeze.Metadata
{
    public class NavigationProperties : MetadataList<NavigationProperty>
    {
        public NavigationProperties()
        {
        }

        public NavigationProperties(List<Dictionary<string, object>> listOfDict) : base(listOfDict)
        {
        }

        protected override NavigationProperty Convert(Dictionary<string, object> item)
        {
            return new NavigationProperty(item);
        }
    }
}
