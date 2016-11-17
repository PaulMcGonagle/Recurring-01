using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Scheduler.Calendars;

namespace ConsoleHarness
{
    class Program
    {
        static Dictionary<string, Scheduler.ISchedule> Calendars = new Dictionary<string, Scheduler.ISchedule>();

        static IList<IsoDayOfWeek> WeekdaysMonToFri = new List<IsoDayOfWeek>()
            {
                IsoDayOfWeek.Monday,
                IsoDayOfWeek.Tuesday,
                IsoDayOfWeek.Wednesday,
                IsoDayOfWeek.Thursday,
                IsoDayOfWeek.Friday,
            };

        static IList<IsoDayOfWeek> WeekdaysSatToSun = new List<IsoDayOfWeek>()
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

            Console.ReadKey();
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

            Console.ReadKey();
        }

        private static void Test2()
        {
            var calendarEvents = new CalendarEvents();

            Period p = new PeriodBuilder { Hours = 15, Minutes = 30, }.Build();
            calendarEvents.Add(new CalendarEvent()
            {
                TimeStart = new NodaTime.LocalTime(hour: 15, minute: 30),
                Period = new PeriodBuilder { Minutes = 15, }.Build(),
                Schedule = new Scheduler.ScheduleInstances.ByDayOfMonth()
                {
                    DayOfMonth = 15,
                }
            });

            var episodes = calendarEvents.First().Episodes();

            var sorted = episodes.OrderByDescending(x => x.From);
            DisplayList(sorted);

            Console.ReadKey();
        }

        static void DisplayGrid(IEnumerable<LocalDate> dates)
        {
            if (dates == null)
            {
                Console.WriteLine("empty");
                return;
            }

            var sortedDates = dates.ToList();
            sortedDates.Sort();

            LocalDate firstDate = sortedDates.First();
            LocalDate lastDate = sortedDates.Last();

            LocalDate firstMonday = firstDate.PlusDays(-(int)firstDate.DayOfWeek + 1);
            var range = Scheduler.DateTimeHelper.Range(firstMonday, lastDate);
            Console.WriteLine(((int)firstDate.DayOfWeek).ToString());

            Console.WriteLine("Year Month  Mon Tue Wed Thu Fri Sat Sun");
             
            foreach (var r in range)
            {
                if (r.DayOfWeek == (int)DayOfWeek.Monday)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(string.Format("{0} {1}   ", r.Year.ToString("0000"), r.Month.ToString("00")));
                }

                Console.ForegroundColor = sortedDates.Contains(r) ? ConsoleColor.White : ConsoleColor.Red;
                Console.Write(string.Format("  {0}", r.Day.ToString("00")));
            }
            Console.WriteLine();
        }

        static void DisplayList(IEnumerable<LocalDate> dates)
        {
            if (dates == null)
            {
                Console.WriteLine("empty");
                return;
            }

            var sortedDates = dates.ToList();
            sortedDates.Sort();

            foreach (var d in sortedDates)
            {
                Console.WriteLine(d.ToString());
            }
        }

        static void DisplayList(IEnumerable<Scheduler.Episode> appointments)
        {
            if (appointments == null)
            {
                Console.WriteLine("empty");
                return;
            }

            foreach (var a in appointments)
            {
                Console.WriteLine(a.ToString());
            }
        }
    }
}
