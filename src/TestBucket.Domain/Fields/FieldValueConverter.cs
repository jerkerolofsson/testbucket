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

        switch(fieldDefinition.Type)
        {
            case FieldType.Boolean:
                fieldValue.BooleanValue = values[0] == "true";
                return true;

            case FieldType.Integer:
                if (long.TryParse(values[0], out long longValue))
                {
                    fieldValue.LongValue = longValue;
                    return true;
                }
                break;

            case FieldType.Double:
                if (double.TryParse(values[0], CultureInfo.InvariantCulture, out double doubleValue))
                {
                    fieldValue.DoubleValue = doubleValue;
                    return true;
                }
                break;

            case FieldType.StringArray:
                fieldValue.StringArrayValue = values;
                break;

            case FieldType.SingleSelection:
            case FieldType.String:
                fieldValue.StringValue = values[0];
                return true;
        }

        return false;
    }
}
