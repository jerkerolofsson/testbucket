using TestBucket.Domain.Metrics.Models;

namespace TestBucket.Domain.Metrics;
public interface IMetricsRepository
{
    /// <summary>
    /// Adds a metric
    /// </summary>
    /// <param name="metric"></param>
    /// <returns></returns>
    Task AddMetric(Metric metric);

    /// <summary>
    /// Deletes a metric
    /// </summary>
    /// <param name="metric"></param>
    /// <returns></returns>
    Task DeleteMetric(Metric metric);
}
