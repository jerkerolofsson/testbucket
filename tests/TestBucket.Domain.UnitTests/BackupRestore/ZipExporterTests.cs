using TestBucket.Domain.Export.Zip;

namespace TestBucket.Domain.UnitTests.BackupRestore;

/// <summary>
/// Tests for zip import and zip export
/// </summary>
[Component("Backup")]
[UnitTest]
[EnrichedTest]
[FunctionalTest]
public class ZipExporterTests
{
    /// <summary>
    /// Verfifies that after writing a single entity, the entity source,type and ID are correct
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task WriteEntityAsync_ToZip_PathAccordingToConventions()
    {
        using var zipStream = new MemoryStream();
        byte[] data = [1, 2, 3];
        using var entity = new MemoryStream(data);

        // Act
        using (var zipExporter = new ZipExporter(zipStream))
        {
            await zipExporter.WriteEntityAsync("SourceName", "TheEntityType", "TheEntityId", entity, TestContext.Current.CancellationToken);
        }

        // Assert
        using (var zip = new ZipImporter(zipStream))
        {
            var entityCount = zip.ReadAll().Where(x => x.Source == "SourceName" && x.Type == "TheEntityType" && x.Id == "TheEntityId").Count();
            Assert.Equal(1, entityCount);
        }
    }

    /// <summary>
    /// Verfifies that after writing two entries with different ID but same source and type, both entities are added
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task WriteEntityAsync_ToZip_WithTwoItemsOfSameTypeAndSource_BothItemsAdded()
    {
        using var zipStream = new MemoryStream();
        byte[] data = [1, 2, 3];
        using var entity = new MemoryStream(data);

        // Act
        using (var zipExporter = new ZipExporter(zipStream))
        {
            await zipExporter.WriteEntityAsync("SourceName", "TheEntityType", "1", entity, TestContext.Current.CancellationToken);
            await zipExporter.WriteEntityAsync("SourceName", "TheEntityType", "2", entity, TestContext.Current.CancellationToken);
        }

        // Assert
        using (var zip = new ZipImporter(zipStream))
        {
            Assert.Equal(2, zip.ReadAll().Count());

            var entities = zip.ReadAll().Where(x => x.Source == "SourceName" && x.Type == "TheEntityType").ToList();
            IEnumerable<string> ids = entities.Select(x=>x.Id).AsEnumerable();
            Assert.Equal(2, entities.Count);
            Assert.Contains("1", ids);
            Assert.Contains("2", ids);
        }
    }

    /// <summary>
    /// Verfifies that after writing two entries with different ID and type but same source, both entities are added
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task WriteEntityAsync_ToZip_WithTwoItemsOfSDifferentType_BothItemsAdded()
    {
        using var zipStream = new MemoryStream();
        byte[] data = [1, 2, 3];
        using var entity = new MemoryStream(data);

        // Act
        using (var zipExporter = new ZipExporter(zipStream))
        {
            await zipExporter.WriteEntityAsync("SourceName", "A", "1", entity, TestContext.Current.CancellationToken);
            await zipExporter.WriteEntityAsync("SourceName", "B", "2", entity, TestContext.Current.CancellationToken);
        }

        // Assert
        using (var zip = new ZipImporter(zipStream))
        {
            Assert.Equal(2, zip.ReadAll().Count());

            var aCount = zip.ReadAll().Where(x => x.Source == "SourceName" && x.Type == "A" && x.Id == "1").Count();
            var bCount = zip.ReadAll().Where(x => x.Source == "SourceName" && x.Type == "B" && x.Id == "2").Count();
            Assert.Equal(1, aCount);
            Assert.Equal(1, bCount);
        }
    }
    /// <summary>
    /// Verfifies that after writing two entries with different ID, type and source, both entities are added
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task WriteEntityAsync_ToZip_WithTwoItemsOfSDifferenSourcetTypeId_BothItemsAdded()
    {
        using var zipStream = new MemoryStream();
        byte[] data = [1, 2, 3];
        using var entity = new MemoryStream(data);

        // Act
        using (var zipExporter = new ZipExporter(zipStream))
        {
            await zipExporter.WriteEntityAsync("S1", "A", "1", entity, TestContext.Current.CancellationToken);
            await zipExporter.WriteEntityAsync("S2", "B", "2", entity, TestContext.Current.CancellationToken);
        }

        // Assert
        using (var zip = new ZipImporter(zipStream))
        {
            Assert.Equal(2, zip.ReadAll().Count());

            var aCount = zip.ReadAll().Where(x => x.Source == "S1" && x.Type == "A" && x.Id == "1").Count();
            var bCount = zip.ReadAll().Where(x => x.Source == "S2" && x.Type == "B" && x.Id == "2").Count();
            Assert.Equal(1, aCount);
            Assert.Equal(1, bCount);
        }
    }
}
