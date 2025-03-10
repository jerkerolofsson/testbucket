using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Files.Models;
public class FileResource
{
    /// <summary>
    /// DB ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Original file name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Tenant ID
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Content Type
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Bytes
    /// </summary>
    public required byte[] Data { get; set; }

    /// <summary>
    /// File Size
    /// </summary>
    public int Length { get; set; } = 0;

    public DateTimeOffset? Created { get; set; }

    /// <summary>
    /// For test run attachments, like screenshots or log files
    /// </summary>
    public long? TestRunId { get; set; }

    /// <summary>
    /// For test case attachments, like screenshots or log files
    /// </summary>
    public long? TestCaseId { get; set; }

    /// <summary>
    /// For test case run attachments, like screenshots or log files
    /// </summary>
    public long? TestCaseRunId { get; set; }

    /// <summary>
    /// For test suite, can contain documents that describe the test suite
    /// </summary>
    public long? TestSuiteId { get; set; }

    /// <summary>
    /// For test suite folders, can contain requirements or other documents for RAG
    /// </summary>
    public long? TestSuiteFolderId { get; set; }

    /// <summary>
    /// For test projects, can contain documents describing the resource
    /// </summary>
    public long? TestProjectId { get; set; }
}
