using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBucket.Domain.Files.Models;

namespace TestBucket.Domain.UnitTests.TestHelpers
{
    class FileResourceCreator
    {
        public static FileResource CreateText(string name, string text) => new FileResource() { ContentType = "text/plain", Data = Encoding.UTF8.GetBytes(text), TenantId = "", Name = name };
    }
}
