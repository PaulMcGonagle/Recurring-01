using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Scheduler;
using static System.Console;

namespace ConsoleHarness
{
    class Program
    {
        static readonly Dictionary<string, ISchedule> Calendars = new Dictionary<string, ISchedule>();

        static readonly IList<IsoDayOfWeek> WeekdaysMonToFri = new List<IsoDayOfWeek>
            {
                IsoDayOfWeek.Monday,
                IsoDayOfWeek.Tuesday,
                IsoDayOfWeek.Wednesday,
                IsoDayOfWeek.Thursday,
                IsoDayOfWeek.Friday,
            };

        static readonly IList<IsoDayOfWeek> WeekdaysSatToSun = new List<IsoDayOfWeek>
            {
                IsoDayOfWeek.Saturday,
                IsoDayOfWeek.Sunday,
            };

        static void Main(string[] args)
        {
            var x =
                new Scheduler.CompositeSchedule()
                {

                    Inclusions = new List<ISchedule>
                    {
                        TestData.DataRetrieval.ScheduleArchive["Schools.Term.201617.Autumn"],
                    }
                };

            x.Breaks.Add(TestData.DataRetrieval.Ranges["Schools.Term.201617.Autumn.HalfTerm"]);

            DisplayGrid(x.Dates);

            ReadKey();
        }

        private static void Test1()
        {
            var s3 = new Scheduler.ScheduleInstances.ByDayOfMonth
            {
                DayOfMonth = 15,
                Range = new Range(2015, YearMonth.MonthValue.April, 15, 2015, YearMonth.MonthValue.August, 15),
            };

            DisplayList(s3.Dates);

            ReadKey();
        }

        private static void Test2()
        {
            var calendarEvents = new Serials
            {
                new Serial
                {
                    TimeStart = new LocalTime(15, 30),
                    Period = new PeriodBuilder {Minutes = 15,}.Build(),
                    Schedule = new Scheduler.ScheduleInstances.ByDayOfMonth()
                    {
                        DayOfMonth = 15,
                    }
                }
            };

            var episodes = calendarEvents.First().Episodes;

            var sorted = episodes.OrderByDescending(x => x.From);
            DisplayList(sorted);

            ReadKey();
        }

        private static void DisplayGrid(IEnumerable<LocalDate> dates)
        {
            if (dates == null)
            {
                WriteLine("empty");
                return;
            }

            var sortedDates = dates.ToList();
            sortedDates.Sort();

            var firstDate = sortedDates.First();
            var lastDate = sortedDates.Last();

            var firstMonday = firstDate.PlusDays(-firstDate.DayOfWeek + 1);
            var range = DateTimeHelper.Range(firstMonday, lastDate);

            WriteLine(firstDate.DayOfWeek.ToString());

            WriteLine("Year Month  Mon Tue Wed Thu Fri Sat Sun");
             
            foreach (var r in range)
            {
                if (r.DayOfWeek == (int)DayOfWeek.Monday)
                {
                    WriteLine();
                    ForegroundColor = ConsoleColor.White;
                    Write($"{r.Year:0000} {r.Month:00}   ");
                }

                ForegroundColor = sortedDates.Contains(r) ? ConsoleColor.White : ConsoleColor.Red;
                Write($"  {r.Day:00}");
            }
            WriteLine();
        }

        static void DisplayList(IEnumerable<LocalDate> dates)
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

        static void DisplayList(IEnumerable<Episode> appointments)
        {
            if (appointments == null)
            {
                WriteLine("empty");
                return;
            }

            foreach (var a in appointments)
            {
                WriteLine(a.ToString());
            }
        }
    }
}
