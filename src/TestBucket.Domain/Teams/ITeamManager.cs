using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Teams;
using TestBucket.Domain.Teams.Models;

namespace TestBucket.Domain.Teams;
public interface ITeamManager
{
    Task<OneOf<Team, AlreadyExistsError>> AddAsync(ClaimsPrincipal user, Team team);
    Task DeleteAsync(ClaimsPrincipal user, Team team);
    Task<Team?> GetTeamBySlugAsync(ClaimsPrincipal principal, string slug);
    Task<Team?> GetTeamByIdAsync(ClaimsPrincipal principal, long id);
}
