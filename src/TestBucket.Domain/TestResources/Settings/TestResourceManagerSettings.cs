namespace TestBucket.Domain.TestResources.Settings;
public class TestResourceManagerSettings
{
    public TimeSpan DeleteResourceIfNotSeenFor { get; set; } = TimeSpan.FromDays(2);
}
