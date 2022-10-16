using System;

namespace CockyGrabber.Utility
{
    internal static class Time
    {
        public static DateTimeOffset FromWebkitTimeMicroseconds(long ms)
        {
            if (ms > 0 && ms.ToString().Length < 18) // If the passed long is bigger than 0 and has a shorter char count than 18 (Webkit timestamps have a max. char count of 17)
            {
                DateTime dateTime = new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                ms /= 1000000;
                dateTime = dateTime.AddSeconds(ms).ToLocalTime();
                return dateTime;
            }
            return new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }
        public static DateTimeOffset FromWebkitTimeSeconds(long s)
        {
            if (s > 0) // If the passed long is a valid number (bigger than 0)
            {
                DateTime dateTime = new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(s).ToLocalTime();
                return dateTime;
            }
            return new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTimeOffset FromUnixTimeSeconds(long s) => DateTimeOffset.FromUnixTimeSeconds(s);
        public static DateTimeOffset FromUnixTimeMilliseconds(long ms) => DateTimeOffset.FromUnixTimeMilliseconds(ms);
        public static DateTimeOffset FromUnixTimeMicroseconds(long ms) => DateTimeOffset.FromUnixTimeMilliseconds(ms / 1000);
    }
}
