using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace PowerArhitecture.Validation.Extensions
{
    public static class IRuleBuilderExtensions
    {
        /*
        public static IRuleBuilderOptions<T, TElement> MustBeUnique<T, TElement, TEntity>(this IRuleBuilder<T, TElement> ruleBuilder,
            Expression<Func<TEntity, object>> uniqueProp)
            where TEntity : class
            where TElement : IEnumerable<TEntity> 
        {
            return ruleBuilder.SetValidator(new UniqueListValidator<TEntity>(uniqueProp));
        }*/
    }
}
