﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Metrics.Models;

namespace TestBucket.Domain.Metrics;
public interface IMetricsManager
{
    /// <summary>
    /// Adds a metric
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="metric"></param>
    /// <returns></returns>
    Task AddMetricAsync(ClaimsPrincipal principal, Metric metric);

    /// <summary>
    /// Deletes a metric
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="metric"></param>
    /// <returns></returns>
    Task DeleteMetricAsync(ClaimsPrincipal principal, Metric metric);
}
