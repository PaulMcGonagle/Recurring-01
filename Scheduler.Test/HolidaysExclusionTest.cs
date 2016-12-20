using NodaTime;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test
{
    public class HolidaysExclusionTest
    {
        private CompositeSchedule _term;
        private IEnumerable<Scheduler.Date> _holidays;

        [Fact]
        public void RunExamplesWithFluentApi()
        {
            var t = GenerateTerm();

            this.WithExamples(new ExampleTable("term", "holidays")
                {
                    {   t, ScheduleTestHelper.BankHolidays },
                    {   t, ScheduleTestHelper.BankHolidays.Where(b => b.Value.Year == 2016) },
                })
                .BDDfy();
        }

        public void GivenWeHaveATermOfWeekdays(CompositeSchedule term)
        {
            _term = term;
        }

        public void WhenThereAreSomeHolidays(IEnumerable<Scheduler.Date> holidays)
        {
            _holidays = holidays;

            _term.Exclusions.Add(new DateList { Items = _holidays });
        }

        public void ThenThereShouldBeNoWeekendDays()
        {
            _term.Dates
                .Where(o => ScheduleTestHelper.WeekendDays.Contains(o.Value.IsoDayOfWeek))
                .ShouldBeEmpty();
        }

        public void AndThenThereShouldBeNoHolidaysMatching()
        {
            _term.Dates
                .Where(o => ScheduleTestHelper.BankHolidays.Contains(o))
                .ShouldBeEmpty();
        }

        public static CompositeSchedule GenerateTerm()
        {
            return new CompositeSchedule
            {
                Inclusions = new Schedules(
                    new ByWeekdays
                    {
                        Days = ScheduleTestHelper.Weekdays,
                        Range = new Range(2016, YearMonth.MonthValue.September, 06, 2016, YearMonth.MonthValue.December, 19),
                    })
            };
        }
    }
}
