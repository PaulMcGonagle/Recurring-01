using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using NodaTime.Testing;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Shouldly;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test
{
    public class SerialTests
    {
        public class VerifyDateOutOfBoundsExceptionIsThrown
        {
            private SerialBuilder _sut;
            private IClock _clock;
            private Serial _serial;
            private IEdgeVertexs<IEpisode> _episodes;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";
                var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

                this.WithExamples(new ExampleTable("sut", "clock", "expectedEpisodes")
                    {
                        {
                            new SerialBuilder
                            {
                                EdgeSchedule = new EdgeSchedule(new ByDateList
                                {
                                    Items = new EdgeVertexs<IDate>()
                                    {
                                        new EdgeVertex<IDate>(new Date(2016, YearMonth.MonthValue.January, 05)),
                                        new EdgeVertex<IDate>(new Date(2016, YearMonth.MonthValue.January, 06)),
                                        new EdgeVertex<IDate>(new Date(2016, YearMonth.MonthValue.January, 07)),
                                    }
                                }),
                                RangeTime = new EdgeRangeTime(new LocalTime(15, 30), new PeriodBuilder {Hours = 00, Minutes = 30,}.Build()),
                                TimeZoneProvider = timeZoneProvider
                            },
                            fakeClock,
                            new Episodes
                            {
                                new Episode
                                {
                                    Start = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 05), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),

                                },
                                new Episode
                                {
                                    Start = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 06), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                                new Episode
                                {
                                    Start = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 07), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenABuilder(SerialBuilder sut)
            {
                _sut = sut;
            }

            public void WhenSerialIsBuilt()
            {
                _serial = _sut.Build();
            }

            public void WhenEpisodesAreRetrieved()
            {
                _episodes = _serial.GenerateEpisodes(_clock);
            }

            public void ThenEpisodesAreExpected(IEpisodes expectedEpisodes)
            {
                _episodes
                    .Select(e => e.ToVertex)
                    .ShouldBe(expectedEpisodes);
            }
        }

        public class VerifyMissingPropertyThrowsArgumentException
        {
            private SerialBuilder _sut;
            private IClock _clock;
            private Serial _serial;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";
                var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

                this.WithExamples(new ExampleTable(
                    "sut",
                    "clock",
                    "paramName")
                    {
                        {
                            new SerialBuilder
                            {
                                EdgeSchedule = new EdgeSchedule(new ByDateList { Items = new EdgeVertexs<IDate>(), }),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            fakeClock,
                            "RangeTime"
                        },
                        {
                            new SerialBuilder
                            {
                                EdgeSchedule = new EdgeSchedule(new ByDateList { Items = new EdgeVertexs<IDate>(), }),
                                RangeTime = new EdgeRangeTime(
                                    start: new LocalTime(15, 30),
                                    period: new PeriodBuilder { Minutes = 30 }.Build()),
                            },
                            fakeClock,
                            "TimeZoneProvider"
                        },
                        {
                            new SerialBuilder
                            {
                                RangeTime = new EdgeRangeTime(
                                    start: new LocalTime(15, 30),
                                    period: new PeriodBuilder { Minutes = 30 }.Build()),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            fakeClock,
                            "EdgeSchedule"
                        },
                    })
                    .BDDfy();
            }

            public void GivenABuilder(SerialBuilder sut)
            {
                _sut = sut;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenEpisodesAreRetrieved()
            {
                _exception = Record.Exception(() => { _serial = _sut.Build(); });
            }

            public void ThenArgumentExceptionIsThrown(string paramName)
            {
                _exception.ShouldBeOfType<ArgumentNullException>();
                var a = (ArgumentNullException) _exception;

                a.ParamName.ShouldBe(paramName);
            }
        }
    }
}