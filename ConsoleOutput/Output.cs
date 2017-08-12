using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Scheduler;
using static System.Console;

namespace ConsoleOutput
{
    public static class Output
    {

        #region Display

        public static void DisplayGrid(IEnumerable<IDate> dates)
        {
            if (dates == null)
            {
                WriteLine("empty");
                return;
            }

            var sortedDates = dates
                .Select(d => d.Value)
                .ToList();
            sortedDates.Sort();

            var firstDate = sortedDates.First();
            var lastDate = sortedDates.Last();

            var firstMonday = firstDate.PlusDays(-firstDate.DayOfWeek + 1);
            var range = DateTimeHelper.Range(firstMonday, lastDate);

            WriteLine(firstDate.DayOfWeek.ToString());

            WriteLine("Year Month  Mon Tue Wed Thu Fri Sat Sun");

            foreach (var r in range)
            {
                if (r.Value.DayOfWeek == (int)DayOfWeek.Monday)
                {
                    WriteLine();
                    ForegroundColor = ConsoleColor.White;
                    Write($"{r.Value.Year:0000} {r.Value.Month:00}   ");
                }

                ForegroundColor = sortedDates.Contains(r.Value) ? ConsoleColor.White : ConsoleColor.Red;
                Write($"  {r.Value.Day:00}");
            }
            WriteLine();
        }

        public static void DisplayList(IEnumerable<ZonedDateTime> dates)
        {
            if (dates == null)
            {
                WriteLine("empty");
                return;
            }

            var sortedDates = dates.ToList();
            sortedDates.Sort();

            foreach (var d in sortedDates)
            {
                WriteLine(d.ToString());
            }
        }

        public static void DisplayList(IEnumerable<LocalDateTime> dateTimes)
        {
            if (dateTimes == null)
            {
                WriteLine("empty");
                return;
            }

            var sortedDateTimes = dateTimes.ToList();
            sortedDateTimes.Sort();

            foreach (var d in sortedDateTimes)
            {
                WriteLine(d.ToString());
            }
        }

        public static void DisplayList(IEpisodes episodes)
        {
            if (episodes == null)
            {
                WriteLine("empty");
                return;
            }

            foreach (var a in episodes)
            {
                WriteLine(a.ToString());
            }
        }

        public static void Wait()
        {
            ReadKey();
        }

        #endregion

    }
}
