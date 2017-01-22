using NodaTime;
using System.Collections.Generic;
using Scheduler;
using static System.Console;

namespace ConsoleHarness
{
    class OldProgram
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
            ScheduleGeneration.Generate.Go("sample");

            ReadKey();
        }
    }
}
