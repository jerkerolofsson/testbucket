using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Domain.TestResources.Allocation;
public class NoResourceFoundException : Exception
{
    private readonly TestCaseDependency _dependency;

    public NoResourceFoundException(TestCaseDependency dependency) : base($"Resource not found: {dependency}")
    {
        _dependency = dependency;
    }
}
