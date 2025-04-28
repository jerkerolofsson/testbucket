using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Octokit;

using TestBucket.Contracts.Code.Models;

namespace TestBucket.Github.Mapping;
internal static class RepositoryMapper
{
    internal static RepositoryDto ToDto(this Repository repo)
    {
        return new RepositoryDto
        {
            Url = repo.Url,
            ExternalId = repo.Id.ToString()
        };
    }
}
