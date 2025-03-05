
using TestBucket.Components.Tenants;

namespace TestBucket.Components.Users;

public record class CreateUserResponse(ApplicationUser? User, IdentityError[] Errors);

internal class UserRegistrationService : TenantBaseService
{
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserRegistrationService> _logger;
    private readonly NavigationManager _navigationManager;
    private readonly IEmailSender<ApplicationUser> _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public UserRegistrationService(
        IUserStore<ApplicationUser> userStore, 
        UserManager<ApplicationUser> userManager, 
        ILogger<UserRegistrationService> logger, 
        NavigationManager navigationManager, 
        IEmailSender<ApplicationUser> emailSender, 
        SignInManager<ApplicationUser> signInManager,
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        AuthenticationStateProvider authenticationStateProvider) : base(authenticationStateProvider)
    {
        _userStore = userStore;
        _userManager = userManager;
        _logger = logger;
        _navigationManager = navigationManager;
        _emailSender = emailSender;
        _signInManager = signInManager;
        _dbContextFactory = dbContextFactory;
    }


    public async Task<PagedResult<ApplicationUser>> SearchAsync(SearchQuery query)
    {
        var tenantId = await GetTenantIdAsync();

        using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var users = dbContext.Users.AsNoTracking().Where(x => x.TenantId == tenantId);

        // Apply filter
        if (query.Text is not null)
        {
            users = users.Where(x => x.UserName != null && x.UserName.ToLower().Contains(query.Text.ToLower()));
        }

        long totalCount = await users.LongCountAsync();
        var items = users.OrderBy(x => x.UserName).Skip(query.Offset).Take(query.Count);

        return new PagedResult<ApplicationUser>
        {
            TotalCount = totalCount,
            Items = items.ToArray()
        };
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }

    public async Task<CreateUserResponse> RegisterUserAsync(string tenantId, string email, string password, string? returnUrl)
    {
        // Todo: Get the tenant and see if they allow registrations

        var user = new ApplicationUser();
        user.TenantId = tenantId;

        await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);

        await _userManager.AddClaimAsync(user, new Claim("tenant", tenantId));

        if (!result.Succeeded)
        {
            return new CreateUserResponse(null, result.Errors.ToArray());
        }

        _logger.LogInformation("User created a new account with password.");

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _navigationManager.GetUriWithQueryParameters(
            _navigationManager.ToAbsoluteUri($"{tenantId}/Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = returnUrl });

        await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));

        if (!_userManager.Options.SignIn.RequireConfirmedAccount)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
        return new CreateUserResponse(user, []);
    }
}
