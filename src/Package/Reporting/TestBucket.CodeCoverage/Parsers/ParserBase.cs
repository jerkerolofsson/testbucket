using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.CodeCoverage.Parsers;
public abstract class ParserBase
{
    public async Task<CodeCoverageReport> ParseTextAsync(string xml, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var report = new CodeCoverageReport();
        return await ParseStreamAsync(report, stream, cancellationToken);
    }

    public async Task<CodeCoverageReport> ParseFileAsync(string path, CancellationToken cancellationToken)
    {
        var report = new CodeCoverageReport();
        return await ParseFileAsync(report, path, cancellationToken);
    }

    public async Task<CodeCoverageReport> ParseFileAsync(CodeCoverageReport report, string path, CancellationToken cancellationToken)
    {
        using var stream = File.OpenRead(path);
        await ParseStreamAsync(report, stream, cancellationToken);
        return report;
    }

    public abstract Task<CodeCoverageReport> ParseStreamAsync(CodeCoverageReport report, Stream stream, CancellationToken cancellationToken);
}
