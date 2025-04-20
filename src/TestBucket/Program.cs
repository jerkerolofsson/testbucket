namespace TestBucket;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.CreateApp();
        app.Run();
    }
}
