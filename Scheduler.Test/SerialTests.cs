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
            private Serial _sut;
            private IClock _clock;
            private IEdgeVertexs<IEpisode> _episodes;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                this.WithExamples(new ExampleTable("sut", "clock", "expectedEpisodes")
                    {
                        {
                            new Serial(
                                schedule: new DateList
                                    {
                                        Items = new List<IDate>()
                                        {
                                            new Date(2016, YearMonth.MonthValue.January, 05),
                                            new Date(2016, YearMonth.MonthValue.January, 06),
                                            new Date(2016, YearMonth.MonthValue.January, 07),
                                        }
                                    },
                                timeRange: new EdgeRangeTime(new LocalTime(15, 30), new PeriodBuilder {Hours = 00, Minutes = 30,}.Build()),
                                timeZoneProvider: timeZoneProvider),
                            fakeClock,
                            new Episodes
                            {
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 05), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),

                                },
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 06), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 07), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenACalendarEvent(Serial sut)
            {
                _sut = sut;
            }

            public void WhenEpisodesAreRetrieved()
            {
                _episodes = _sut.GenerateEpisodes(_clock);
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
            private Serial _sut;
            private IClock _clock;
            private IEdgeVertexs<IEpisode> _episodes;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";
                var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

                this.WithExamples(new ExampleTable("sut", "clock", "parameterName")
                    {
                        {
                            new Serial(
                                schedule: new DateList { Items = new List<IDate>(), },
                                timeRange: null,
                                timeZoneProvider: timeZoneProvider),
                            fakeClock,
                            "TimeRange"
                        },
                        {
                            new Serial(
                                schedule: new DateList { Items = new List<IDate>(), },
                                timeRange: new EdgeRangeTime(new LocalTime(15, 30), null),
                                timeZoneProvider: timeZoneProvider),
                            fakeClock,
                            "Period"
                        },
                        {
                            new Serial(
                                schedule: new DateList { Items = new List<IDate>(), },
                                timeRange: new EdgeRangeTime(new LocalTime(15, 30), new PeriodBuilder {Minutes = 30,}.Build()),
                                timeZoneProvider: null),
                            fakeClock,
                            "TimeZoneProvider"
                        },
                    })
                    .BDDfy();
            }

            public void GivenACalendarEvent(Serial sut)
            {
                _sut = sut;
            }

            public void WhenEpisodesAreRetrieved()
            {
                _exception = Record.Exception(() => { _episodes = _sut.GenerateEpisodes(_clock); });
            }

            public void ThenArgumentExceptionIsThrown(string parameterName)
            {
                _exception.ShouldNotBeNull();

                _exception.Message.ShouldBe(parameterName);
            }
        }
    }
}