namespace TestBucket.Domain.IntegrationTests.TestResources
{
    [Component("Test Resources")]
    [IntegrationTest]
    [FunctionalTest]
    public class TestResourceManagerTests(ProjectFixture Fixture) : IClassFixture<ProjectFixture>
    {
        [Fact]
        [TestDescription("""
            Verifies that an resource can be added and that it is not initially locked 
            """)]
        public async Task AddTestResource_NotLocked()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"]);
            try
            {
                // Act
                var account = await Fixture.Resources.GetByIdAsync(resource1.Id);

                // Assert
                Assert.Equal("owner1", resource1.Owner);
                Assert.Contains("phone", resource1.Types);
                Assert.Contains("android", resource1.Types);
                Assert.False(resource1.Locked);
            }
            finally
            {
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that when browsing for resources after deletion it is not included in the result
            """)]
        public async Task DeleteTestResource_IsDeleted()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"]);
            try
            {
                var result = await Fixture.Resources.BrowseAsync();
                var account1Browse = result.Items.Where(x=>x.Id == resource1.Id).FirstOrDefault();   
                Assert.NotNull(resource1);

                // Act
                await Fixture.Resources.DeleteAsync(resource1);

                // Assert
                var resultAfterDelete = await Fixture.Resources.BrowseAsync();
                var account1AfterDelete = resultAfterDelete.Items.Where(x => x.Id == resource1.Id).FirstOrDefault();
                Assert.Null(account1AfterDelete);
            }
            finally
            {
            }
        }


        [Fact]
        [TestDescription("""
            Verifies that an resource can be updated
            """)]
        public async Task UpdateTestResource_IsUpdated()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"]);
            try
            {
                // Act
                resource1.Types = ["keyboard"];
                await Fixture.Resources.UpdateAsync(resource1);

                // Assert
                var result = await Fixture.Resources.BrowseAsync();
                var afterUpdate = result.Items.Where(x => x.Id == resource1.Id).FirstOrDefault();
                Assert.NotNull(afterUpdate);
                Assert.DoesNotContain("phone", afterUpdate.Types);
                Assert.DoesNotContain("android", afterUpdate.Types);
                Assert.Contains("keyboard", afterUpdate.Types);
            }
            finally
            {
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that an resource is locked (Locked==true) after allocation
            """)]
        public async Task AllocateResource_AccountIsLocked()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"]);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" }
                };

                // Act
                var resources = await Fixture.Resources.AllocateAsync(run, guid, dependencies);
                Assert.Single(resources.Resources);

                // Assert
                Assert.Equal("owner1", resources.Resources[0].Owner);
                Assert.Equal(resource1.Id, resources.Resources[0].Id);
                Assert.True(resources.Resources[0].Locked);
            }
            finally
            {
                await Fixture.Resources.ReleaseAsync(guid);
                await Fixture.Resources.DeleteAsync(resource1);

            }
        }

        [Fact]
        [TestDescription("""
            Verifies that a resource cannot be allocated twice 

            # Steps

            """)]
        public async Task AllocateResource_WhenAllLocked_EmptyCollectionReturned()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone"]);
            string guid = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" }
                };
                var resources = await Fixture.Resources.AllocateAsync(run, guid, dependencies);

                // Act
                var resourcesShouldFail = await Fixture.Resources.AllocateAsync(run, guid2, dependencies);

                // Assert
                Assert.Empty(resourcesShouldFail.Resources);
            }
            finally
            {
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that a resource cannot be allocated if Enabled is false

            # Steps
            1. Add a resource
            2. Set Enabled to false
            3. Try to allocate the resource: An empty collection is returned
            """)]
        public async Task AllocateResource_WhenDisabled_EmptyCollectionReturned()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone"]);
            resource1.Enabled = false;
            await Fixture.Resources.UpdateAsync(resource1);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" }
                };

                // Act
                var resourcesShouldFail = await Fixture.Resources.AllocateAsync(run, guid, dependencies);

                // Assert
                Assert.Empty(resourcesShouldFail.Resources);
            }
            finally
            {
                await Fixture.Resources.ReleaseAsync(guid);
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that a resource cannot be allocated if Health is Unhealthy

            # Steps
            1. Add a resource
            2. Set Health to Unhealthy
            3. Try to allocate the resource: An empty collection is returned
            """)]
        public async Task AllocateResource_WhenUnhealthy_EmptyCollectionReturned()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone"]);
            resource1.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy;
            await Fixture.Resources.UpdateAsync(resource1);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" }
                };

                // Act
                var resourcesShouldFail = await Fixture.Resources.AllocateAsync(run, guid, dependencies);

                // Assert
                Assert.Empty(resourcesShouldFail.Resources);
            }
            finally
            {
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that a resource cannot be allocated if Health is Degraded

            # Steps
            1. Add a resource
            2. Set Health to Degraded
            3. Try to allocate the resource: An empty collection is returned
            """)]
        public async Task AllocateResource_WhenDegraded_EmptyCollectionReturned()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone"]);
            resource1.Health = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded;
            await Fixture.Resources.UpdateAsync(resource1);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" }
                };

                // Act
                var resourcesShouldFail = await Fixture.Resources.AllocateAsync(run, guid, dependencies);

                // Assert
                Assert.Empty(resourcesShouldFail.Resources);
            }
            finally
            {
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that an resource that has multiple types can be allocated when specifying one of the types
            """)]
        public async Task AllocateResource_WithFirstType_AccountIsLocked()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"]);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
            {
                new TestCaseDependency { ResourceType = "android" }
            };

                // Act
                var resources = await Fixture.Resources.AllocateAsync(run, guid, dependencies);

                // Assert
                Assert.Single(resources.Resources);
                Assert.Equal("owner1", resources.Resources[0].Owner);
                Assert.Equal(resource1.Id, resources.Resources[0].Id);
                Assert.True(resources.Resources[0].Locked);
            }
            finally
            {
                await Fixture.Resources.ReleaseAsync(guid);
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that when trying to allocate a resource without any supported type, an empty collection is returned
            """)]
        public async Task AllocateResource_WithoutSupportedType_EmptyCollectionReturned()
        {
            // Arrange
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"]);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "mouse" }
                };

                // Act
                var resources = await Fixture.Resources.AllocateAsync(run, guid, dependencies);

                // Assert
                Assert.Empty(resources.Resources);
            }
            finally
            {
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }

        [Fact]
        [TestDescription("""
            Verifies that when allocating a resource with variables, those variables are added to the TestExecutionContext in the 
            format resources__type__index__key = value
            """)]
        public async Task AllocateAccountWithVariables_AccountIsLockedAndVariablesAdded()
        {
            // Arrange
            var variables = new Dictionary<string, string>() { ["ssid"]= "helicopter" };
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone"], variables);
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
                        new TestCaseDependency { ResourceType = "phone" }
                    }
                };

                // Act
                var resources = await Fixture.Resources.AllocateAsync(run, context);
                Assert.Single(resources.Resources);

                // Assert
                Assert.Equal("owner1", resources.Resources[0].Owner);
                Assert.Equal(resource1.Id, resources.Resources[0].Id);
                Assert.True(resources.Resources[0].Locked);
                Assert.True(context.Variables.ContainsKey("resources__phone__0__ssid"));
                Assert.Equal("helicopter", context.Variables["resources__phone__0__ssid"]);

            }
            finally
            {
                await Fixture.Resources.ReleaseAsync(guid);
                await Fixture.Resources.DeleteAsync(resource1);

            }
        }


        [Fact]
        [TestDescription("""
            Verifies that the environment variable is named after the requested resource type.
            
            When a resource has multiple types, for example "phone" and "android", and the requested type is "android",
            the environment variable should be named "resources__android__0__ssid" and not "resources__android__0__ssid".

            # Steps
            1. Create a resource with types "phone" and "android"
            2. Allocate the resource with the requested type "android"
            3. Verify that the environment variable is named "resources__android__0__ssid" and not "resources__phone__0__ssid".
            """)]
        public async Task AllocateAccountWithVariables_WithPhoneAndroidAndAndroidRequested_VariableNamedAsAndroid()
        {
            // Arrange
            var variables = new Dictionary<string, string>() { ["ssid"] = "helicopter" };
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"], variables);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var context = new TestExecutionContext
                {
                    Guid = guid,
                    TestRunId = run.Id,
                    ProjectId = Fixture.ProjectId,
                    TeamId = Fixture.TeamId,
                    Dependencies = new List<TestCaseDependency>
                    {
                        new TestCaseDependency { ResourceType = "android" }
                    }
                };

                // Act
                var resources = await Fixture.Resources.AllocateAsync(run, context);
                Assert.Single(resources.Resources);

                // Assert
                Assert.Equal("owner1", resources.Resources[0].Owner);
                Assert.Equal(resource1.Id, resources.Resources[0].Id);
                Assert.True(resources.Resources[0].Locked);
                Assert.True(context.Variables.ContainsKey("resources__android__0__ssid"));
                Assert.Equal("helicopter", context.Variables["resources__android__0__ssid"]);

            }
            finally
            {
                await Fixture.Resources.ReleaseAsync(guid);
                await Fixture.Resources.DeleteAsync(resource1);

            }
        }

        [Fact]
        [TestDescription("""
            Verifies that the environment variable is named after the requested resource type.
            
            When a resource has multiple types, for example "phone" and "android", and the requested type is "phone",
            the environment variable should be named "resources__phone__0__ssid" and not "resources__android__0__ssid".

            # Steps
            1. Add a resource with types "phone" and "android"
            2. Allocate the resource with the requested type "phone"
            3. Verify that the environment variable is named "resources__phone__0__ssid" and not "resources__android__0__ssid".
            """)]
        public async Task AllocateAccountWithVariables_WithPhoneAndroidAndPhoneRequested_VariableNamedAsPhone()
        {
            // Arrange
            var variables = new Dictionary<string, string>() { ["ssid"] = "helicopter" };
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone", "android"], variables);
            string guid = Guid.NewGuid().ToString();
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var context = new TestExecutionContext
                {
                    Guid = guid,
                    TestRunId = run.Id,
                    ProjectId = Fixture.ProjectId,
                    TeamId = Fixture.TeamId,
                    Dependencies = new List<TestCaseDependency>
                    {
                        new TestCaseDependency { ResourceType = "phone" }
                    }
                };

                // Act
                var resources = await Fixture.Resources.AllocateAsync(run, context);
                Assert.Single(resources.Resources);

                // Assert
                Assert.Equal("owner1", resources.Resources[0].Owner);
                Assert.Equal(resource1.Id, resources.Resources[0].Id);
                Assert.True(resources.Resources[0].Locked);
                Assert.True(context.Variables.ContainsKey("resources__phone__0__ssid"));
                Assert.Equal("helicopter", context.Variables["resources__phone__0__ssid"]);

            }
            finally
            {
                await Fixture.Resources.ReleaseAsync(guid);
                await Fixture.Resources.DeleteAsync(resource1);

            }
        }

        [Fact]
        [TestDescription("""
            Verifies that an resource is unlocked (Locked==false) when released
            """)]
        public async Task ReleaseResources_ResourceIsUnlocked()
        {
            // Arrange
            string guid = Guid.NewGuid().ToString();
            var resource1 = await Fixture.Resources.AddTestResourceAsync("owner1", ["phone"]);
            try
            {
                var run = await Fixture.Runs.AddAsync();
                var dependencies = new List<TestCaseDependency>
                {
                    new TestCaseDependency { ResourceType = "phone" }
                };

                // Act
                var resources = await Fixture.Resources.AllocateAsync(run, "12345", dependencies);
                await Fixture.Resources.ReleaseAsync("12345");

                // Assert
                Assert.Single(resources.Resources);
                var account = await Fixture.Resources.GetByIdAsync(resource1.Id);
                Assert.NotNull(account);
                Assert.False(account.Locked);
            }
            finally
            {
                await Fixture.Resources.DeleteAsync(resource1);
            }
        }
    }
}
