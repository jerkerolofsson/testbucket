using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Settings.Models
{
    public record class SettingsSection
    {
        public required string Name { get; set; }

        /// <summary>
        /// SVG  Icon
        /// </summary>
        public string? Icon { get; set; }

    }
}
