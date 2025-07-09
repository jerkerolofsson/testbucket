using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace TestBucket.Blazor.Helpers;

/// <summary>
/// Returns the printable representation of a (javascript) key.
/// </summary>
public class KeyPresenter
{
    public static string ToPrintable(string key)
    {
        if (key.StartsWith("Key"))
        {
            key = key.Replace("Key", "");
        }

        switch (key)
        {
            case "Slash":
                return "/";
        }

        return key;
    }
}
