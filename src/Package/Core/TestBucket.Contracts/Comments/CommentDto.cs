namespace TestBucket.Contracts.Comments;
public class CommentDto
{

    /// <summary>
    /// Timestamp when the test case was created
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Modified by user name
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Timestamp when the test case was Modified
    /// </summary>
    public DateTimeOffset Modified { get; set; }

    /// <summary>
    /// Created by user name
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Comment, in markdown format
    /// </summary>
    public string? Markdown { get; set; }

    /// <summary>
    /// Icon displayed for action
    /// </summary>
    public string? LoggedActionIcon { get; set; }

    /// <summary>
    /// Accent color for logged action
    /// </summary>
    public string? LoggedActionColor { get; set; }

    /// <summary>
    /// A special action which is logged, for example "approved"
    /// </summary>
    public string? LoggedAction { get; set; }

    /// <summary>
    /// Argument related to the action, for example a specific field value
    /// </summary>
    public string? LoggedActionArgument { get; set; }
}
