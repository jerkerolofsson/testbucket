using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Export.Models
{
    public enum ExportDestinationType
    {
        /// <summary>
        /// Write backup to disk
        /// </summary>
        Disk,

        /// <summary>
        /// Write backup to S3
        /// </summary>
        S3,

        /// <summary>
        /// Write into memory
        /// </summary>
        Memory
    }
}
