using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Appium.PageSources;

[Flags]
public enum ElementMatching
{
    MatchText = 1,
    MatchId = 2,
    MatchContentDescription = 4,

    UseEmbeddings = 0x1000
}
