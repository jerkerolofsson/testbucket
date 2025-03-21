using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing;

public interface ITestRunObserver
{
    Task OnRunCreatedAsync(TestRun testRun);
    Task OnRunDeletedAsync(TestRun testRun);
}
