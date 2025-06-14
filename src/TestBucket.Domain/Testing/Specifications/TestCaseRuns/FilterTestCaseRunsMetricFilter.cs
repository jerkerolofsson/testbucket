using System.Linq.Expressions;
using TestBucket.Domain.Search.Models;
using TestBucket.Domain.Shared.Specifications;
using TestBucket.Domain.Testing.Models;
using TestBucket.Domain.Search;

namespace TestBucket.Domain.Testing.Specifications.TestCaseRuns;

public class FilterTestCaseRunsMetricFilter : FilterSpecification<TestCaseRun>
{
    private readonly string _metricName;
    private readonly FilterOperator? _operator;
    private readonly double? _value;

    public FilterTestCaseRunsMetricFilter(string metricFilterText)
    {
        // Find the first operator in the string
        var operatorIndex = metricFilterText.IndexOfAny(new[] { '>', '<', '=', '!' });
        if (operatorIndex > 0)
        {
            _metricName = metricFilterText.Substring(0, operatorIndex);
            var filterPart = metricFilterText.Substring(operatorIndex);

            try
            {
                var filter = NumericalFilterParser.Parse(filterPart);
                _operator = filter.Operator;
                _value = filter.Value;
            }
            catch
            {
                // Fallback: treat as just a metric name if parsing fails
                _metricName = metricFilterText;
                _operator = null;
                _value = null;
            }
        }
        else
        {
            _metricName = metricFilterText;
            _operator = null;
            _value = null;
        }
    }

    protected override Expression<Func<TestCaseRun, bool>> GetExpression()
    {
        if (_operator is not null && _value is not null)
        {
            return _operator switch
            {
                FilterOperator.Equals =>
                    x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName && m.Value == _value),
                FilterOperator.NotEquals =>
                    x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName && m.Value != _value),
                FilterOperator.GreaterThan =>
                    x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName && m.Value > _value),
                FilterOperator.GreaterThanOrEqual =>
                    x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName && m.Value >= _value),
                FilterOperator.LessThan =>
                    x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName && m.Value < _value),
                FilterOperator.LessThanOrEqual =>
                    x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName && m.Value <= _value),
                _ =>
                    x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName)
            };
        }

        return x => x.Metrics != null && x.Metrics.Any(m => m.Name == _metricName);
    }
}