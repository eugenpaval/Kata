using System.Linq;

namespace Kata
{
    public class HumanTimeFormat
    {
        private const int Minute = 60;
        private const int Hour = Minute * 60;
        private const int Day = Hour * 24;
        private const int Year = Day * 365;

        public static string formatDuration(int seconds)
        {
            if (seconds == 0)
                return "now";

            string plural(int x)
            {
                return x > 1 ? "s" : null;
            }

            string output(int x, string s)
            {
                return x > 0 ? $"{x} {s}{plural(x)}" : null;
            }

            var y = seconds / Year;
            seconds -= y * Year;
            var d = seconds / Day;
            seconds -= d * Day;
            var h = seconds / Hour;
            seconds -= h * Hour;
            var m = seconds / Minute;
            seconds -= m * Minute;

            var s1 = string.Join
            (
                ", ", 
                new[] 
                {
                    output(y, "year"), 
                    output(d, "day"), 
                    output(h, "hour"), 
                    output(m,"minute"),
                    output(seconds, "second")
                }.Where(s => s != null)
            );

            var cIndex = s1.LastIndexOf(',');
            if (cIndex != -1)
                s1 = s1.Substring(0, cIndex) + " and" + s1.Substring(cIndex + 1);

            return s1;
        }
    }
}
