using TestBucket.Contracts.TestResources;

namespace TestBucket.Servers.NodeResourceServer.Models;

public class NodeService
{
    public required TestResourceDto Description { get; set; }

    public required int Port { get; set; }
}
