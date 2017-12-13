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
            var ranges = DateTimeHelper.Range(firstMonday, lastDate);

            WriteLine(firstDate.DayOfWeek.ToString());

            WriteLine("Year Month  Mon Tue Wed Thu Fri Sat Sun");

            foreach (var range in ranges)
            {
                if (range.Value.DayOfWeek == (int)DayOfWeek.Monday)
                {
                    WriteLine();
                    ForegroundColor = ConsoleColor.White;
                    Write($"{range.Value.Year:0000} {range.Value.Month:00}   ");
                }

                ForegroundColor = sortedDates.Contains(range.Value) ? ConsoleColor.White : ConsoleColor.Red;
                Write($"  {range.Value.Day:00}");
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

            foreach (var sortedDate in sortedDates)
            {
                WriteLine(sortedDate.ToString());
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

            foreach (var sortedDateTime in sortedDateTimes)
            {
                WriteLine(sortedDateTime.ToString());
            }
        }

        public static void DisplayList(IEnumerable<IEpisode> episodes)
        {
            if (episodes == null)
            {
                WriteLine("empty");
                return;
            }

            foreach (var episode in episodes)
            {
                WriteLine($"Episode, start {episode.Start}, period: {episode.Period}");
            }
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public static void Wait()
        {
            ReadLine();
        }

        #endregion

    }
}
