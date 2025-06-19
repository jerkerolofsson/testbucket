
using System.Diagnostics;
using System.Xml.Linq;

using OneOf;

using TestBucket.Domain.Errors;
using TestBucket.Domain.Fields.Models;
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
            .Where(x => x.Slug == slug && x.TenantId == tenantId).FirstOrDefaultAsync();
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
        ArgumentException.ThrowIfNullOrWhiteSpace(project.Name);

        if (await NameExistsAsync(project.TenantId, project.Name))
        {
            return new AlreadyExistsError();
        }

        project.Slug = await GenerateSlugAsync(project.TenantId, project.Name);
        project.ShortName = await GenerateShortNameAsync(project.Slug, project.Name);
       
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
    /// Generates a unique slug
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
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

        // Code
       
        foreach (var commit in dbContext.Commits.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Commits.Remove(commit);
        }
        foreach (var layer in dbContext.ArchitecturalLayers.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.ArchitecturalLayers.Remove(layer);
        }
        foreach (var system in dbContext.ProductSystems.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.ProductSystems.Remove(system);
        }
        foreach (var component in dbContext.Components.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Components.Remove(component);
        }
        foreach (var feature in dbContext.Features.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Features.Remove(feature);
        }
        foreach (var heuristic in dbContext.Heuristics.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Heuristics.Remove(heuristic);
        }
        // Fields

        foreach (var fieldDefinition in dbContext.FieldDefinitions.Where(x=>x.TestProjectId == project.Id).ToList())
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
            foreach (var fieldValue in dbContext.IssueFields.Where(x => x.FieldDefinitionId == fieldDefinition.Id))
            {
                dbContext.IssueFields.Remove(fieldValue);
            }

            dbContext.FieldDefinitions.Remove(fieldDefinition);
        }

        // Comments
        foreach (var item in dbContext.Comments.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Comments.Remove(item);
        }

        // Issues
        foreach (var item in dbContext.LinkedIssues.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.LinkedIssues.Remove(item);
        }
        foreach (var item in dbContext.LocalIssues.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.LocalIssues.Remove(item);
        }
        foreach (var milestone in dbContext.Milestones.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Milestones.Remove(milestone);
        }
        foreach (var label in dbContext.Labels.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Labels.Remove(label);
        }
        // Test
        foreach (var item in dbContext.Metrics.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Metrics.Remove(item);
        }
        foreach (var item in dbContext.TestCaseRuns.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestCaseRuns.Remove(item);
        }
        foreach (var item in dbContext.TestRuns.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestRuns.Remove(item);
        }
        foreach (var testCase in dbContext.TestCases
            .Include(x=>x.TestSteps)
            .Where(x => x.TestProjectId == project.Id))
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

        // Folders
        foreach (var item in dbContext.TestRepositoryFolders.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestRepositoryFolders.Remove(item);
        }
        foreach (var item in dbContext.TestLabFolders.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.TestLabFolders.Remove(item);
        }


        // Requirement

        foreach (var item in dbContext.Requirements.Include(x => x.TestLinks).Include(x => x.RequirementFields).Where(x => x.TestProjectId == project.Id))
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

        // Automation

        foreach (var item in dbContext.PipelineJobs.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.PipelineJobs.Remove(item);
        }
        foreach (var item in dbContext.Runners.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Runners.Remove(item);
        }
        foreach (var item in dbContext.Pipelines.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Pipelines.Remove(item);
        }
        foreach (var item in dbContext.Jobs.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.Jobs.Remove(item);
        }

        foreach (var item in dbContext.ExternalSystems.Where(x => x.TestProjectId == project.Id))
        {
            dbContext.ExternalSystems.Remove(item);
        }
        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();
    }
}
