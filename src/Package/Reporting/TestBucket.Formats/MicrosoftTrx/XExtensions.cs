using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Formats.MicrosoftTrx
{
    /// <summary>
    /// Xml parsing e xtensions
    /// </summary>
    internal static class XExtensions
    {
        /// <summary>
        /// Returns null or a DateTimeOffset corresponding to the element.Attribute(name)?.Value
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTimeOffset? XAttributeDateTimeOffset(this XElement element, string name)
        {
            var value = element.Attribute(name)?.Value;
            if (value is not null && DateTimeOffset.TryParse(value, out DateTimeOffset date))
            {
                return date;
            }
            return null;
        }

        /// <summary>
        /// Returns null or a TimeSpan corresponding to the element.Attribute(name)?.Value
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TimeSpan? XAttributeTimeSpan(this XElement element, string name)
        {
            var value = element.Attribute(name)?.Value;
            if (value is not null && TimeSpan.TryParse(value, out TimeSpan date))
            {
                return date;
            }
            return null;
        }
    }
}
