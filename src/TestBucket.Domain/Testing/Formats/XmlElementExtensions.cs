using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Brightest.Testing.Domain.Formats.Xml
{
    public static class XmlElementExtensions
    {
        public static void SetAttribute(this XmlElement node, string attributeName, TimeSpan val)
        {
            var text = val.ToString();
            node.SetAttribute(attributeName, text);
        }
        public static void SetAttribute(this XmlElement node, string attributeName, DateTime val)
        {
            var text = val.ToString("o");
            node.SetAttribute(attributeName, text);
        }
        public static DateTime GetDateTimeAttribute(this XmlElement node, string attributeName)
        {
            var val = node.GetAttribute(attributeName);
            if (val != null)
            {
                return XmlConvert.ToDateTime(val, XmlDateTimeSerializationMode.Unspecified);
            }
            return DateTime.MinValue;
        }
        public static TimeSpan GetTimeSpanAttribute(this XmlElement node, string attributeName)
        {
            var val = node.GetAttribute(attributeName);
            if (val != null)
            {
                return XmlConvert.ToTimeSpan(val);
            }
            return TimeSpan.MinValue;
        }
        public static double GetDoubleAttribute(this XmlElement node, string attributeName)
        {
            var numberFormat = new NumberFormatInfo();
            numberFormat.NumberDecimalSeparator = ".";
            numberFormat.NumberGroupSeparator = "";
            var val = node.GetAttribute(attributeName);
            if (val != null)
            {
                if (double.TryParse(val, NumberStyles.AllowDecimalPoint, numberFormat, out double doubleVal))
                {
                    return doubleVal;
                }
            }
            return 0;
        }

    }
}
