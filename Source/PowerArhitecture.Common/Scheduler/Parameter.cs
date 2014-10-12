using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAF.Common.Scheduler
{
    public class Parameter
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public object Value { get; set; }

        public override bool Equals(object obj)
        {
            var parameter = obj as Parameter;
            if (parameter != null)
            {
                return Equals(parameter.Name, Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Name))
                return base.GetHashCode();
            unchecked
            {
                var result = 37;
                result *= 397;
                if (Name != null)
                    result += Name.GetHashCode();
                return result;
            }
        }
    }
}
