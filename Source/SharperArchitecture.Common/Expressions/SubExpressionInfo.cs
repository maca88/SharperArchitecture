using System;

namespace SharperArchitecture.Common.Expressions
{
    public class SubExpressionInfo
    {
        public string Path { get; set; }

        public Type MemberType { get; set; }

        public string MemberName { get; set; }

        public System.Linq.Expressions.Expression Expression { get; set; }
    }
}
