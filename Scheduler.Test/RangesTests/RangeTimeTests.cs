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
    public class RangeTimeTests
    {

        public class VerifyCanBeSaved
        {
            private RangeTime _rangeTime;
            private IClock _clock;
            private IArangoDatabase _db;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<RangeTime>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                        "SUT",
                        "db",
                        "clock"
                    )
                    {
                        {
                            new RangeTimeBuilder
                            {
                                Start = new LocalTime(09, 15),
                                End = new LocalTime(10, 10)
                            }.Build(),
                            mockDb.Object,
                            new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15))
                        },
                    })
                    .BDDfy();
            }

            public void GivenARangeTime(RangeTime sut)
            {
                _rangeTime = sut;
            }

            public void AndGivenTimabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenSaved()
            {
                _rangeTime.Save(_db, _clock);
            }

            public void ThenRangeTimeIsPersisted()
            {
                _rangeTime.IsPersisted.ShouldBeTrue();
            }
        }

        public class RangeTimeCanBeBuilt
        {
            private RangeTimeBuilder _rangeTimeBuilder;
            private RangeTime _rangeTime;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                        "SUT",
                        "startTime",
                        "endTime",
                        "expectedStartTime",
                        "expectedEndTime")
                    {
                        {
                            new RangeTimeBuilder(),
                            new LocalTime(10, 30),
                            new LocalTime(12, 00),
                            new LocalTime(10, 30),
                            new LocalTime(12, 00)
                        },
                    })
                    .BDDfy();
            }

            public void GivenBuilder(RangeTimeBuilder sut)
            {
                _rangeTimeBuilder = sut;
            }

            public void AndGivenAStartTime(LocalTime startTime)
            {
                _rangeTimeBuilder.Start = startTime;
            }

            public void AndGivenAnEndTime(LocalTime endTime)
            {
                _rangeTimeBuilder.End = endTime;
            }

            public void WhenBuilt()
            {
                _rangeTime = _rangeTimeBuilder.Build();
            }

            public void AndThenStartTimeIsExpected(LocalTime expectedStartTime)
            {
                _rangeTime.Start.ShouldBe(expectedStartTime);
            }

            public void AndThenEndTimeIsExpected(LocalTime expectedEndTime)
            {
                _rangeTime.End.ShouldBe(expectedEndTime);
            }
        }

        public class MissingParameterThrowsException
        {
            private RangeTimeBuilder _rangeTimeBuilder;
            private RangeTime _rangeTime;
            private Exception _exception;

            [Fact]
            public void Execute()
            {
                var timeEarlier = new LocalTime(08, 00);
                var timeLater = new LocalTime(11, 15);

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "expectedException")
                {
                    //{
                    //    new RangeTimeBuilder
                    //    {
                    //        Start = timeEarlier
                    //    },
                    //    new ArgumentNullException(nameof(_rangeTimeBuilder.Period))
                    //},
                    //{
                    //    new RangeTimeBuilder
                    //    {
                    //        End = timeLater
                    //    },
                    //    new ArgumentNullException(nameof(_rangeTimeBuilder.Start))
                    //},
                    {
                        new RangeTimeBuilder
                        {
                            Start = timeLater,
                            End = timeEarlier
                        },
                        new ArgumentOutOfRangeException(nameof(_rangeTime.Period))
                    },
                }).BDDfy();
            }

            public void GivenAPrePopulatedBuilder(RangeTimeBuilder sut)
            {
                _rangeTimeBuilder = sut;
            }

            public void WhenBuilt()
            {
                _exception = Record.Exception(() => _rangeTime = _rangeTimeBuilder.Build());
            }

            public void ThenExceptionIsExpected(Exception expectedException)
            {
                _exception.ShouldBeOfType(expectedException.GetType());
                _exception.Message.ShouldBe(expectedException.Message);
            }
        }
    }
}
