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
            Calendars.Add("Weekdays", new Scheduler.ScheduleInstances.ByWeekdays { Days = WeekdaysMonToFri });
            Calendars.Add("Weekends", new Scheduler.ScheduleInstances.ByWeekdays { Days = WeekdaysSatToSun });
            Calendars.Add("English Holidays",
                new Scheduler.ScheduleInstances.DateList
                {
                    Items = new List<LocalDate>
                    {
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.January, 01),
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 25),
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 28),
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.May, 02),
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.May, 30),
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.August, 29),
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 26),
                        DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 27),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.January, 02),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 14),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 17),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.May, 01),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.May, 29),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.August, 28),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.December, 25),
                        DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.December, 26),
                    }
                }
            );

            var schoolYearCalendars = new List<string>
            {
                "2016 - Autumn 1",
                "2016 - Autumn 2",
                "2017 - Spring 1",
                "2017 - Spring 2",
                "2017 - Summer 1",
                "2017 - Summer 2",
            };

            Calendars.Add("2016 - Autumn 1", new Scheduler.ScheduleInstances.ByWeekdays
            {
                Days = WeekdaysMonToFri,
                DateFrom = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.September, 05),
                DateTo = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.October, 21),
            });
            Calendars.Add("2016 - Autumn 2", new Scheduler.ScheduleInstances.ByWeekdays
            {
                Days = WeekdaysMonToFri,
                DateFrom = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.October, 31),
                DateTo = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 21),
            });
            Calendars.Add("2017 - Spring 1", new Scheduler.ScheduleInstances.ByWeekdays
            {
                Days = WeekdaysMonToFri,
                DateFrom = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.January, 04),
                DateTo = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.February, 12),
            });
            Calendars.Add("2017 - Spring 2", new Scheduler.ScheduleInstances.ByWeekdays
            {
                Days = WeekdaysMonToFri,
                DateFrom = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.February, 24),
                DateTo = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.March, 24),
            });
            Calendars.Add("2017 - Summer 1", new Scheduler.ScheduleInstances.ByWeekdays
            {
                Days = WeekdaysMonToFri,
                DateFrom = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.April, 11),
                DateTo = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.May, 27),
            });
            Calendars.Add("2017 - Summer 2", new Scheduler.ScheduleInstances.ByWeekdays
            {
                Days = WeekdaysMonToFri,
                DateFrom = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.June, 06),
                DateTo = DateTimeHelper.GetLocalDate(2017, YearMonth.MonthValue.July, 20),
            });

            var schoolYear = Calendars
                .Where(c => schoolYearCalendars.Contains((c.Key)))
                .Select(c => c.Value)
                .ToList();

            var year = new CompositeSchedule
            {
                Inclusions = schoolYear,

                Exclusions = new List<ISchedule> { Calendars["English Holidays"], },
            };

            DisplayGrid(year.Dates);

            ReadKey();
        }

        private static void Test1()
        {
            var s3 = new Scheduler.ScheduleInstances.ByDayOfMonth
            {
                DayOfMonth = 15,
                DateFrom = DateTimeHelper.GetLocalDate(2015, YearMonth.MonthValue.April, 15),
                DateTo = DateTimeHelper.GetLocalDate(2015, YearMonth.MonthValue.August, 15),
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
