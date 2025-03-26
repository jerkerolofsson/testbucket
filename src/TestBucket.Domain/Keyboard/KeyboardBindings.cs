using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Keyboard
{
    /// <summary>
    /// Contains keyboard bindings 
    /// </summary>
    public class KeyboardBindings
    {
        public KeyboardBinding? UnifiedSearchBinding { get; set; }

        public Dictionary<string, KeyboardBinding>? Commands { get; set;  }
    }
}
