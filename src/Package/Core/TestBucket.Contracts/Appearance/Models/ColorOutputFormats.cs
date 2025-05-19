using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Appearance.Models;

/// <summary>
/// Specifies different output formats for <seealso cref="ThemeColor"/>.
/// </summary>
public enum ColorOutputFormats
{
    /// <summary>
    /// Output will be starting with a # and include r,g and b but no alpha values. Example #ab2a3d
    /// </summary>
    Hex,
    /// <summary>
    /// Output will be starting with a # and include r,g and b and alpha values. Example #ab2a3dff
    /// </summary>
    HexA,
    /// <summary>
    /// Will output css like output for value. Example rgb(12,15,40)
    /// </summary>
    RGB,
    /// <summary>
    /// Will output css like output for value with alpha. Example rgba(12,15,40,0.42)
    /// </summary>
    RGBA,
    /// <summary>
    /// Will output the color elements without any decorator and without alpha. Example 12,15,26
    /// </summary>
    ColorElements
}
