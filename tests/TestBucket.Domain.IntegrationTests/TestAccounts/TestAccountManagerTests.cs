namespace TestBucket.Domain.IntegrationTests.TestAccounts
{
    [Component("TestAccounts")]
    [IntegrationTest]
    [FunctionalTest]
    public class TestAccountManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        /// <summary>
        /// Verifies that an account can be added and that it is not initially locked 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddTestAccount_NotLocked()
        {
            // Arrange
            var account1 = await Fixture.Accounts.AddTestAccountAsync("owner1", "email");
            try
            {
                // Act
                var account = await Fixture.Accounts.GetByIdAsync(account1.Id);

                // Assert
                Assert.Equal("owner1", account1.Owner);
                Assert.Equal("email", account1.Type);
                Assert.False(account1.Locked);
            }
            finally
            {
                await Fixture.Accounts.DeleteAccountAsync(account1);
            }
        }

        /// <summary>
        /// Verifies that when browsing for accounts after deletion it is not included in the result
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteTestAccount_IsDeleted()
        {
            // Arrange
            var account1 = await Fixture.Accounts.AddTestAccountAsync("owner1", "email");
            try
            {
                var result = await Fixture.Accounts.BrowseAsync();
                var account1Browse = result.Items.Where(x=>x.Id == account1.Id).FirstOrDefault();   
                Assert.NotNull(account1);

                // Act
                await Fixture.Accounts.DeleteAccountAsync(account1);

                // Assert
                var resultAfterDelete = await Fixture.Accounts.BrowseAsync();
                var account1AfterDelete = resultAfterDelete.Items.Where(x => x.Id == account1.Id).FirstOrDefault();
                Assert.Null(account1AfterDelete);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Verifies that an account can be updated
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateTestAccount_IsUpdated()
        {
            // Arrange
            var account1 = await Fixture.Accounts.AddTestAccountAsync("owner1", "coop");
            try
            {
                // Act
                account1.Type = "ica-kort";
                account1.Owner = "ica";
                await Fixture.Accounts.UpdateAsync(account1);

                // Assert
                var result = await Fixture.Accounts.BrowseAsync();
                var account1AfterUpdate = result.Items.Where(x => x.Id == account1.Id).FirstOrDefault();
                Assert.NotNull(account1AfterUpdate);
                Assert.Equal("ica-kort", account1AfterUpdate.Type);
                Assert.Equal("ica", account1AfterUpdate.Owner);
            }
            finally
            {
            }
        }


        /// <summary>
        /// Verifies that an account which is locked cannot be allocated
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AllocateAccount_WithDisabledAccount_Fails()
        {
            // Arrange
            var account1 = await Fixture.Accounts.AddTestAccountAsync("owner1", "github");
            account1.Enabled = false;
            await Fixture.Accounts.UpdateAsync(account1);

            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { AccountType = "github" }
                };

                // Act
                var resources = await Fixture.Accounts.AllocateAsync(run, guid, dependencies);

                // Assert
                Assert.Empty(resources.Accounts);
            }
            finally
            {
                await Fixture.Accounts.DeleteAccountAsync(account1);

            }
        }

        /// <summary>
        /// Verifies that an account is locked (Locked==true) after allocation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AllocateAccount_AccountIsLocked()
        {
            // Arrange
            var account1 = await Fixture.Accounts.AddTestAccountAsync("owner1", "github");
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { AccountType = "github" }
                };

                // Act
                var resources = await Fixture.Accounts.AllocateAsync(run, guid, dependencies);

                // Assert
                Assert.Single(resources.Accounts);
                Assert.Equal("owner1", resources.Accounts[0].Owner);
                Assert.Equal("github", resources.Accounts[0].Type);
                Assert.True(resources.Accounts[0].Locked);
            }
            finally
            {
                await Fixture.Accounts.ReleaseAsync(guid);
                await Fixture.Accounts.DeleteAccountAsync(account1);

            }
        }

        [Fact]
        [TestDescription("""
            Verifies that when allocating a resource with variables, those variables are added to the TestExecutionContext in the 
            format accounts__type__index__key = value
            """)]
        public async Task AllocateAccountWithVariables_AccountIsLockedAndVariablesAdded()
        {
            // Arrange
            var variables = new Dictionary<string, string>() { ["ssid"]= "helicopter" };
            var account1 = await Fixture.Accounts.AddTestAccountAsync("owner1", "wifi6", variables);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var context = new TestExecutionContext 
                { 
                    Guid = guid, 
                    TestRunId = run.Id, 
                    ProjectId = Fixture.ProjectId, 
                    TeamId = Fixture.TeamId, Dependencies = new List<TestCaseDependency>
                    {
                        new TestCaseDependency { AccountType = "wifi6" }
                    }
                };

                // Act
                var resources = await Fixture.Accounts.AllocateAsync(run, context);

                // Assert
                Assert.Single(resources.Accounts);
                Assert.Equal("owner1", resources.Accounts[0].Owner);
                Assert.Equal("wifi6", resources.Accounts[0].Type);
                Assert.True(resources.Accounts[0].Locked);
                Assert.True(context.Variables.ContainsKey("accounts__wifi6__0__ssid"));
                Assert.Equal("helicopter", context.Variables["accounts__wifi6__0__ssid"]);

            }
            finally
            {
                await Fixture.Accounts.ReleaseAsync(guid);
                await Fixture.Accounts.DeleteAccountAsync(account1);

            }
        }

        [Fact]
        [TestDescription("""
            Verifies that an account is unlocked (Locked==false) when released
            """)]
        public async Task ReleaseAccounts_AccountIsUnlocked()
        {
            // Arrange
            string guid = Guid.NewGuid().ToString();
            var account1 = await Fixture.Accounts.AddTestAccountAsync("owner1", "wifi123");
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { AccountType = "wifi123" }
                };

                // Act
                var resources = await Fixture.Accounts.AllocateAsync(run, "12345", dependencies);

                // Assert
                await Fixture.Accounts.ReleaseAsync("12345");
                Assert.Single(resources.Accounts);
                var account = await Fixture.Accounts.GetByIdAsync(account1.Id);
                Assert.NotNull(account);
                Assert.False(account.Locked);
            }
            finally
            {
                await Fixture.Accounts.DeleteAccountAsync(account1);
            }
        }
    }
}
