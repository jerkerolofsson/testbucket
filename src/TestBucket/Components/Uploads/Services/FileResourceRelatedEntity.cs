namespace TestBucket.Components.Uploads.Services;

public record class FileResourceRelatedEntity
{
    public long? TestCaseId { get; set; }
    public long? TestRunId { get; set; }
    public long? TestCaseRunId { get; set; }

    public long? RequirementId { get; set; }
    public long? RequirementSpecificationId { get; set; }

    public long? TestSuiteId { get; set; }
    public long? TestSuiteFolderId { get; set; }
    public long? TestProjectId { get; set; }

    public long? ComponentId { get; set; }
    public long? LayerId { get; set; }
    public long? SystemId { get; set; }
    public long? FeatureId { get; set; }
}
