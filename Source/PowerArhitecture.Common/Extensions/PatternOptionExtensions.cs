using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Enums;

namespace PowerArhitecture.Common.Extensions
{
    public static class PatternOptionExtensions
    {
        public static string ProcessPattern(this PatternOption opt, string pattern)
        {
            if (opt.HasFlag(PatternOption.Contains))
                return string.Format("*{0}*", pattern);
            if(opt.HasFlag(PatternOption.StartsWith))
                pattern = string.Format("*{0}", pattern);
            if (opt.HasFlag(PatternOption.EndsWith))
                pattern = string.Format("{0}*", pattern);
            return pattern;
        }
    }
}
