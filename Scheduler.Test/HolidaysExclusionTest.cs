using Shouldly;
using System.Collections.Generic;
using System.Linq;
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
        private IEnumerable<IDate> _holidays;

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

        public void WhenThereAreSomeHolidays(IEnumerable<IDate> holidays)
        {
            _holidays = holidays;

            var dateList = new DateList();

            ISchedule d = new DateList {Items = _holidays};
            _term.ExclusionsEdges.Add(new EdgeVertex<ISchedule>(d));
        }

        public void ThenThereShouldBeNoWeekendDays()
        {
            _term.Generate()
                .Where(date => ScheduleTestHelper.WeekendDays.Contains(date.Value.IsoDayOfWeek))
                .ShouldBeEmpty();
        }

        public void AndThenThereShouldBeNoHolidaysMatching()
        {
            _term.Generate()
                .Where(date => ScheduleTestHelper.BankHolidays.Contains(date))
                .ShouldBeEmpty();
        }

        public static CompositeSchedule GenerateTerm()
        {
            return new CompositeSchedule
            {
                InclusionsEdges = new EdgeVertexs<ISchedule>
                {
                    new EdgeVertex<ISchedule>(new ByWeekdays
                        {
                            Days = ScheduleTestHelper.Weekdays,
                            EdgeRange = new EdgeRangeDate(2016, YearMonth.MonthValue.September, 06, 2016, YearMonth.MonthValue.December, 19),
                        })
                },
            };
        }
    }
}
