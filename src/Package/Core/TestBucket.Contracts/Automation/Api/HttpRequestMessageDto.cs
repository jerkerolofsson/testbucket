using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Contracts.Automation.Api;
public record class HttpRequestMessageDto(string Method, string Url, HeadersCollectionDto Headers, byte[] Body)
{
    public string? RequestName { get; set; }
}
