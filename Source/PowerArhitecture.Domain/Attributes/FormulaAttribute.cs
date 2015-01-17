using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Domain.Attributes
{
    public class FormulaAttribute : Attribute
    {
        public FormulaAttribute(string formula)
        {
            SqlFormula = formula;
            if (!SqlFormula.StartsWith("(") && !SqlFormula.EndsWith(")"))
                SqlFormula = string.Format("({0})", SqlFormula);
        }

        public string SqlFormula { get; private set; }
    }
}
