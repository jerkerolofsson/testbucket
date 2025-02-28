


using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

using OneOf;
using TestBucket.Domain.Errors;
using TestBucket.Domain.Projects;
using TestBucket.Domain.Projects.Models;

namespace TestBucket.Data.Testing;
internal class ProjectRepository : IProjectRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ProjectRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Returns a project by slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<TestProject?> GetBySlugAsync(string tenantId, string slug)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Projects.Where(x => x.Slug == slug && x.TenantId == tenantId).SingleOrDefaultAsync();
    }


    public async Task<PagedResult<TestProject>> SearchAsync(string tenantId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var tests = dbContext.Projects.Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.Text is not null)
        {
            tests = tests.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await tests.LongCountAsync();
        var items = tests.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count);

        return new PagedResult<TestProject>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }

    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="name">Name of project</param>
    /// <returns></returns>
    public async Task<OneOf<TestProject, AlreadyExistsError>> CreateAsync(string tenantId, string name)
    {
        var project = new TestProject()
        {
            Name = name,
            Slug = GenerateSlug(name),
            TenantId = tenantId,
            ShortName = "",
        };

        if (await SlugExistsAsync(tenantId, project.Slug))
        {
            return new AlreadyExistsError();
        }

        project.ShortName = await GenerateShortNameAsync(project.Slug, tenantId);

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        return project;
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
            var exists = await dbContext.Projects.AsNoTracking().Where(x => x.ShortName == shortName && x.TenantId == tenantId).AnyAsync();
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
            var exists = await dbContext.Projects.AsNoTracking().Where(x => x.ShortName == shortName && x.TenantId == tenantId).AnyAsync();
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
        return await dbContext.Projects.AsNoTracking().Where(x => x.Slug == slug && x.TenantId == tenantId).AnyAsync();
    }

    /// <summary>
    /// Returns true if a project exists with the specified name
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public async Task<bool> NameExistsAsync(string tenantId, string name)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Projects.AsNoTracking().Where(x => x.Name == name && x.TenantId == tenantId).AnyAsync();
    }
}
