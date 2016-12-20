using NodaTime;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using NodaTime.Testing;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test
{
    public class DateTimeHelperRangeTest
    {
        public class RangeVerification
        {
            Scheduler.Date _dateFrom;
            int _count;
            int _interval;

            IEnumerable<Scheduler.Date> _dates;

            [Fact]
            public void Execute()
            {

                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(new ExampleTable("dateFrom", "count", "interval", "expectedDates")
                    {
                        {
                            new Scheduler.Date(2016, YearMonth.MonthValue.December, 01),
                            20,
                            7,
                            Enumerable.Range(0, 20*7)
                                .Where(e => e%7 == 0)
                                .Select(
                                    e =>
                                        new Scheduler.Date(2016, YearMonth.MonthValue.December, 01)
                                            .PlusDays(e))
                                .ToList()
                        },
                        {
                            new Scheduler.Date(2016, YearMonth.MonthValue.December, 01),
                            4,
                            2,
                            Enumerable.Range(0, 8)
                                .Where(e => e%2 == 0)
                                .Select(
                                    e =>
                                        new Scheduler.Date(2016, YearMonth.MonthValue.December, 01)
                                            .PlusDays(e))
                        },
                        {
                            new Scheduler.Date(2016, YearMonth.MonthValue.April, 01),
                            4,
                            1,
                            Enumerable.Range(0, 4)
                                .Select(
                                    e => new Scheduler.Date(2016, YearMonth.MonthValue.April, 01).PlusDays(e))
                        },
                    })
                    .BDDfy();
            }

            private void GivenADateFrom(Scheduler.Date dateFrom)
            {
                _dateFrom = dateFrom;
            }

            private void GivenACount(int count)
            {
                _count = count;
            }

            private void GivenAnInterval(int interval)
            {
                _interval = interval;
            }

            public void WhenDatesAreRetrieved()
            {
                _dates = DateTimeHelper.Range(_dateFrom, _count, _interval);
            }

            public void ThenTheDatesAreAsExpected(IEnumerable<Scheduler.Date> expectedDates)
            {
                _dates
                    .Select(d => d.Value)
                    .ShouldBe(expectedDates.Select(e => e.Value));
            }
        }

        public class GetTodayVerification
        {

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("clock", "expectedDate")
                {
                    {
                        ScheduleTestHelper.GetFakeClock(2016, 05, 01),
                        new Scheduler.Date(2016, YearMonth.MonthValue.April, 01)
                    },
                }).BDDfy();
            }

            public void WhenTodayIsRetreived(IClock clock)
            {

            }

            public void ThenDateIsExpected(Scheduler.Date expectedDate)
            {

            }
        }
    }
}