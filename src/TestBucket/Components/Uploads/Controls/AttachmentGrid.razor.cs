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
    [Parameter] public bool AllowUpload { get; set; } = false;

    [Parameter] public string? Style { get; set; }
    [Parameter] public string Class { get; set; } = "pa-2";

    [Parameter] public EventCallback<FileResource> FileUploaded { get; set; }

    private List<FileResource> _attachments = [];

    private bool _isGrid = true;

    public Color GridColor => _isGrid ? Color.Tertiary : Color.Default;
    public Color ListColor => !_isGrid ? Color.Tertiary : Color.Default;

    private async Task OnFileUploadedAsync(FileResource _)
    {
        await ReloadAttachmentsAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await ReloadAttachmentsAsync();
    }

    private async Task ReloadAttachmentsAsync()
    { 
        if (TestCaseId is not null)
        {
            _attachments = (await attachmentsService.GetTestCaseAttachmentsAsync(TestCaseId.Value)).ToList();
        }
        else if (TestCaseRunId is not null)
        {
            _attachments = (await attachmentsService.GetTestCaseRunAttachmentsAsync(TestCaseRunId.Value)).ToList();
        }
    }
}
