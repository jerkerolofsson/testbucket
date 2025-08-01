using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Core;

namespace TestBucket.Traits.MSTest;
public class CommitAttribute : TestPropertyAttribute
{
    public CommitAttribute(string sha) : base(TargetTraitNames.Commit, sha)
    {
    }
}
