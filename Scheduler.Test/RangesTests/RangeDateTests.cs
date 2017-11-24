using System;
using System.Globalization;
using ArangoDB.Client;
using NodaTime;
using NodaTime.Testing;
using Scheduler.Ranges;
using Shouldly;
using TestHelpers;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test.RangesTests
{
    public class RangeDateTests
    {

        public class VerifyCanBeSaved
        {
            private RangeDate _rangeDate;
            private IClock _clock;
            private IArangoDatabase _db;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<RangeDate>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                        "SUT",
                        "db",
                        "clock"
                    )
                    {
                        {
                            new RangeDate.Builder
                            {
                                Start = new Date(2016, YearMonth.MonthValue.January, 01),
                                End = new Date(2016, YearMonth.MonthValue.February, 01)
                            }.Build(),
                            mockDb.Object,
                            new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15))
                        },
                    })
                    .BDDfy();
            }

            public void GivenARangeDate(RangeDate sut)
            {
                _rangeDate = sut;
            }

            public void AndGivenDatabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenSaved()
            {
                _rangeDate.Save(_db, _clock);
            }

            public void ThenRangeDateIsPersisted()
            {
                _rangeDate.IsPersisted.ShouldBeTrue();
            }
        }

        public class RangeDateCanBeBuilt
        {
            private RangeDate.Builder _rangeDateBuilder;
            private RangeDate _rangeDate;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                        "SUT",
                        "startDate",
                        "endDate",
                        "expectedStartDate",
                        "expectedEndDate")
                    {
                        {
                            new RangeDate.Builder(),
                            new Date(2016, YearMonth.MonthValue.January, 01),
                            new Date(2017, YearMonth.MonthValue.February, 28),
                            new LocalDate(2016, 01, 01),
                            new LocalDate(2017, 02, 28)
                        },
                    })
                    .BDDfy();
            }

            public void GivenBuilder(RangeDate.Builder sut)
            {
                _rangeDateBuilder = sut;
            }

            public void AndGivenAStartDate(IDate startDate)
            {
                _rangeDateBuilder.Start = startDate;
            }

            public void AndGivenAnEndDate(IDate endDate)
            {
                _rangeDateBuilder.End = endDate;
            }

            public void WhenBuilt()
            {
                _rangeDate = _rangeDateBuilder.Build();
            }

            public void AndThenStartDateIsExpected(LocalDate expectedStartDate)
            {
                _rangeDate.Start.Date.Value.ShouldBe(expectedStartDate);
            }

            public void AndThenEndDateIsExpected(LocalDate expectedEndDate)
            {
                _rangeDate.End.Date.Value.ShouldBe(expectedEndDate);
            }
        }

        public class MissingParameterThrowsException
        {
            private RangeDate.Builder _rangeDateBuilder;
            private RangeDate _rangeDate;
            private Exception _exception;

            [Fact]
            public void Execute()
            {
                IDate dateEarlier = new Date(2016, YearMonth.MonthValue.January, 01);
                IDate dateLater = new Date(2016, YearMonth.MonthValue.February, 01);

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "expectedException")
                {
                    {
                        new RangeDate.Builder
                        {
                            Start = dateEarlier
                        },
                        new ArgumentNullException(nameof(RangeDate.Builder.End))
                    },
                    {
                        new RangeDate.Builder
                        {
                            End = dateLater
                        },
                        new ArgumentNullException(nameof(RangeDate.Builder.Start))
                    },
                    {
                        new RangeDate.Builder
                        {
                            Start = dateLater,
                            End = dateEarlier,
                        },
                        new ArgumentOutOfRangeException(nameof(_rangeDate.Start), $"Start date [{dateLater.Value.ToString("D", CultureInfo.CurrentCulture)}] cannot be greater than End date [{dateEarlier.Value.ToString("D", CultureInfo.CurrentCulture)}]")
                    },
                }).BDDfy();
            }

            public void GivenAPrePopulatedBuilder(RangeDate.Builder sut)
            {
                _rangeDateBuilder = sut;
            }

            public void WhenBuilt()
            {
                _exception = Record.Exception(() => _rangeDate = _rangeDateBuilder.Build());
            }

            public void ThenExceptionIsExpected(Exception expectedException)
            {
                _exception.ShouldBeOfType(expectedException.GetType());
                _exception.Message.ShouldBe(expectedException.Message);
            }
        }
    }
}
