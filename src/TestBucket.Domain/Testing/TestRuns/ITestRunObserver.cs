using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestRuns;

public interface ITestRunObserver
{
    Task OnRunCreatedAsync(TestRun testRun);
    Task OnRunUpdatedAsync(TestRun testRun);
    Task OnRunDeletedAsync(TestRun testRun);

    Task OnTestCaseRunCreatedAsync(TestCaseRun testCaseRun);
    Task OnTestCaseRunUpdatedAsync(TestCaseRun testCaseRun);
}
