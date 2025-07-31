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
    /// Filters by project
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterByProject<T> : ProjectSpecification<T> where T : ProjectEntity
    {
        public long Id { get; private set; }

        public FilterByProject(long testProjectId)
        {
            Id = testProjectId;
        }

        protected override Expression<Func<T, bool>> GetExpression()
        {
            return x => x.TestProjectId == Id || x.TestProjectId == null;
        }
    }
}
