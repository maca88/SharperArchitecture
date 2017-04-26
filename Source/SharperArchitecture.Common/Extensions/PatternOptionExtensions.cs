using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Enums;

namespace SharperArchitecture.Common.Extensions
{
    public static class PatternOptionExtensions
    {
        public static string ProcessPattern(this PatternOptions opt, string pattern)
        {
            if (opt.HasFlag(PatternOptions.Contains))
                return string.Format("*{0}*", pattern);
            if(opt.HasFlag(PatternOptions.StartsWith))
                pattern = string.Format("*{0}", pattern);
            if (opt.HasFlag(PatternOptions.EndsWith))
                pattern = string.Format("{0}*", pattern);
            return pattern;
        }
    }
}
