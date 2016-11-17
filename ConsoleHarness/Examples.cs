using Scheduler.ScheduleAbstracts;
using Scheduler;

namespace ConsoleHarness
{
    class Examples
    {
        public static Scheduler.ISchedule The31stOfTheMonth(RepeatingDay.RollStrategyType rollStrategy)
        {
            return new Scheduler.ScheduleInstances.ByDayOfMonth()
            {
                DayOfMonth = 31,
                RollStrategy = rollStrategy,
            };
        }

        public static Scheduler.ISchedule ByDayOfYear(YearMonth.MonthValue month, int dayOfYear, RepeatingDay.RollStrategyType rollStrategy)
        {
            return new Scheduler.ScheduleInstances.ByDayOfYear()
            {
                Month = month,
                DayOfYear = dayOfYear,
                RollStrategy = rollStrategy,
            };
        }

    }
}
