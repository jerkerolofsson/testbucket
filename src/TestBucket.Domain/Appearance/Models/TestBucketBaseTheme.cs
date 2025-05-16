using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Appearance.Models;
public abstract class TestBucketBaseTheme
{
    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public abstract string Dark { get; }
    /// <summary>
    /// CSS Stylesheet
    /// </summary>
    public abstract string Light { get; }
}
