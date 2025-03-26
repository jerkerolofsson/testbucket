using System.Security.Claims;

using TestBucket.Domain.Keyboard;

namespace TestBucket.Domain.Commands
{
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
        public KeyboardBinding DefaultKeyboardBinding { get; }

        /// <summary>
        /// Icon
        /// </summary>
        public string? Icon { get; }

        /// <summary>
        /// This will be shown in the context menu for the applicable types
        /// </summary>
        public string[] ContextMenuTypes { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns></returns>
        ValueTask ExecuteAsync();
    }
}
