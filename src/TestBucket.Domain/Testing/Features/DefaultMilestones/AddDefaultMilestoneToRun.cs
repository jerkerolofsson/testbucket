using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Milestones;
using TestBucket.Domain.Tenants.Models;
using TestBucket.Domain.Testing.Models;
using TestBucket.Traits.Core;

namespace TestBucket.Domain.Testing.Features.DefaultMilestones;

public class AddDefaultMilestoneToRun
{
    /// <summary>
    /// Adds a milestone field for the test-run if:
    /// 1. There is a milestone field definition with TraitType.Milestone
    /// 2. ProjectId and TenantId set for the run
    /// 3. There is an open milestone with the current date inside the milestone start/end date
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="testRun"></param>
    /// <param name="testRunFieldDefinitions"></param>
    /// <param name="milestoneManager"></param>
    /// <param name="fieldManager"></param>
    /// <returns></returns>
    public static async Task AddAsync(ClaimsPrincipal principal, TestRun testRun, IReadOnlyList<FieldDefinition> testRunFieldDefinitions, IMilestoneManager milestoneManager, IFieldManager fieldManager)
    {
        if (testRun.TestProjectId is not null && testRun.TenantId is not null)
        {
            var tenantId = principal.GetTenantIdOrThrow(testRun);

            var milestoneFieldDefinition = testRunFieldDefinitions.FirstOrDefault(x => x.TraitType == TraitType.Milestone);
            if (milestoneFieldDefinition is not null)
            {
                // Get the latest milestone
                var currentOpenMilestone = await milestoneManager.GetCurrentMilestoneAsync(principal, testRun.TestProjectId.Value);
                if (currentOpenMilestone?.Title is not null)
                {
                    var field = new TestRunField
                    {
                        TenantId = tenantId,
                        TestRunId = testRun.Id,
                        FieldDefinitionId = milestoneFieldDefinition.Id,
                        FieldDefinition = milestoneFieldDefinition,
                    };
                    if (FieldValueConverter.TryAssignValue(milestoneFieldDefinition, field, [currentOpenMilestone.Title]))
                    {
                        await fieldManager.UpsertTestRunFieldAsync(principal, field);
                    }
                }
            }
        }
    }
}
