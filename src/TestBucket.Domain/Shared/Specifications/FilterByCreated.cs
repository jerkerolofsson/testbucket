using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;

namespace TestBucket.Domain.Shared.Specifications
{
    /// <summary>
    /// Filters by creation date
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterByCreated<T> : ProjectSpecification<T> where T : ProjectEntity
    {
        private readonly DateTimeOffset _from;
        private readonly DateTimeOffset _until;

        public FilterByCreated(DateTimeOffset from, DateTimeOffset until)
        {
            _from = from;
            _until = until;
        }

        protected override Expression<Func<T, bool>> GetExpression()
        {
            return x => x.Created >= _from && x.Created <= _until;
        }
    }
}
