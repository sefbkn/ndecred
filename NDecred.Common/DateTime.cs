using System;

namespace NDecred.Common
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime dt)
        {
            return ((DateTimeOffset) dt).ToUnixTimeSeconds();
        }

        public static DateTime FromUnixTime(long unixTimeSeconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds).UtcDateTime;
        }
    }
}