using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Requirements.Types;
public class RequirementTypeConverter
{
    private static readonly Dictionary<string, MappedRequirementType> _map = new Dictionary<string, MappedRequirementType>()
    {
        [RequirementTypes.General] = MappedRequirementType.General,
        [RequirementTypes.Regulatory] = MappedRequirementType.Regulatory,
        [RequirementTypes.Task] = MappedRequirementType.Task,
        [RequirementTypes.Initiative] = MappedRequirementType.Initiative,
        [RequirementTypes.Epic] = MappedRequirementType.Epic,
        [RequirementTypes.Story] = MappedRequirementType.Story,
        [RequirementTypes.Other] = MappedRequirementType.Other,
        [RequirementTypes.Standard] = MappedRequirementType.Standard,
    };

    public static MappedRequirementType GetMappedRequirementTypeFromString(string name)
    {
        if(_map.TryGetValue(name, out var result))
        {
            return result;
        }
        return MappedRequirementType.Other;   
    }
}
