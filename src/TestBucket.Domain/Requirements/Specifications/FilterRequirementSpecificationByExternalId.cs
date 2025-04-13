using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Requirements.Specifications
{
    public class FilterRequirementSpecificationByExternalId : FilterSpecification<RequirementSpecification>
    {
        private readonly string _externalProvider;
        private readonly string _externalId;

        public FilterRequirementSpecificationByExternalId(string externalProvider, string externalId)
        {
            _externalProvider = externalProvider;
            _externalId = externalId;
        }

        protected override Expression<Func<RequirementSpecification, bool>> GetExpression()
        {
            return x => x.ExternalProvider == _externalProvider && x.ExternalId == _externalId;
        }
    }
}
