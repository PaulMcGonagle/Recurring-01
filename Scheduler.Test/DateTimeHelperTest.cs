using NodaTime;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scheduler.Generation;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test
{
    [TestClass]
    public class DateTimeHelperRangeTest
    {
        public class RangeVerification
        {
            Date _dateFrom;
            int _count;
            int _interval;

            IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(
                    new ExampleTable(
                        "dateFrom", 
                        "count", 
                        "interval", 
                        "expectedDates")
                    {
                        {
                            new Date(2016, YearMonth.MonthValue.December, 01),
                            20,
                            7,
                            Enumerable.Range(0, 20*7)
                                .Where(e => e%7 == 0)
                                .Select(
                                    e =>
                                        new Date(2016, YearMonth.MonthValue.December, 01)
                                            .PlusDays(e))
                                .ToList()
                        },
                        {
                            new Date(2016, YearMonth.MonthValue.December, 01),
                            4,
                            2,
                            Enumerable.Range(0, 8)
                                .Where(e => e%2 == 0)
                                .Select(
                                    e =>
                                        new Date(2016, YearMonth.MonthValue.December, 01)
                                            .PlusDays(e))
                        },
                        {
                            new Date(2016, YearMonth.MonthValue.April, 01),
                            4,
                            1,
                            Enumerable.Range(0, 4)
                                .Select(
                                    e => new Date(2016, YearMonth.MonthValue.April, 01).PlusDays(e))
                        },
                    })
                    .BDDfy();
            }

            private void GivenADateFrom(Date dateFrom)
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

            public void ThenTheDatesAreAsExpected(IEnumerable<IDate> expectedDates)
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
                        new Date(2016, YearMonth.MonthValue.April, 01)
                    },
                }).BDDfy();
            }

            public void WhenTodayIsRetreived(IClock clock)
            {

            }

            public void ThenDateIsExpected(Date expectedDate)
            {

            }
        }

        public class GetNextWeekdayTest
        {
            private LocalDate _input;
            private IsoDayOfWeek _isoDayOfWeek;
            private LocalDate _rollForwardDate;
            private LocalDate _rollBackDate;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "input",
                    "isoDayOfWeek")
                {
                    {   new LocalDate(2016, 01, 01), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2016, 01, 31), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2016, 02, 29), IsoDayOfWeek.Monday },
                    {   new LocalDate(2016, 12, 28), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2016, 12, 29), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2016, 12, 30), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2016, 12, 31), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2017, 01, 01), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2017, 01, 02), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2017, 01, 03), IsoDayOfWeek.Thursday },
                    {   new LocalDate(2016, 12, 28), IsoDayOfWeek.Monday },
                    {   new LocalDate(2016, 12, 29), IsoDayOfWeek.Monday },
                    {   new LocalDate(2016, 12, 30), IsoDayOfWeek.Monday },
                    {   new LocalDate(2016, 12, 31), IsoDayOfWeek.Monday },
                    {   new LocalDate(2017, 01, 01), IsoDayOfWeek.Monday },
                    {   new LocalDate(2017, 01, 02), IsoDayOfWeek.Monday },
                    {   new LocalDate(2017, 01, 03), IsoDayOfWeek.Monday },
                    {   new LocalDate(2016, 12, 28), IsoDayOfWeek.Friday },
                    {   new LocalDate(2016, 12, 29), IsoDayOfWeek.Friday },
                    {   new LocalDate(2016, 12, 30), IsoDayOfWeek.Friday },
                    {   new LocalDate(2016, 12, 31), IsoDayOfWeek.Friday },
                    {   new LocalDate(2017, 01, 01), IsoDayOfWeek.Friday },
                    {   new LocalDate(2017, 01, 02), IsoDayOfWeek.Friday },
                    {   new LocalDate(2017, 01, 03), IsoDayOfWeek.Friday },
                }).BDDfy();
            }

            public void GivenTheDate(LocalDate input)
            {
                _input = input;
            }

            public void AndGivenTheRequestedDayOfWeek(IsoDayOfWeek isoDayOfWeek)
            {
                _isoDayOfWeek = isoDayOfWeek;
            }

            public void WhenRollForwardDateIsRetrieved()
            {
                _rollForwardDate = DateTimeHelper.GetNextWeekday(_input, _isoDayOfWeek, false);
            }

            public void AndWhenRollBackDateIsRetrieved()
            {
                _rollBackDate = DateTimeHelper.GetNextWeekday(_input, _isoDayOfWeek, true);
            }

            public void ThenRollForwardDateIsCorrectWeekday()
            {
                _rollForwardDate.IsoDayOfWeek.ShouldBe(_isoDayOfWeek);
            }

            public void AndThenRollBackDateIsCorrectWeekday()
            {
                _rollBackDate.IsoDayOfWeek.ShouldBe(_isoDayOfWeek);
            }

            public void AndThenTheRollForwardDateIsNotMoreThanAWeekAhead()
            {
                Period.Between(_input, _rollForwardDate).Days.ShouldBeLessThanOrEqualTo(7);
            }

            public void AndThenTheRollBackDateIsNotMoreThanAWeekAgo()
            {
                Period.Between(_input, _rollBackDate).Days.ShouldBeGreaterThanOrEqualTo(-7);
            }
        }
    }
}