using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Shared.Specifications
{
    /// <summary>
    /// Base class for specification pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FilterSpecification<T>
    {
        public Expression<Func<T, bool>> Expression => GetExpression();

        protected abstract Expression<Func<T, bool>> GetExpression(); 

        public bool IsMatch(T obj)
        {
            var expression = GetExpression().Compile();
            return expression(obj);
        }
    }
}
