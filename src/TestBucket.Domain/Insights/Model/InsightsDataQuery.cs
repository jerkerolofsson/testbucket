﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Domain.Insights.Model;

/// <summary>
/// A query that contains a filter and a specific data source as identified by the DataSource property
/// </summary>
public class InsightsDataQuery
{
    /// <summary>
    /// Identifier for the data source
    /// </summary>
    public required string DataSource { get; set; }

    /// <summary>
    /// A query that can be parsed to filter the data
    /// </summary>
    public string Query { get; set; } = "";

    /// <summary>
    /// Colors for the data series/labels created from this query
    /// </summary>
    public Dictionary<string, string>? Colors { get; set; }
}
