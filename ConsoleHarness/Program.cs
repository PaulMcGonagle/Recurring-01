using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Scheduler.Calendars;
using static System.Console;

namespace ConsoleHarness
{
    class Program
    {
        static readonly Dictionary<string, Scheduler.ISchedule> Calendars = new Dictionary<string, Scheduler.ISchedule>();

        static readonly IList<IsoDayOfWeek> WeekdaysMonToFri = new List<IsoDayOfWeek>()
            {
                IsoDayOfWeek.Monday,
                IsoDayOfWeek.Tuesday,
                IsoDayOfWeek.Wednesday,
                IsoDayOfWeek.Thursday,
                IsoDayOfWeek.Friday,
            };

        static readonly IList<IsoDayOfWeek> WeekdaysSatToSun = new List<IsoDayOfWeek>()
            {
                IsoDayOfWeek.Saturday,
                IsoDayOfWeek.Sunday,
            };

        static void Main(string[] args)
        {
            Calendars.Add("Weekdays", new Scheduler.ScheduleInstances.ByWeekdays() { Days = WeekdaysMonToFri });
            Calendars.Add("Weekends", new Scheduler.ScheduleInstances.ByWeekdays() { Days = WeekdaysSatToSun });
            Calendars.Add("English Holidays",
                new Scheduler.ScheduleInstances.DateList
                {
                    Items = new List<LocalDate>
                    {
                        new LocalDate(2016, 01, 01),
                        new LocalDate(2016, 03, 25),
                        new LocalDate(2016, 03, 28),
                        new LocalDate(2016, 05, 02),
                        new LocalDate(2016, 05, 30),
                        new LocalDate(2016, 08, 29),
                        new LocalDate(2016, 12, 26),
                        new LocalDate(2016, 12, 27),
                        new LocalDate(2017, 01, 02),
                        new LocalDate(2017, 04, 14),
                        new LocalDate(2017, 04, 17),
                        new LocalDate(2017, 05, 01),
                        new LocalDate(2017, 05, 29),
                        new LocalDate(2017, 08, 28),
                        new LocalDate(2017, 12, 25),
                        new LocalDate(2017, 12, 26),
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

            Calendars.Add("2016 - Autumn 1", new Scheduler.ScheduleInstances.ByWeekdays()
            {
                Days = WeekdaysMonToFri,
                DateFrom = new LocalDate(2016, 09, 05),
                DateTo = new LocalDate(2016, 10, 21),
            });
            Calendars.Add("2016 - Autumn 2", new Scheduler.ScheduleInstances.ByWeekdays()
            {
                Days = WeekdaysMonToFri,
                DateFrom = new LocalDate(2016, 10, 31),
                DateTo = new LocalDate(2016, 12, 21),
            });
            Calendars.Add("2017 - Spring 1", new Scheduler.ScheduleInstances.ByWeekdays()
            {
                Days = WeekdaysMonToFri,
                DateFrom = new LocalDate(2017, 01, 04),
                DateTo = new LocalDate(2017, 02, 12),
            });
            Calendars.Add("2017 - Spring 2", new Scheduler.ScheduleInstances.ByWeekdays()
            {
                Days = WeekdaysMonToFri,
                DateFrom = new LocalDate(2017, 02, 24),
                DateTo = new LocalDate(2017, 03, 24),
            });
            Calendars.Add("2017 - Summer 1", new Scheduler.ScheduleInstances.ByWeekdays()
            {
                Days = WeekdaysMonToFri,
                DateFrom = new LocalDate(2017, 04, 11),
                DateTo = new LocalDate(2017, 05, 27),
            });
            Calendars.Add("2017 - Summer 2", new Scheduler.ScheduleInstances.ByWeekdays()
            {
                Days = WeekdaysMonToFri,
                DateFrom = new LocalDate(2017, 06, 06),
                DateTo = new LocalDate(2017, 07, 20),
            });

            var schoolYear = new List<Scheduler.ISchedule>();
            foreach (var calendar in Calendars)
                if (schoolYearCalendars.Contains(calendar.Key))
                    schoolYear.Add(calendar.Value);

            var year = new Scheduler.CompositeSchedule
            {
                Inclusions = schoolYear,

                Exclusions = new List<Scheduler.ISchedule>() { Calendars["English Holidays"], },
            };

            DisplayGrid(year.Dates());

            ReadKey();
        }

        private static void Test1()
        {
            var s3 = new Scheduler.ScheduleInstances.ByDayOfMonth()
            {
                DayOfMonth = 15,
                DateFrom = new LocalDate(2015, 04, 15),
                DateTo = new LocalDate(2015, 08, 15),
            };

            DisplayList(s3.Dates());

            ReadKey();
        }

        private static void Test2()
        {
            var calendarEvents = new CalendarEvents
            {
                new CalendarEvent
                {
                    TimeStart = new LocalTime(hour: 15, minute: 30),
                    Period = new PeriodBuilder {Minutes = 15,}.Build(),
                    Schedule = new Scheduler.ScheduleInstances.ByDayOfMonth()
                    {
                        DayOfMonth = 15,
                    }
                }
            };

            var episodes = calendarEvents.First().Episodes();

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
            var range = Scheduler.DateTimeHelper.Range(firstMonday, lastDate);

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

        static void DisplayList(IEnumerable<Scheduler.Episode> appointments)
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
