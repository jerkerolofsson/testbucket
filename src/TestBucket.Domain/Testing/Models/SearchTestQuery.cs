﻿
namespace TestBucket.Domain.Testing.Models;
public class SearchTestQuery : SearchQuery
{
    public long? TestRunId { get; set; }
    public long? TestSuiteId { get; set; }
    public long? FolderId { get; set; }

    public bool CompareFolder { get; set; } = true;
}
