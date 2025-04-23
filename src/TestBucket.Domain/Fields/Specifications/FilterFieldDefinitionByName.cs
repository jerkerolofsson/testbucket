using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;

namespace TestBucket.Domain.Fields.Specifications
{
    public class FilterFieldDefinitionByName : FilterSpecification<FieldDefinition>
    {
        private readonly string _name;

        public FilterFieldDefinitionByName(string name)
        {
            _name = name;
        }

        protected override Expression<Func<FieldDefinition, bool>> GetExpression()
        {
            return x => x.Name == _name;
        }
    }
}
