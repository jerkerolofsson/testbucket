using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Keyboard
{
    [Flags]
    public enum ModifierKey
    {
        None = 0,
        Shift = 1,
        Ctrl = 2,
        Alt = 4,
        Meta = 8
    }
}
