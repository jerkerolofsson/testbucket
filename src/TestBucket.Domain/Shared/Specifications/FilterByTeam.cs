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
    /// Filters by team
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterByTeam<T> : ProjectSpecification<T> where T : ProjectEntity
    {
        private readonly long _teamId;

        public FilterByTeam(long teamId)
        {
            _teamId = teamId;
        }

        protected override Expression<Func<T, bool>> GetExpression()
        {
            return x => x.TeamId == _teamId || x.TeamId == null;
        }
    }
}
