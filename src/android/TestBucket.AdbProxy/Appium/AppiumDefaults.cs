using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Appium;
internal class AppiumDefaults
{
    public static readonly TimeSpan SwipeDuration = TimeSpan.FromMilliseconds(1000);
    public static readonly TimeSpan FlickDuration = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// Time to sleep when trying to find something before checking again.
    /// This is not the timeout. This is the interval between checking
    /// </summary>
    public static readonly TimeSpan FindDelayDuration = TimeSpan.FromMilliseconds(250);
}
