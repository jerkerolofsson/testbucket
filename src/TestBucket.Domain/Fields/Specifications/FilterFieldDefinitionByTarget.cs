using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Fields;
using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Fields.Specifications
{
    public class FilterFieldDefinitionByTarget : FilterSpecification<FieldDefinition>
    {
        private readonly FieldTarget _target;

        public FilterFieldDefinitionByTarget(FieldTarget target)
        {
            _target = target;
        }

        protected override Expression<Func<FieldDefinition, bool>> GetExpression()
        {
            return x => (x.Target&_target) != 0;
        }
    }
}
