using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models
{
    public record class SettingsCategory
    {
        public required string Name { get; set; }
        public SettingIcon Icon { get; set; } = SettingIcon.Default;

    }
}
