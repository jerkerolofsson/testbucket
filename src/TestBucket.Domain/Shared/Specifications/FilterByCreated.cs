using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;
using TestBucket.Domain.Requirements.Models;

namespace TestBucket.Domain.Shared.Specifications
{
    /// <summary>
    /// Filters by creation date
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterByCreated<T> : ProjectSpecification<T> where T : ProjectEntity
    {
        private readonly DateTimeOffset? _from;
        private readonly DateTimeOffset? _until;
        public FilterByCreated(DateTimeOffset? from, DateTimeOffset? until)
        {
            _from = from;
            _until = until;
            if (_from != null)
            {
                _from = _from.Value.ToUniversalTime();
            }
            if (_until != null)
            {
                _until = _until.Value.ToUniversalTime();
            }
        }

        protected override Expression<Func<T, bool>> GetExpression()
        {
            if (_from is null && _until is not null)
            {
                return x => x.Created <= _until;
            }
            if (_from is not null && _until is null)
            {
                return x => x.Created >= _from;
            }
            return x => x.Created >= _from && x.Created <= _until;
        }
    }
}
