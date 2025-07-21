using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.AI.Agent;
internal class SemanticKernelToolNaming
{
    /// <summary>
    /// Returns a valid name for a semantic kernel plugin, replacing invalid characters with underscore.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetSemanticKernelPluginName(string name)
    {
        var sb = new StringBuilder();
        foreach(var nameChar in name)
        {
            if(Char.IsAsciiLetter(nameChar) || Char.IsAsciiDigit(nameChar) || nameChar == '_')
            {
                sb.Append(nameChar);
            }
            else
            {
                sb.Append('_');
            }
        }

        return sb.ToString();
    }
}
