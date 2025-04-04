﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using TestBucket.Domain.Identity.Models;

namespace TestBucket.Data.Identity;
public class ApplicationUserStore : UserStore<ApplicationUser>
{
    public ApplicationUserStore(DbContext context, string tenantId)
      : base(context)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; }

    /// <summary>
    /// Browse all users
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<PagedResult<ApplicationUser>> BrowseAsync(int offset, int count)
    {
        var users = this.Users.Where(x => x.TenantId == TenantId);

        var totalCount = await users.LongCountAsync();
        var items = await users
            .OrderBy(x => x.UserName)
            .Skip(offset)
            .Take(count)
            .ToArrayAsync();

        return new PagedResult<ApplicationUser>()
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public override Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        if (user == null)
        {
            throw new ArgumentNullException("user");
        }

        user.TenantId = this.TenantId;
        return base.CreateAsync(user, cancellationToken);
    }

    public override Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return this.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == email && x.TenantId == TenantId, cancellationToken);
    }

    public override Task<ApplicationUser?> FindByNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return this.Users.SingleOrDefaultAsync(x => x.NormalizedUserName == userName && x.TenantId == TenantId, cancellationToken);
    }

    public override Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return this.Users.SingleOrDefaultAsync(u => u.Id == userId && u.TenantId == this.TenantId);
    }
}