using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats;
public interface ITestRepositorySerializer
{
    /// <summary>
    /// Gets the media type for the serialized format
    /// </summary>
    public string MediaType { get; }

    ValueTask<TestRepositoryDto> DeserializeAsync(Stream source);

    ValueTask SerializeAsync(TestRepositoryDto source, Stream destination);
}
