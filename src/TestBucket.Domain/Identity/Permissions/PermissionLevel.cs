using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Identity.Permissions
{
    [Flags]
    public enum PermissionLevel
    {
        None = 0,

        /// <summary>
        /// Read items
        /// </summary>
        Read = 1,

        /// <summary>
        /// Add/Edit
        /// </summary>
        Write = 2,

        /// <summary>
        /// Can delete
        /// </summary>
        Delete = 4,

        All = Delete | Write | Read
    }
}
