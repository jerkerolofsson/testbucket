using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Keyboard
{
    public class KeyboardBinding
    {
        /// <summary>
        /// ID of the command
        /// </summary>
        public required string CommandId { get; set; }

        /// <summary>
        /// Modifier keys
        /// </summary>
        public required ModifierKey ModifierKeys { get; set; } = ModifierKey.None;

        /// <summary>
        /// Activation key
        /// </summary>
        public required string Key { get; set; }
    }
}
