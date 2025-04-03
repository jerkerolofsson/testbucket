using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Identity.Permissions;

namespace TestBucket.Data.Identity
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public PermissionsRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddProjectUserPermissionAsync(ProjectUserPermission userPermission)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.ProjectUserPermissions.AddAsync(userPermission);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddTenantRolePermissionAsync(RolePermission rolePermission)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.RolePermissions.AddAsync(rolePermission);
            await dbContext.SaveChangesAsync();
        }

        public async Task<ProjectUserPermission[]> GetProjectUserPermissionsAsync(string tenantId, long userId, long projectId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.ProjectUserPermissions.AsNoTracking().Where(x => x.TestProjectId == projectId && x.TenantId == tenantId && x.ApplicationUserId == userId).ToArrayAsync();
        }

        public async Task<RolePermission[]> GetTenantRolePermissionsAsync(string tenantId, string role)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.RolePermissions.AsNoTracking().Where(x => x.Role == role && x.TenantId == tenantId).ToArrayAsync();
        }
        public async Task<RolePermission[]> GetTenantRolePermissionsAsync(string tenantId)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await dbContext.RolePermissions.AsNoTracking().Where(x => x.TenantId == tenantId).ToArrayAsync();
        }

        public async Task UpdateProjectUserPermissionAsync(ProjectUserPermission userPermission)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.ProjectUserPermissions.Update(userPermission);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateTenantRolePermissionAsync(RolePermission rolePermission)
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.RolePermissions.Update(rolePermission);
            await dbContext.SaveChangesAsync();
        }
    }
}
