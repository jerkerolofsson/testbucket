namespace TestBucket.Components.Environments.ViewModels;

public static class EnvironmentVariablesMapping
{
    public static List<EnvironmentVariable> ToEnvironmentVariables(this Dictionary<string, string> vars) => vars.Select(kv => new EnvironmentVariable(kv.Key, kv.Value)).ToList();

    public static Dictionary<string,string> ToDictionary(this List<EnvironmentVariable> vars)
    {
        var result = new Dictionary<string, string>();
        foreach(var var in vars)
        {
            result[var.Key] = var.Value;
        }
        return result;
    }

}
