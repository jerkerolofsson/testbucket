using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models
{
    public class Setting : FieldDefinition
    {
        public required SettingsCategory Category { get; set; }
        public required SettingsSection Section { get; set; }
    }
}
