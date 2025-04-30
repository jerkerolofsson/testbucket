
using System.Xml.Linq;

using OneOf;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Teams;
using TestBucket.Domain.Teams.Models;
using TestBucket.Domain.Tenants.Models;

namespace TestBucket.Data.Teams;
internal class TeamRepository : ITeamRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IProjectRepository _projectRepository;

    public TeamRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory, IProjectRepository projectRepository)
    {
        _dbContextFactory = dbContextFactory;
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Returns a team by slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<Team?> GetBySlugAsync(string tenantId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Teams.Where(x => x.Slug == slug && x.TenantId == tenantId).FirstOrDefaultAsync();
    }
    /// <summary>
    /// Returns a team by id
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Team?> GetTeamByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Teams.Where(x => x.Id == id && x.TenantId == tenantId).FirstOrDefaultAsync();
    }

    public async Task UpdateTeamAsync(Team team)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.Teams.Update(team);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PagedResult<Team>> SearchAsync(string tenantId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.Teams.Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.Text is not null)
        {
            tests = tests.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count);

        return new PagedResult<Team>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }


    /// <summary>
    /// Creates a new team
    /// </summary>
    /// <param name="name">Name of project</param>
    /// <returns></returns>
    public async Task<OneOf<Team, AlreadyExistsError>> AddAsync(Team team)
    {
        ArgumentNullException.ThrowIfNull(team.TenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(team.Name);

        team.Slug = await GenerateSlugAsync(team.TenantId, team.Name);
        team.ShortName = await GenerateShortNameAsync(team.Slug, team.TenantId);

        if (await NameExistsAsync(team.TenantId, team.Name))
        {
            return new AlreadyExistsError();
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Teams.AddAsync(team);
        await dbContext.SaveChangesAsync();

        return team;
    }

    /// <summary>
    /// Creates a new team
    /// </summary>
    /// <param name="name">Name of project</param>
    /// <returns></returns>
    public async Task<OneOf<Team, AlreadyExistsError>> CreateAsync(string tenantId, string name)
    {
        if (await NameExistsAsync(tenantId, name))
        {
            return new AlreadyExistsError();
        }
        var team = new Team()
        {
            Name = name,
            Slug = await GenerateSlugAsync(tenantId, name),
            TenantId = tenantId,
            ShortName = ""
        };

        team.ShortName = await GenerateShortNameAsync(team.Slug, tenantId);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Teams.AddAsync(team);
        await dbContext.SaveChangesAsync();

        return team;
    }

    public async Task<string> GenerateSlugAsync(string tenantId, string name)
    {
        var slugHelper = new Slugify.SlugHelper();
        var slug = slugHelper.GenerateSlug(name);
        int counter = 1;
        while (await SlugExistsAsync(tenantId, slug))
        {
            slug = slugHelper.GenerateSlug(slug + counter);
            counter++;
        }
        return slug;
    }
    public string GenerateSlug(string name)
    {
        return new Slugify.SlugHelper().GenerateSlug(name);
    }

    public async Task<string> GenerateShortNameAsync(string slug, string tenantId)
    {
        slug = slug.ToUpper();
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        // First generate a preferred one consting of the first character of each word
        var words = slug.Split('-', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
        if(words.Length>=2)
        {
            var shortName = new string(words.Select(x => x.First()).ToArray());
            var exists = await dbContext.Teams.AsNoTracking().Where(x => x.ShortName == shortName && x.TenantId == tenantId).AnyAsync();
            if (!exists)
            {
                return shortName;
            }
        }

        // Next generate a consisting of alteast 2 characters, increasing to avoid duplicates
        // There shouldn't be any duplicates for the worst case scenario as we know the slug is unique when calling
        // Note that UI may not be unique as it has not been saved yet.
        for (int len=2; len  < slug.Length; len++)
        {
            var shortName  = slug[0..len];
            var exists = await dbContext.Teams.AsNoTracking().Where(x => x.ShortName == shortName && x.TenantId == tenantId).AnyAsync();
            if(!exists)
            {
                return shortName;
            }
        }

        return slug;
    }

    /// <summary>
    /// Returns true if a project exists with the specified slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<bool> SlugExistsAsync(string tenantId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Teams.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
    }

    /// <summary>
    /// Returns true if a project exists with the specified name
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<bool> NameExistsAsync(string tenantId, string name)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var exists = await dbContext.Teams.AsNoTracking()
            .Where(x => x.Name == name && x.TenantId == tenantId).AnyAsync();

        if(exists)
        {
            var teams = await dbContext.Teams.AsNoTracking().ToListAsync();
        }

        return exists;
    }

    public async Task DeleteAsync(Team team)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        foreach(var project in dbContext.Projects.AsNoTracking().Where(x=>x.TeamId == team.Id))
        {
            await _projectRepository.DeleteProjectAsync(project);
        }

        dbContext.Teams.Remove(team);
        await dbContext.SaveChangesAsync();
    }
}
