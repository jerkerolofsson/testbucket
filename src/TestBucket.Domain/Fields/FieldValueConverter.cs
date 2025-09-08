using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Domain.Fields.Models;

namespace TestBucket.Domain.Fields;
public static class FieldValueConverter
{
    public static bool TryAssignValue(FieldDefinition fieldDefinition, FieldValue fieldValue, string[] values)
    {
        if (values.Length == 0)
        {
            return false;
        }
        if(fieldDefinition.Type != FieldType.String && fieldDefinition.Type != FieldType.StringArray) fieldValue.StringValue = null;
        if (fieldDefinition.Type != FieldType.Double) fieldValue.DoubleValue = null;
        if (fieldDefinition.Type != FieldType.Integer) fieldValue.LongValue = null;
        if (fieldDefinition.Type != FieldType.Boolean) fieldValue.BooleanValue = null;
        if (fieldDefinition.Type != FieldType.DateOnly) fieldValue.DateValue = null;
        if (fieldDefinition.Type != FieldType.TimeSpan) fieldValue.TimeSpanValue = null;
        if (fieldDefinition.Type != FieldType.DateTimeOffset) fieldValue.DateTimeOffsetValue = null;
        fieldValue.StringValuesList = null;

        switch (fieldDefinition.Type)
        {
            case FieldType.Boolean:
                var booleanValue = values[0] == "true";
                if (fieldValue.BooleanValue != booleanValue)
                {
                    fieldValue.BooleanValue = booleanValue;
                    return true;
                }
                return false;

            case FieldType.Integer:
                if (long.TryParse(values[0], out long longValue) && longValue != fieldValue.LongValue)
                {
                    fieldValue.LongValue = longValue;
                    return true;
                }
                break;

            case FieldType.TimeSpan:
                if (TimeSpan.TryParse(values[0], out var timespan) && fieldValue.TimeSpanValue != timespan)
                {
                    fieldValue.TimeSpanValue = timespan;
                    return true;
                }
                break;

            case FieldType.DateTimeOffset:
                if (DateTimeOffset.TryParse(values[0], out var dateTimeOffset) && fieldValue.DateTimeOffsetValue != dateTimeOffset)
                {
                    fieldValue.DateTimeOffsetValue = dateTimeOffset;
                    return true;
                }
                break;

            case FieldType.DateOnly:
                if (DateOnly.TryParse(values[0], out var date) && fieldValue.DateValue != date)
                {
                    fieldValue.DateValue = date;
                    return true;
                }
                break;
            case FieldType.Double:
                if (double.TryParse(values[0], CultureInfo.InvariantCulture, out double doubleValue) && fieldValue.DoubleValue != doubleValue)
                {
                    fieldValue.DoubleValue = doubleValue;
                    return true;
                }
                break;

            case FieldType.MultiSelection:
            case FieldType.StringArray:
                fieldValue.StringValuesList = values.ToList();
                return true;

            case FieldType.SingleSelection:
            case FieldType.String:
                if (fieldValue.StringValue != values[0])
                {
                    fieldValue.StringValue = values[0];
                    return true;
                }
                return false;
        }

        return false;
    }
}
