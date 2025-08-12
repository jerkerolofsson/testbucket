using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.Appium;
internal record class PackageNameEmbedding(ReadOnlyMemory<float> Embedding, string PackageName)
{
}
