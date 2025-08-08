using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Editor.Models;
public class EditorSettings
{
    public bool BlockEditInReviewState { get; set; } = true;
    public bool ChangeStateToCompletedWhenApproved { get; set; } = true;
    public bool ChangeStateToOngoingWhenEditingTests { get; set; } = true;
}
