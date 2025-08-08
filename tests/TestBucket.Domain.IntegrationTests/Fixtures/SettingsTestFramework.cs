using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Contracts;
using TestBucket.Domain.Fields;
using TestBucket.Domain.Issues;
using TestBucket.Domain.Issues.Models;
using TestBucket.Domain.Requirements.Models;
using TestBucket.Domain.Settings;
using TestBucket.Domain.Settings.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.IntegrationTests.Fixtures
{
    /// <summary>
    /// Test fixture
    /// </summary>
    /// <param name="Fixture"></param>
    public class SettingsTestFramework(ProjectFixture Fixture)
    {

        internal SettingsCategory[] Categories
        {
            get
            {
                var manager = Fixture.Services.GetRequiredService<ISettingsManager>();
                return manager.Categories;
            }
        }
        internal ISetting[] Search(string text)
        {
            var principal = Impersonation.Impersonate(Fixture.App.Tenant);
            var context = new SettingContext { Principal = principal, ProjectId = Fixture.ProjectId, TenantId = Fixture.App.Tenant };
            var manager = Fixture.Services.GetRequiredService<ISettingsManager>();
            return manager.Search(context, text);
        }

    }
}
