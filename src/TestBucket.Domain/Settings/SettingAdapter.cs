using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields;
using TestBucket.Domain.Settings.Models;

namespace TestBucket.Domain.Settings
{
    public interface ISetting
    {
        /// <summary>
        /// Metadata about the setting. This is compatible with Field so we can use field editor to 
        /// edit settings.
        /// </summary>
        Setting Metadata { get; }
        Task WriteAsync(SettingContext context, FieldValue value);
        Task<FieldValue> ReadAsync(SettingContext context);
    }

    /// <summary>
    /// Base class to implement a setting
    /// </summary>
    public abstract class SettingAdapter : ISetting
    {
        public Setting Metadata { get; } = new Setting
        {
            Category = new SettingsCategory { Name = "General" },
            Section = new SettingsSection { Name = "Common" },
            Name = "Undefined",
            Type = FieldType.String
        };

        /// <summary>
        /// Main category
        /// </summary>
        public string Category { get; set; } = "General";
        public string Section { get; set; } = "Common";

        /// <summary>
        /// Reads a value from a setting
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task<FieldValue> ReadAsync(SettingContext context);

        /// <summary>
        /// Writes a value to the settings
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract Task WriteAsync(SettingContext context, FieldValue value);
    }
}
