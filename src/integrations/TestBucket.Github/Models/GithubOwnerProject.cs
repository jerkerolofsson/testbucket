using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Github.Models;
internal record class GithubOwnerProject(string Owner, string Project)
{
    public static GithubOwnerProject Parse(string? text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var orgAndProject = text.Split('/', StringSplitOptions.TrimEntries);
        if (orgAndProject.Length != 2)
        {
            throw new ArgumentException("Expected project ID to be in the format organization/project");
        }

        return new GithubOwnerProject(orgAndProject[0], orgAndProject[1]);

    }
}
