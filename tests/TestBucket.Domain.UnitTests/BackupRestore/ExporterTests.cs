using System.Security.Claims;

using Mediator;

using NSubstitute;

using TestBucket.Domain.Export;
using TestBucket.Domain.Export.Models;
using TestBucket.Domain.Export.Services;
using TestBucket.Domain.Identity;
using TestBucket.Domain.Progress;

namespace TestBucket.Domain.UnitTests.BackupRestore;

/// <summary>
/// Unit tests for the <see cref="Exporter"/> class, covering export functionality and notifications.
/// </summary>
[Component("Backup")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class ExporterTests
{
    /// <summary>
    /// Substitute mediator used for verifying published events and interactions.
    /// </summary>
    private readonly IMediator _mediator = NSubstitute.Substitute.For<IMediator>();

    /// <summary>
    /// Verifies that Exporter.ExportFullAsync throws NotSupportedException
    /// when an unsupported export format is specified in ExportOptions
    /// </summary>
    [Fact]
    public async Task ExportFullAsync_ThrowsForUnsupportedFormat()
    {
        var exporter = new Exporter(_mediator);
        var options = new ExportOptions { ExportFormat = (ExportFormat)999 };
        var stream = new MemoryStream();
        var progressTask = new ProgressTask("", Substitute.For<IProgressManager>());

        var principal = Impersonation.Impersonate("tenant1");

        await Assert.ThrowsAsync<NotSupportedException>(() =>
            exporter.ExportFullAsync(new ClaimsPrincipal(), options, "tenant1", stream, progressTask));
    }

    /// <summary>
    /// Verifies that Exporter.ExportFullAsync publishes an ExportNotification event after a successful export operation.
    /// </summary>
    [Fact]
    public async Task ExportFullAsync_PublishesExportNotification()
    {
        var principal = Impersonation.Impersonate("tenant1");

        var exporter = new Exporter(_mediator);
        var options = new ExportOptions();
        var stream = new MemoryStream();
        var progressTask = new ProgressTask("", Substitute.For<IProgressManager>());

        await exporter.ExportFullAsync(principal, options, "tenant1", stream, progressTask);

        await _mediator.Received().Publish(
            Arg.Is<Domain.Export.Events.ExportNotification>(n =>
                n.TenantId == "tenant1" &&
                n.Options == options &&
                n.progressTask == progressTask
            ), Arg.Any<CancellationToken>()
        );
    }

    /// <summary>
    /// Verifies that Exporter.ExportFullAsync writes the tenant entity
    /// when the provided filter allows it.
    /// </summary>
    [Fact]
    public async Task ExportFullAsync_WritesTenantEntity_WhenFilterAllows()
    {
        var principal = Impersonation.Impersonate("tenant1");

        var exporter = new Exporter(_mediator);
        var options = new ExportOptions
        {
            Filter = entity => true
        };
        var sink = NSubstitute.Substitute.For<IDataExporterSink>();
        var progressTask = new ProgressTask("", Substitute.For<IProgressManager>());

        await exporter.ExportFullAsync(principal, options, "tenant1", progressTask, sink);

        await sink.Received(1).WriteEntityAsync(
            "exporter",
            "tenant",
            "tenant1",
            Arg.Any<Stream>(),
            Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<Domain.Export.Events.ExportNotification>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that Exporter.ExportFullAsync does not write the tenant entity
    /// when the provided filter does not allow it
    /// </summary>
    [Fact]
    public async Task ExportFullAsync_DoesNotWriteTenantEntity_WhenFilterDoesNotAllow()
    {
        var principal = Impersonation.Impersonate("tenant1");

        var exporter = new Exporter(_mediator);
        var options = new ExportOptions
        {
            Filter = entity => false
        };
        var sink = NSubstitute.Substitute.For<IDataExporterSink>();
        var progressTask = new ProgressTask("", Substitute.For<IProgressManager>());

        await exporter.ExportFullAsync(principal, options, "tenant1", progressTask, sink);

        await sink.DidNotReceive().WriteEntityAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<Domain.Export.Events.ExportNotification>(), Arg.Any<CancellationToken>());
    }
}