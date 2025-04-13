using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts;
using TestBucket.Contracts.Integrations;
using TestBucket.Contracts.Requirements;

using TrelloDotNet.Model.Options.GetCardOptions;

namespace TestBucket.Trello;
internal class TrelloRequirementProvider : IExternalRequirementProvider
{
    public string SystemName => ExtensionConstants.SystemName;

    public async Task<PagedResult<RequirementEntityDto>> BrowseSpecificationAsync(ExternalSystemDto config, RequirementSpecificationDto specification, int offset, int count, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(config.ApiKey);
        ArgumentNullException.ThrowIfNull(config.AccessToken);

        var client = new TrelloDotNet.TrelloClient(config.ApiKey, config.AccessToken);

        var cards = await client.GetCardsOnBoardAsync(specification.ExternalId, new GetCardOptions
        {
            IncludeList = true
        }, cancellationToken);


        List<RequirementEntityDto> reqirements = [];
        foreach (var card in cards.Skip(offset).Take(count))
        {
            reqirements.Add(new RequirementDto
            {
                Name = card.Name,
                ExternalId = card.Id,
                Provider = SystemName,
                ReadOnly = true,
                Description = card.Description,
                State = card.List?.Name,
                Path = card.List?.Name
            });
        }

        return new PagedResult<RequirementEntityDto>() { Items = reqirements.ToArray(), TotalCount = reqirements.Count };
    }

    public async Task<PagedResult<RequirementSpecificationDto>> BrowseSpecificationsAsync(ExternalSystemDto config, int offset, int count, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(config.ApiKey);
        ArgumentNullException.ThrowIfNull(config.AccessToken);

        var client = new TrelloDotNet.TrelloClient(config.ApiKey, config.AccessToken);

        var boards = await client.GetBoardsCurrentTokenCanAccessAsync(cancellationToken);


        List<RequirementSpecificationDto> specs = [];
        foreach(var board in boards.Skip(offset).Take(count))
        {
            specs.Add(new RequirementSpecificationDto
            { 
                Name = board.Name ,
                ExternalId = board.Id,
                Provider = SystemName,
                ReadOnly = true,
                Description = board.Description,
                Icon = ExtensionConstants.Icon,
            });
        }

        return new PagedResult<RequirementSpecificationDto>() { Items = specs.ToArray(), TotalCount = boards.Count };
    }
}
