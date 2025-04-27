using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestSuites;
public interface ITestSuiteObserver
{
    Task OnTestSuiteCreatedAsync(TestSuite suite);
    Task OnTestSuiteDeletedAsync(TestSuite suite);
    Task OnTestSuiteSavedAsync(TestSuite suite);
}
