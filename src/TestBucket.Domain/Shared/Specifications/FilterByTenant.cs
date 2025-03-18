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
    /// Filters by tenant ID
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterByTenant<T> : FilterSpecification<T> where T : Entity
    {
        private readonly string _tenantId;

        public FilterByTenant(string tenantId)
        {
            _tenantId = tenantId;
        }

        protected override Expression<Func<T, bool>> GetExpression()
        {
            return x => x.TenantId == _tenantId;
        }
    }
}
