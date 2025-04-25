
using System.Diagnostics;
using System.Xml.Linq;

using OneOf;

using TestBucket.Domain.Errors;
using TestBucket.Domain.Projects;

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
        return await dbContext.Projects
            .Include(x => x.Team)
            .Include(x => x.ExternalSystems)
            .Where(x => x.Slug == slug && x.TenantId == tenantId).SingleOrDefaultAsync();
    }
    /// <summary>
    /// Returns a project by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<TestProject?> GetProjectByIdAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Projects.Where(x => x.Id == id && x.TenantId == tenantId)
            .Include(x => x.Team)
            .Include(x=>x.ExternalSystems)
            .SingleOrDefaultAsync();
    }

    public async Task UpdateProjectAsync(TestProject testProject)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.Projects.Update(testProject);
        await dbContext.SaveChangesAsync();
    }


    public async Task<PagedResult<TestProject>> SearchAsync(string tenantId, SearchQuery query)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var projects = dbContext.Projects
            .Include(x => x.Team)
            .Include(x => x.ExternalSystems)
            .Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.TeamId is not null)
        {
            projects = projects.Where(x => x.TeamId == query.TeamId);
        }
        if (query.Text is not null)
        {
            projects = projects.Where(x => x.Name.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await projects.LongCountAsync();
        var items = projects.OrderBy(x => x.Name).Skip(query.Offset).Take(query.Count);

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
    public async Task<OneOf<TestProject, AlreadyExistsError>> AddAsync(TestProject project)
    {
        ArgumentNullException.ThrowIfNull(project.TenantId);

        if (string.IsNullOrEmpty(project.Slug))
        {
            project.Slug = GenerateSlug(project.Name);
        }
        if (string.IsNullOrEmpty(project.ShortName))
        {
            project.ShortName = await GenerateShortNameAsync(project.Slug, project.Name);
        }

        if (await SlugExistsAsync(project.TenantId, project.Slug))
        {
            return new AlreadyExistsError();
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        var createdProject = await GetProjectByIdAsync(project.TenantId, project.Id);
        Debug.Assert(createdProject is not null);
        return createdProject;
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

    public async Task AddProjectIntegrationsAsync(string tenantId, string slug, ExternalSystem system)
    {
        var project = await GetBySlugAsync(tenantId, slug);
        if(project is null)
        {
            throw new InvalidOperationException("Project not found: " + slug);
        }
        system.TestProjectId = project.Id;

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        //await dbContext.ExternalSystems.AsNoTracking().Where(x => x.TenantId == tenantId && x.TestProjectId == project.Id && x.Name == system.Name).ExecuteDeleteAsync();
        await dbContext.ExternalSystems.AddAsync(system);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, long projectId)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ExternalSystems.Where(x => x.TenantId == tenantId && x.TestProjectId == projectId).ToListAsync();
    }
    public async Task<IReadOnlyList<ExternalSystem>> GetProjectIntegrationsAsync(string tenantId, string slug)
    {
        var project = await GetBySlugAsync(tenantId, slug);
        if (project is null)
        {
            throw new InvalidOperationException("Project not found: " + slug);
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.ExternalSystems
            .Where(x => x.TenantId == tenantId && x.TestProjectId == project.Id)
            .OrderBy(x=> x.Name).ToListAsync();
    }

    public async Task UpdateProjectIntegrationAsync(string tenantId, string slug, ExternalSystem system)
    {
        var project = await GetBySlugAsync(tenantId, slug);
        if (project is null)
        {
            throw new InvalidOperationException("Project not found: " + slug);
        }

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.ExternalSystems.Update(system);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteProjectIntegrationAsync(string tenantId, long id)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.ExternalSystems.Where(x=>x.TenantId == tenantId && x.Id == id).ExecuteDeleteAsync();
    }

    public async Task DeleteProjectAsync(TestProject project)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        foreach(var fieldDefinition in dbContext.FieldDefinitions.Where(x=>x.TestProjectId == project.Id))
        {
            foreach (var fieldValue in dbContext.TestRunFields.Where(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                dbContext.TestRunFields.Remove(fieldValue);
            }
            foreach (var fieldValue in dbContext.TestCaseRunFields.Where(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                dbContext.TestCaseRunFields.Remove(fieldValue);
            }
            foreach (var fieldValue in dbContext.RequirementFields.Where(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                dbContext.RequirementFields.Remove(fieldValue);
            }
            foreach (var fieldValue in dbContext.TestCaseFields.Where(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                dbContext.TestCaseFields.Remove(fieldValue);
            }

            dbContext.FieldDefinitions.Remove(fieldDefinition);
        }

        foreach (var item in dbContext.TestCaseRuns.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestCaseRuns.Remove(item);
        }
        foreach (var item in dbContext.TestRuns.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestRuns.Remove(item);
        }
        foreach (var testCase in dbContext.TestCases.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestCases.Remove(testCase);
        }
        foreach (var item in dbContext.TestSuiteFolders.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestSuiteFolders.Remove(item);
        }
        foreach (var item in dbContext.TestSuites.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestSuites.Remove(item);
        }

        foreach (var item in dbContext.Requirements.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Requirements.Remove(item);
        }
        foreach (var item in dbContext.RequirementSpecificationFolders.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.RequirementSpecificationFolders.Remove(item);
        }
        foreach (var item in dbContext.RequirementSpecifications.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.RequirementSpecifications.Remove(item);
        }

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();
    }
}
