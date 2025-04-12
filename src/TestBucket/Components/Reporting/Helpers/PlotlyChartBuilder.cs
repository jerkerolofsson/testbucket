using Plotly.Blazor.Traces;

using TestBucket.Domain.Testing.Aggregates;
using Plotly.Blazor;
using TestBucket.Components.Shared.Themeing;
using Plotly.Blazor.LayoutLib;

namespace TestBucket.Components.Reporting.Helpers;

public class PlotlyChartBuilder
{
    public static void BuildPassrateCategoryBarData(IList<ITrace> data, IEnumerable<string> categories, string result, IEnumerable<TestExecutionResultSummary> source)
    {
        var bar = new Bar
        {
            Name = result,
            X = new List<object>(categories),
            Y = new List<object>(),

        };
        data.Add(bar);

        foreach (var categoryResult in source)
        {
            var sum = result switch
            {
                "passed" => categoryResult.Passed,
                "failed" => categoryResult.Failed,
                "blocked" => categoryResult.Blocked,
                "skipped" => categoryResult.Skipped,
                "norun" => categoryResult.NoRun,
                _ => 0
            };
            bar.Y.Add(sum);
        }
    }

    public static readonly string[] ResultLabels = ["passed", "failed", "blocked", "skipped"];

    public static Pie CreateResultPie(TestExecutionResultSummary result, string name)
    {
        var bar = new Pie
        {
            AutoMargin = true,
            Name = name,
            Labels = new List<object>(ResultLabels),
            Values = [result.Passed, result.Failed, result.Blocked, result.Skipped, result.NoRun],
        };
        return bar;
    }
    public static Bar CreateResultBar(TestExecutionResultSummary result, string name)
    {
        var bar = new Bar
        {
            Name = name,
            X = new List<object>(ResultLabels),
            Y = [result.Passed, result.Failed, result.Blocked, result.Skipped, result.NoRun],
        };

        return bar;
    }

    public static Plotly.Blazor.Layout GetDefaultLayout(bool isDarkMode)
    {
        return new Plotly.Blazor.Layout()
        {
            BarCornerRadius = 5,
            PaperBgColor = "transparent",
            PlotBgColor = "transparent",
            Font = new Font
            {
                Color = isDarkMode ? "#eee" : "#111",
            }
        };
    }

}
