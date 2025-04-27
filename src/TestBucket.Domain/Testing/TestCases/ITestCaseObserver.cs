using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestCases;
public interface ITestCaseObserver
{
    Task OnTestCreatedAsync(TestCase testCase);
    Task OnTestDeletedAsync(TestCase testCase);
    Task OnTestSavedAsync(TestCase testCase);
}
