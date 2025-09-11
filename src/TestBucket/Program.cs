namespace TestBucket;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = await builder.CreateApp();
        app.Run();
    }
}
