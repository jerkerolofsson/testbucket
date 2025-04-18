using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Fields.Models;

/// <summary>
/// Type of field
/// </summary>
public enum FieldType
{
    /// <summary>
    /// string
    /// </summary>
    String = 0,

    /// <summary>
    /// int64 / integer / long
    /// </summary>
    Integer = 1,

    /// <summary>
    /// double / floating point
    /// </summary>
    Double = 2,

    /// <summary>
    /// bool
    /// </summary>
    Boolean = 3,

    /// <summary>
    /// DateTimeOffset
    /// </summary>
    DateTimeOffset = 4,

    /// <summary>
    /// Single item can be selected from options
    /// </summary>
    SingleSelection = 5,

    /// <summary>
    /// Multiple strings that can be freely selected
    /// </summary>
    StringArray = 6,

    /// <summary>
    /// Multiple strings, but can only be selected from a pre-defined set
    /// </summary>
    MultiSelection = 7,

    /// <summary>
    /// An image URI where the user can upload an image which is converted to a data uri
    /// </summary>
    ImageUri = 8,

}
