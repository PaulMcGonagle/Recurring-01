using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Scheduler;
using Scheduler.ScheduleInstances;
using static System.Console;
using NodaTime.Serialization.JsonNet;
using NodaTime.Testing;
using Calendar = System.Globalization.Calendar;

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
            var e = new MyCalendar.Event
            {
                Serials = new Serial
                {
                    From = new LocalTime(16, 30),
                    Period = new PeriodBuilder { Minutes = 45 }.Build(),
                    TimeZoneProvider = "Europe/London",

                    Schedule = new CompositeSchedule()
                    {
                        Inclusions = new List<ISchedule>
                        {
                            new ByWeekday
                            {
                                Range =
                                    new Range(2016, YearMonth.MonthValue.January, 01, 2016, YearMonth.MonthValue.January,
                                        05),
                                Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                Weekday = IsoDayOfWeek.Wednesday,
                            }
                        }
                    }
                }
            };


            ReadKey();

            var to2 = e.Serials.Episodes.Select(t => t.To);
            var fo2 = e.Serials.Episodes.Select(t => t.From);

            DisplayList(e.Serials.Episodes);

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

        static void DisplayList(IEnumerable<ZonedDateTime> dates)
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

        static void DisplayList(IEnumerable<LocalDateTime> dateTimes)
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
