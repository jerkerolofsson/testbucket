using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Requirements.Types;

namespace TestBucket.Contracts.Testing.States;
public class TestCaseRunStates
{
    public const string NotStarted = nameof(MappedTestState.NotStarted);
    public const string Assigned = nameof(MappedTestState.Assigned);
    public const string Ongoing = nameof(MappedTestState.Ongoing);
    public const string Completed = nameof(MappedTestState.Completed);
}
