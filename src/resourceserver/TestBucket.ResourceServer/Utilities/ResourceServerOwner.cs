namespace TestBucket.ResourceServer.Utilities;
public class ResourceServerOwner
{
    public static string Name => Environment.GetEnvironmentVariable("TB_SERVER_UUID") ?? "no-uuid-configured";
}
