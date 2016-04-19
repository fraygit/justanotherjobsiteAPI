using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jajs.API
{
    public static class Helper
    {
        public static DateTime SetDateForMongo(DateTime datetime)
        {
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        }

        public static DateTime ConvertTimeZone(DateTime date)
        {
            TimeZoneInfo nzSTZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(date, nzSTZone);
        }
    }
}
