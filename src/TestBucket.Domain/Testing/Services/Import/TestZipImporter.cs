using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Formats;

namespace TestBucket.Domain.Testing.Services.Import
{
    public class TestZipImporter
    {
        private readonly ITextTestResultsImporter _textTestResultsImporter;

        public TestZipImporter(ITextTestResultsImporter textTestResultsImporter)
        {
            _textTestResultsImporter = textTestResultsImporter;
        }


        public Task ImportZipAsync(ClaimsPrincipal principal, long teamId, long projectId, string[] globPatterns, byte[] zipContents, ImportHandlingOptions options)
        {
            using var stream = new MemoryStream(zipContents);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);


            return Task.CompletedTask;
        }
    }
}
