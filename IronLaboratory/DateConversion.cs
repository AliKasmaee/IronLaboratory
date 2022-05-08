using System;
using System.Globalization;

namespace IronLaboratory
{
    public static class DateConversion
    {
        public static string ConvertToPersian(DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", pc.GetYear(dateTime), pc.GetMonth(dateTime), pc.GetDayOfMonth(dateTime));
        }

        public static string ConvertToGregorian(string dateTime)
        {
            //date like: 1400/10/10
            var value = dateTime.Split('/');
            int y = Convert.ToInt32(value[0]);
            int m = Convert.ToInt32(value[1]);
            int d = Convert.ToInt32(value[2]);

            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(y, m, d, pc);
            return Convert.ToString(dt.Date);
        }

        public static string TimeConversion(string dateTime)
        {
            return DateTime.Parse(dateTime).ToString("H:mm");
        }

        public static string TimeWithSecondsConversion(string dateTime)
        {
            return DateTime.Parse(dateTime).ToString("H:mm:ss");
        }
    }
}
