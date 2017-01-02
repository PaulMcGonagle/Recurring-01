using NodaTime;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using NodaTime.Testing;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test
{
    public class HolidaysExclusionTest
    {
        private CompositeSchedule _term;
        private IEnumerable<Date> _holidays;

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

        public void WhenThereAreSomeHolidays(IEnumerable<Date> holidays)
        {
            _holidays = holidays;

            _term.ExclusionsEdges.Add(new EdgeVertex<Schedule>(new DateList { Items = _holidays }));
        }

        public void ThenThereShouldBeNoWeekendDays()
        {
            _term.GenerateDates()
                .Where(o => ScheduleTestHelper.WeekendDays.Contains(o.Value.IsoDayOfWeek))
                .ShouldBeEmpty();
        }

        public void AndThenThereShouldBeNoHolidaysMatching()
        {
            _term.GenerateDates()
                .Where(o => ScheduleTestHelper.BankHolidays.Contains(o))
                .ShouldBeEmpty();
        }

        public static CompositeSchedule GenerateTerm()
        {
            return new CompositeSchedule
            {
                InclusionsEdges = new EdgeVertexs<Schedule>
                {
                    new EdgeVertex<Schedule>(new ByWeekdays
                        {
                            Days = ScheduleTestHelper.Weekdays,
                            EdgeRange = new EdgeRange(2016, YearMonth.MonthValue.September, 06, 2016, YearMonth.MonthValue.December, 19),
                        })
                },
            };
        }
    }
}
