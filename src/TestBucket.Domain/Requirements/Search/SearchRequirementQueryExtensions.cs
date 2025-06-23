using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Search;

namespace TestBucket.Domain.Requirements.Search
{
    public static class SearchRequirementQueryExtensions
    {
        public static string ToSearchText(this SearchRequirementQuery query)
        {
            List<string> items = [];

            if (query.RequirementType is not null)
            {
                items.Add($"is:{query.RequirementType}");
            }
            if (query.RequirementState is not null)
            {
                items.Add($"state:{query.RequirementState}");
            }

            BaseQueryParser.Serialize(query, items);

            foreach (var field in query.Fields)
            {
                var name = field.Name.ToLower();
                var value = field.GetValueAsString();
                if (value.Contains(' '))
                {
                    value = $"\"{value}\"";
                }
                if (name.Contains(' '))
                {
                    name = $"\"{name}\"";
                }
                items.Add($"{name}:{value}");
            }

            return (string.Join(' ', items) + " " + query.Text).Trim();
        }
    }
}
