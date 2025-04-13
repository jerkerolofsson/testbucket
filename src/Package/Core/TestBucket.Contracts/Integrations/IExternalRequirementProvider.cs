using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Requirements;

namespace TestBucket.Contracts.Integrations;

/// <summary>
/// Implemented by an extension
/// </summary>
public interface IExternalRequirementProvider
{
    string SystemName { get; }

    public async IAsyncEnumerable<RequirementSpecificationDto> EnumSpecificationsAsync(ExternalSystemDto config, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int offset = 0;
        int count = 50;
        while(!cancellationToken.IsCancellationRequested)
        {
            var response = await BrowseSpecificationsAsync(config, offset, count, cancellationToken);
            if(response.TotalCount == 0)
            {
                break;
            }
            offset += response.Items.Length;
            foreach(var item in response.Items)
            {
                yield return item;
            }
        }
    }

    public async IAsyncEnumerable<RequirementEntityDto> EnumRequirementsAsync(ExternalSystemDto config, RequirementSpecificationDto specification, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int offset = 0;
        int count = 50;
        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await BrowseSpecificationAsync(config, specification, offset, count, cancellationToken);
            if (response.TotalCount == 0)
            {
                break;
            }
            offset += response.Items.Length;
            foreach (var item in response.Items)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Returns specifications
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<RequirementSpecificationDto>> BrowseSpecificationsAsync(ExternalSystemDto config, int offset, int count, CancellationToken cancellationToken);


    /// <summary>
    /// Browses for requirements/folders
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<PagedResult<RequirementEntityDto>> BrowseSpecificationAsync(ExternalSystemDto config, RequirementSpecificationDto specification, int offset, int count, CancellationToken cancellationToken);
}
