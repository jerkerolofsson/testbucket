using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.Export.Models
{
    public class ExportOptions
    {
        /// <summary>
        /// The format for the export
        /// </summary>
        public ExportFormat ExportFormat { get; set; } = ExportFormat.Zip;

        /// <summary>
        /// What type of system the backup will be written to
        /// </summary>
        public ExportDestinationType DestinationType { get; set; } = ExportDestinationType.Disk;

        /// <summary>
        /// Path / URI dependent on DestinationType
        /// </summary>
        public string? Destination { get; set; }
    }
}
