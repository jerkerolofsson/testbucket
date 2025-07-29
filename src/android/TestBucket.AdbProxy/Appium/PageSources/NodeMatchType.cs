using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Appium.PageSources;
public enum NodeMatchType
{
    ExactText,
    PartialText,

    ExactContentDescription,
    PartialContentDescription,

    ExactId,
    PartialId,
}
