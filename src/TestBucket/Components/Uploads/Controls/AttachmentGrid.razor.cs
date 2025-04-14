using TestBucket.Components.Tests.TestSuites.Dialogs;
using TestBucket.Components.Uploads.Dialogs;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Components.Uploads.Controls;

public partial class AttachmentGrid
{
    [Parameter] public long? TestCaseId { get; set; }
    [Parameter] public long? TestRunId { get; set; }
    [Parameter] public long? TestCaseRunId { get; set; }
    [Parameter] public long? TestProjectId { get; set; }
    [Parameter] public long? TestSuiteId { get; set; }
    [Parameter] public long? TestSuiteFolderId { get; set; }
    [Parameter] public long? RequirementId { get; set; }
    [Parameter] public long? RequirementSpecificationId { get; set; }
    [Parameter] public bool AllowUpload { get; set; } = false;

    [Parameter] public string? Style { get; set; }
    [Parameter] public string Class { get; set; } = "pa-2";

    [Parameter] public EventCallback<FileResource> FileUploaded { get; set; }

    private List<FileResource> _attachments = [];

    private bool _isGrid = false;

    public Color GridColor => _isGrid ? Color.Tertiary : Color.Default;
    public Color ListColor => !_isGrid ? Color.Tertiary : Color.Default;

    private async Task OnAttachmentClickedAsync(FileResource resource)
    {
        var parameters = new DialogParameters<ViewFileResourceDialog>()
        {
            { x => x.Resource, resource },
        };

        var dialog = await dialogService.ShowAsync<ViewFileResourceDialog>(null, parameters, DefaultBehaviors.DialogOptions);
        var result = await dialog.Result;
    }

    private async Task OnFileUploadedAsync(FileResource _)
    {
        await ReloadAttachmentsAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await ReloadAttachmentsAsync();
    }

    public async Task ReloadAttachmentsAsync()
    { 
        if (TestCaseId is not null)
        {
            _attachments = (await attachmentsService.GetTestCaseAttachmentsAsync(TestCaseId.Value)).ToList();
        }
        else if (TestProjectId is not null)
        {
            _attachments = (await attachmentsService.GetTestProjectAttachmentsAsync(TestProjectId.Value)).ToList();
        }
        else if (TestSuiteFolderId is not null)
        {
            _attachments = (await attachmentsService.GetTestSuiteFolderAttachmentsAsync(TestSuiteFolderId.Value)).ToList();
        }
        else if (TestSuiteId is not null)
        {
            _attachments = (await attachmentsService.GetTestSuiteAttachmentsAsync(TestSuiteId.Value)).ToList();
        }
        else if (TestCaseRunId is not null)
        {
            _attachments = (await attachmentsService.GetTestCaseRunAttachmentsAsync(TestCaseRunId.Value)).ToList();
        }
        else if (RequirementId is not null)
        {
            _attachments = (await attachmentsService.GetRequirementAttachmentsAsync(RequirementId.Value)).ToList();
        }
        else if (RequirementSpecificationId is not null)
        {
            _attachments = (await attachmentsService.GetRequirementSpecificationAttachmentsAsync(RequirementSpecificationId.Value)).ToList();
        }
        else if (TestRunId is not null)
        {
            _attachments = (await attachmentsService.GetTestRunAttachmentsAsync(TestRunId.Value)).ToList();
        }
    }
}
