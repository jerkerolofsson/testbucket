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

    /// <summary>
    /// Invoked when the test suite is saved
    /// </summary>
    /// <param name="suite"></param>
    /// <returns></returns>
    Task OnTestSuiteSavedAsync(TestSuite suite);

    /// <summary>
    /// Invoked when the test suite is moved from one folder to another folder (or to root)
    /// </summary>
    /// <param name="suite"></param>
    /// <returns></returns>
    Task OnTestSuiteMovedAsync(TestSuite suite);
}
