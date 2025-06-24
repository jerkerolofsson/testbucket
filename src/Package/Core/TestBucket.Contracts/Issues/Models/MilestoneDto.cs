using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Issues.Models;
public class MilestoneDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public long Id { get; set; }
}
