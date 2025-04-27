using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Commands.Models;
public class CommandContextMenuItem
{
    public ICommand? Command { get; set; }

    /// <summary>
    /// Name of folder
    /// </summary>
    public string? Folder { get; set; }

    /// <summary>
    /// If a folder, contains the commands
    /// </summary>
    public List<ICommand> FolderCommands { get; set; } = [];
}
