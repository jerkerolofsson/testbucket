using System.Linq.Expressions;

namespace TestBucket.Domain.Shared.Specifications
{
    /// <summary>
    /// Filters by tenant ID
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterByTenant<T> : FilterSpecification<T> where T : Entity
    {
        internal string TenantId { get; private set; }

        public FilterByTenant(string tenantId)
        {
            TenantId = tenantId;
        }

        protected override Expression<Func<T, bool>> GetExpression()
        {
            return x => x.TenantId == TenantId;
        }
    }
}
