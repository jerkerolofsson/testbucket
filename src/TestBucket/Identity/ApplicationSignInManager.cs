using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TestBucket.Data.Identity.Models;

namespace TestBucket.Identity;

public class ApplicationSignInManager : SignInManager<ApplicationUser>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public ApplicationSignInManager(UserManager<ApplicationUser> userManager,
                               IHttpContextAccessor contextAccessor,
                               IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                               IOptions<IdentityOptions> optionsAccessor,
                               ILogger<SignInManager<ApplicationUser>> logger,
                               IAuthenticationSchemeProvider schemes,
                               IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _contextAccessor = contextAccessor;
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        // Resolve tenant before signing in
        var tenantId = TenantResolver.ResolveTenantId(_contextAccessor);
        var user = await UserManager.FindByNameAsync(userName);

        if (user == null || user.TenantId != tenantId)
        {
            return SignInResult.Failed;
        }

        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }
}