using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Teams;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Teams.Mapping;
public static class TeamMapping
{
    public static Team ToDbo(this TeamDto item)
    {
        return new Team { Name = item.Name, ShortName = item.ShortName, Slug = item.Slug };
    }
    public static TeamDto ToDto(this Team item)
    {
        return new TeamDto { Name = item.Name, ShortName = item.ShortName, Slug = item.Slug };
    }
}
