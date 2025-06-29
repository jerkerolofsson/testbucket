using TestBucket.Contracts.Insights;
using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights.Extensions;
public static class InsightsTableExtensions
{
    public static InsightsTable ToTable<T,U>(this InsightsVisualizationSpecification specification, InsightsData<T,U> data) where T : notnull
    {
        var table = new InsightsTable();

        if(data.Series.Count > 0)
        {
            // Get labels for each row
            var labels = data.Series.First().Labels;

            table.AddHeader("");
            foreach(var series in data.Series)
            {
                table.AddHeader(series.Name);
            }

            // Accessing series.Values will order the series, so we cache it in memory
            Dictionary<InsightsSeries<T, U>, List<U>> valueMap = [];
            foreach(var series in data.Series)
            {
                valueMap[series] = series.Values.ToList();
            }

            // Labels define the rows, so loop over those first
            foreach(var labelIndex in labels.Index())
            {
                // Allocate the row (we already know the capacity)
                List<string> row = table.AddRow(data.Series.Count + 1);
                row.Add(labelIndex.Item.ToString()!);
                foreach (var series in data.Series)
                {
                    var values = valueMap[series];
                    if (values.Count > labelIndex.Index)
                    {
                        var value = values[labelIndex.Index];
                        var valueAsString = value?.ToString() ?? "";
                        row.Add(valueAsString);
                    }
                    else
                    {
                        row.Add("");
                    }
                }
            }
        }

        return table;
    }
}
