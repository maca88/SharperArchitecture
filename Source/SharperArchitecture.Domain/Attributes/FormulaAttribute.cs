using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Domain.Attributes
{
    public class FormulaAttribute : Attribute
    {
        public FormulaAttribute(string formula)
        {
            SqlFormula = formula;
            if (!SqlFormula.StartsWith("(") && !SqlFormula.EndsWith(")"))
                SqlFormula = $"({SqlFormula})";
        }

        public string SqlFormula { get; }
    }
}
