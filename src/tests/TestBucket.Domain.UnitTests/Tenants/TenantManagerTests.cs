    [Fact]
    public async Task GetTenantByIdAsync_ShouldReturnTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        var tenant = await tenantManager.GetTenantByIdAsync(principal, tenantId);

        // Assert
        Assert.NotNull(tenant);
        Assert.Equal(tenantId, tenant?.Id);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPagedResult_WhenQueryIsValid()
    {
        // Arrange
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider);
        var query = new SearchQuery { Page = 1, PageSize = 10 };

        // Act
        var result = await tenantManager.SearchAsync(principal, query);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResult<Tenant>>(result);
    }

    [Fact]
    public async Task UpdateTenantCiCdKeyAsync_ShouldUpdateKey_WhenTenantExists()
    {
        // Arrange
        var tenantId = "tenant-id";
        var principal = CreatePrincipal();
        var tenantManager = new TenantManager(new FakeProjectRepository(), new FakeTenantRepository(), _settingsProvider);
        await tenantManager.CreateAsync(principal, tenantId);

        // Act
        await tenantManager.UpdateTenantCiCdKeyAsync(tenantId);
        var tenant = await tenantManager.GetTenantByIdAsync(principal, tenantId);

        // Assert
        Assert.NotNull(tenant?.CiCdAccessToken);
        Assert.NotNull(tenant?.CiCdAccessTokenExpires);
    }