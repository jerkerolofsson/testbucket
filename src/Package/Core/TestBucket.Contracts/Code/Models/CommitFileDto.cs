using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Code.Models;
public class CommitFileDto
{
    public required string Filename { get; set; }

    public int Additions { get; set; }

    public int Deletions { get; set; }

    public int Changes { get; set; }

    public string? Status { get; set; }

    public string? BlobUrl { get; set; }

    public string? ContentsUrl { get; set; }

    public string? RawUrl { get; set; }

    public required string Sha { get; set; }
    public string? Patch { get; set; }
    public string? PreviousFileName { get; set; }
}
