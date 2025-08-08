using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Editor.Models;
public class ReviewSettings
{
    public bool AutomaticallyApproveTestIfReviewFinishedWithOnlyPositiveVotes { get; set; } = true;
}
