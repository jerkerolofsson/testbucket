using System;
using System.Globalization;

using TestBucket.Domain.Search.Models;

namespace TestBucket.Domain.Search;
public class NumericalFilterParser
{
    /// <summary>
    /// Parses a numerical filter string into a <see cref="NumericalFilter"/> instance.
    /// </summary>
    /// <param name="filterText">
    /// The filter string to parse. Must start with one of the supported operators (&gt;, &lt;, ==, &gt;=, &lt;=) 
    /// immediately followed by a culture-invariant double value (e.g., "&gt;0", "&lt;100.09", "&gt;=123", "==123").
    /// </param>
    /// <returns>
    /// A <see cref="NumericalFilter"/> representing the parsed operator and value.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="filterText"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown if the filter operator is invalid or the numeric value cannot be parsed.
    /// </exception>
    public static NumericalFilter Parse(string filterText)
    {
        if (string.IsNullOrWhiteSpace(filterText))
        {
            throw new ArgumentException("Filter text cannot be null or empty.", nameof(filterText));
        }

        FilterOperator op;
        string valuePart;

        if (filterText.StartsWith(">="))
        {
            op = FilterOperator.GreaterThanOrEqual;
            valuePart = filterText.Substring(2);
        }
        else if (filterText.StartsWith("!="))
        {
            op = FilterOperator.NotEquals;
            valuePart = filterText.Substring(2);
        }
        else if (filterText.StartsWith("<="))
        {
            op = FilterOperator.LessThanOrEqual;
            valuePart = filterText.Substring(2);
        }
        else if (filterText.StartsWith("=="))
        {
            op = FilterOperator.Equals;
            valuePart = filterText.Substring(2);
        }
        else if (filterText.StartsWith(">"))
        {
            op = FilterOperator.GreaterThan;
            valuePart = filterText.Substring(1);
        }
        else if (filterText.StartsWith("<"))
        {
            op = FilterOperator.LessThan;
            valuePart = filterText.Substring(1);
        }
        else if (filterText.StartsWith("="))
        {
            op = FilterOperator.Equals;
            valuePart = filterText.Substring(1);
        }
        else
        {
            throw new FormatException("Invalid filter operator.");
        }

        if (!double.TryParse(valuePart, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            throw new FormatException("Invalid numeric value in filter.");

        return new NumericalFilter(op, value);
    }
}