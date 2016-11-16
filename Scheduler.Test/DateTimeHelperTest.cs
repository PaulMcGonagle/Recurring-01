using NodaTime;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.DateTimeHelperRangeTest
{
    public class RangeVerification
    {
        LocalDate _dateFrom;
        int _count;
        int _interval;

        IEnumerable<LocalDate> _dates;

        [Fact]
        public void Execute()
        {

            var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

            this.WithExamples(new ExampleTable("dateFrom", "count", "interval", "expectedDates")
                {
                    {   DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 01),
                        20,
                        7,
                        Enumerable.Range(0, 20 * 7)
                            .Where(e => e % 7 == 0)
                            .Select(e => DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 01).PlusDays(e))
                            .ToList()},
                    {   DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 01),
                        4,
                        2,
                        Enumerable.Range(0, 8)
                            .Where(e => e % 2 == 0)
                            .Select(e => DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 01).PlusDays(e))
                            .ToList()},
                    {   DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 01),
                        4,
                        1,
                        Enumerable.Range(0, 4)
                            .Select(e => DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.April, 01).PlusDays(e)) },
                })
                .BDDfy();
        }

        private void GivenADateFrom(LocalDate dateFrom)
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

        public void ThenTheDatesAreAsExpected(IEnumerable<LocalDate> expectedDates)
        {
            _dates.ShouldBe(expectedDates);
        }
    }
}
