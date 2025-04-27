using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.TestSuites;
public interface ITestSuiteFolderObserver
{
    Task OnTestSuiteFolderCreatedAsync(TestSuiteFolder suiteFolder);
    Task OnTestSuiteFolderDeletedAsync(TestSuiteFolder suiteFolder);
    Task OnTestSuiteFolderSavedAsync(TestSuiteFolder suiteFolder);
}
