using System;
using System.Globalization;

namespace Suburb.Utils
{
    public class DateTimeUtils
    {
        public const string DEFAULT_DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string DETAIL_DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss-fff";
        public const string SHORT_DATE_TIME_FORMAT = "dd.MM.yyyy HH:mm";

        public static string GetNow(string dateFormat = null)
        {
            dateFormat = dateFormat ?? DEFAULT_DATE_TIME_FORMAT;
            return DateTime.Now.ToString(dateFormat);
        }

        public static string GetDetailNow()
        {
            return DateTime.Now.ToString(DETAIL_DATE_TIME_FORMAT);
        }

        public static string ParseAndFormat(string dateString, string parseFormat = null, string dateFormat = null)
        {
            parseFormat ??= DEFAULT_DATE_TIME_FORMAT;
            dateFormat ??= DEFAULT_DATE_TIME_FORMAT;

            if (DateTime.TryParseExact(dateString, parseFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                return date.ToString(dateFormat);

            return null;
        }
    }
}
