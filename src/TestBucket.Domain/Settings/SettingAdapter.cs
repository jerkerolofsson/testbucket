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
        Setting Metadata { get; }

        Task WriteAsync(ClaimsPrincipal principal, FieldValue value);
        Task<FieldValue> ReadAsync(ClaimsPrincipal principal);
    }

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

        public abstract Task<FieldValue> ReadAsync(ClaimsPrincipal principal);
        public abstract Task WriteAsync(ClaimsPrincipal principal, FieldValue value);
    }
}
