using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotNet.Globbing;

using TestBucket.Domain.Code;

namespace TestBucket.Domain.Testing.Services.Import
{
    public static class ZipGlobExtensions
    {
        /// <summary>
        /// Scans the zip file for matching entries to the glob patterns
        /// </summary>
        /// <param name="globPatterns"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
        public static IEnumerable<ZipArchiveEntry> GlobFind(this ZipArchive archive, string[] globPatterns)
        {
            foreach (var zipEntry in archive.Entries)
            {
                if(GLobMatcher.IsMatch(zipEntry.FullName, globPatterns))
                {
                    yield return zipEntry;
                }
            }
        }
    }
}
