using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TestBucket.Data.Identity.Models;

namespace TestBucket.Data.Identity;
public class ApplicationUserStore : UserStore<ApplicationUser>
{
    public ApplicationUserStore(DbContext context, string tenantId)
      : base(context)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; }

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