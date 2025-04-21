using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestResources
{
    /// <summary>
    /// Merges two or more lists of dependencies into a single dependency
    /// </summary>
    public class TestCaseDependencyMerger
    {
        public static List<TestCaseDependency> Merge(params IEnumerable<List<TestCaseDependency>> dependencies)
        {
            // Count how many resources of each type is required
            var requiredResourceTypesAndCounts = new Dictionary<string, int>();
            foreach(var list in dependencies)
            {
                var types = list.Where(x => x.ResourceType is not null).Select(x => x.ResourceType!).ToArray();

                foreach (var requiredResourceType in types)
                {
                    if(!requiredResourceTypesAndCounts.ContainsKey(requiredResourceType))
                    {
                        requiredResourceTypesAndCounts.Add(requiredResourceType, types.Length);
                    }
                    else
                    {
                        var currentCount = requiredResourceTypesAndCounts[requiredResourceType];
                        requiredResourceTypesAndCounts[requiredResourceType] = Math.Max(types.Length, currentCount);
                    }
                }   
            }

            // Create output
            var result = new List<TestCaseDependency>();
            foreach (var requiredResourceType in requiredResourceTypesAndCounts.Keys)
            {
                for(int i=0; i< requiredResourceTypesAndCounts[requiredResourceType]; i++)
                {
                    var mergedDependency = new TestCaseDependency { ResourceType = requiredResourceType };

                    // Merge required attributes from each depdendency
                    foreach(var list in dependencies)
                    {
                        TestCaseDependency? source = list.Where(x => x.ResourceType == requiredResourceType).Skip(i).FirstOrDefault();
                        if(source is not null)
                        {
                            // Copy the attributes of this dependency to the merged dependency
                            Merge(mergedDependency, source);
                        }
                    }

                    result.Add(mergedDependency);
                }
            }

            return result;
        }

        private static void Merge(TestCaseDependency destination, TestCaseDependency source)
        {
            if (source.AttributeRequirements is not null)
            {
                destination.AttributeRequirements ??= [];
                destination.AttributeRequirements.AddRange(source.AttributeRequirements);
            }
        }
    }
}
