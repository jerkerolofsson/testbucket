using TestBucket.Domain.Insights.Model;

namespace TestBucket.Domain.Insights.Extensions;

internal static class InsightsDataPointExtensions
{
    public static string GetLabelAsSortableString<T,U>(this InsightsDataPoint<T,U> point)
    {
        if(point.OriginalLabel is DateOnly dateOnly)
        {
            return dateOnly.ToString("yyyy-MM-dd");
        }
        if (point.OriginalLabel is DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
        }
        if (point.OriginalLabel is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
        }

        return point.Label?.ToString() ?? "";
    }

}
