using System.Security.Claims;

using TestBucket.Domain.Identity.Permissions;
using TestBucket.Domain.Keyboard;

namespace TestBucket.Domain.Commands
{
    /// <summary>
    /// Commands can be triggered by the UI, or sometimes by an API
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Id of the command
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of the command. Localized
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the command. Localized
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Returns true if the command is enabled and can be executed
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// The default keyboard binding
        /// </summary>
        public KeyboardBinding? DefaultKeyboardBinding { get; }

        /// <summary>
        /// Icon
        /// </summary>
        public string? Icon { get; }

        /// <summary>
        /// This will be shown in the context menu for the applicable types
        /// </summary>
        public string[] ContextMenuTypes { get; }

        /// <summary>
        /// The entity type that requires the required permission level
        /// This is only used for UI validation, the actual check is done in domain on the manager or request handler
        /// </summary>
        public PermissionEntityType? PermissionEntityType { get; }

        /// <summary>
        /// Required permission level for the command
        /// This is only used for UI validation, the actual check is done in domain on the manager or request handler
        /// </summary>
        public PermissionLevel? RequiredLevel { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns></returns>
        ValueTask ExecuteAsync();
    }
}
