using System.Linq.Expressions;

using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;

namespace TestBucket.Domain.Testing.Specifications.TestCases;

public class FilterTestCasesByAutomationImplementation : FilterSpecification<TestCase>
{
    private readonly string? _className;
    private readonly string? _method;
    private readonly string? _module;
    private readonly string? _assembly;

    public FilterTestCasesByAutomationImplementation(string? className, string? method, string? module, string? assembly)
    {
        _className = className;
        _method = method;
        _module = module;
        _assembly = assembly;
    }

    protected override Expression<Func<TestCase, bool>> GetExpression()
    {
        return x => x.ClassName == _className && x.Method == _method && x.Module == _module && x.AutomationAssembly == _assembly;
    }
}
