using Shouldly;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
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
        private IClock _clock;
        private IEnumerable<IDate> _holidays;

        [Fact]
        public void RunExamplesWithFluentApi()
        {
            var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 03, 15);
            var term = GenerateTerm();

            this.WithExamples(
                new ExampleTable(
                    "term", 
                    "clock", 
                    "holidays")
                {
                    {
                        term,
                        fakeClock,
                        ScheduleTestHelper.BankHolidays
                    },
                    {
                        term,
                        fakeClock,
                        ScheduleTestHelper.BankHolidays.Where(b => b.Value.Year == 2016) },
                })
                .BDDfy();
        }

        public void GivenWeHaveATermOfWeekdays(CompositeSchedule term)
        {
            _term = term;
        }

        public void AndGivenAClock(IClock clock)
        {
            _clock = clock;
        }

        public void WhenThereAreSomeHolidays(IEnumerable<IDate> holidays)
        {
            _holidays = holidays;

            var dateList = new ByDateList();

            ISchedule d = 
                ByDateList.Create(_holidays);

            _term.Exclusions.Add(new EdgeVertex<ISchedule>(d));
        }

        public void ThenThereShouldBeNoWeekendDays()
        {
            _term
                .Generate(_clock)
                .Where(date => ScheduleTestHelper.WeekendDays.Contains(date.Value.IsoDayOfWeek))
                .ShouldBeEmpty();
        }

        public void AndThenThereShouldBeNoHolidaysMatching()
        {
            _term
                .Generate(_clock)
                .Where(date => ScheduleTestHelper.BankHolidays.Contains(date))
                .ShouldBeEmpty();
        }

        public static CompositeSchedule GenerateTerm()
        {
            return new CompositeSchedule
            {
                Inclusions = new EdgeVertexs<ISchedule>
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
